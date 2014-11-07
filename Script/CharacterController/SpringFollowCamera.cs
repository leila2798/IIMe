//-----------------------------------------------------------------------
// <copyright file="SpringFollowCamera.cs" company="Scalify">
//     Copyright (c) 2012 Scalify. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
  
using UnityEngine;

/// <summary>
/// SpringFollowCamera script is a translation of SpringFollowCamera.js
/// </summary>
public class SpringFollowCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 4.0f;
    public float height = 1.0f;
    public float smoothLag = 0.2f;
    public float maxSpeed = 10.0f;
    public float snapLag = 0.3f;
    public float clampHeadPositionScreenSpace = 0.75f;
    public LayerMask lineOfSightMask = 0;

    private bool mIsSnapping = false;
    private Vector3 mHeadOffset = Vector3.zero;

    private Vector3 mCenterOffset = Vector3.zero;
    public Vector3 CenterOffset { get { return mCenterOffset; } }

    private ThirdPersonController controller;
    private Vector3 mVelocity = Vector3.zero;
    private float mTargetHeight = 100000.0f;

    #region MonoBehaviour members
    
    void Start()
    {
        DidChangeTarget();
    }

    void LateUpdate()
    {
        if (target)
        {
            Apply(null, Vector3.zero);
        }
    }

    #endregion

    private void DidChangeTarget()
    {
        if (target)
        {
            CharacterController characterController = (CharacterController) target.collider;
            if (characterController)
            {
                mCenterOffset = characterController.bounds.center - target.position;
                mHeadOffset = mCenterOffset;
                mHeadOffset.y = characterController.bounds.max.y - target.position.y;
            }

            if (target)
            {
                controller = (ThirdPersonController)target.GetComponent(typeof(ThirdPersonController));
            }

            if (!controller)
            {
                Debug.Log("ThirdPersonController is not found");
            }
        }
    }

    private void Apply(Transform dummyTarget, Vector3 dummyCenter)
    {
        Vector3 targetCenter = target.position + mCenterOffset; 
        Vector3 targetHead = target.position + mHeadOffset; 

        if (controller.IsJumping())
        {
            float newTargetHeight = targetCenter.y + height;
            if (newTargetHeight < mTargetHeight || newTargetHeight - mTargetHeight > 5)
            {
                mTargetHeight = targetCenter.y + height;
            }
        }
        else
        {
            mTargetHeight = targetCenter.y + height;
        }

        mTargetHeight = targetCenter.y + height;

        if (Input.GetButton("Fire2") && !mIsSnapping)
        {
            mVelocity = Vector3.zero;
            mIsSnapping = true;
        }

        if (mIsSnapping)
        {
            ApplySnapping(targetCenter);
        }
        else
        {
            ApplyPositionDamping(new Vector3(targetCenter.x, mTargetHeight, targetCenter.z));
        }

        SetUpRotation(targetCenter, targetHead);
    }

    private void ApplySnapping(Vector3 targetCenter)
    {
        Vector3 position = transform.position;
        Vector3 offset = position - targetCenter;
        offset.y = 0;

        float currentDistance = offset.magnitude;
        float targetAngle = target.eulerAngles.y;
        float currentAngle = transform.eulerAngles.y;

        currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref mVelocity.x, snapLag);
        currentDistance = Mathf.SmoothDamp(currentDistance, distance, ref mVelocity.z, snapLag);
        
        Vector3 newPosition = targetCenter;
        newPosition += Quaternion.Euler(0, currentAngle, 0) * Vector3.back * currentDistance;
        newPosition.y = Mathf.SmoothDamp(position.y, targetCenter.y + height, ref mVelocity.y, smoothLag, maxSpeed);
        
        newPosition = AdjustLineOfSight(newPosition, targetCenter);
        transform.position = newPosition;

        if (AngleDistance(currentAngle, targetAngle) < 3.0f)
        {
            mIsSnapping = false;
            mVelocity = Vector3.zero;
        }
    }

    private Vector3 AdjustLineOfSight(Vector3 newPosition, Vector3 targetCenter)
    {
        RaycastHit hit;
        if (Physics.Linecast(targetCenter, newPosition, out hit, lineOfSightMask.value))
        {
            mVelocity = Vector3.zero;
            return hit.point;
        }
        return newPosition;
    }

    private void ApplyPositionDamping(Vector3 targetCenter)
    {
        Vector3 position = transform.position;
        Vector3 offset = position - targetCenter;
        offset.y = 0;

        Vector3 newTargetPos = offset.normalized * distance + targetCenter;
        Vector3 newPosition;
        newPosition.x = Mathf.SmoothDamp(position.x, newTargetPos.x, ref mVelocity.x, smoothLag, maxSpeed);
        newPosition.z = Mathf.SmoothDamp(position.z, newTargetPos.z, ref mVelocity.z, smoothLag, maxSpeed);
        newPosition.y = Mathf.SmoothDamp(position.y, targetCenter.y, ref mVelocity.y, smoothLag, maxSpeed);

        newPosition = AdjustLineOfSight(newPosition, targetCenter);
        transform.position = newPosition;
    }

    private float AngleDistance(float a, float b)
    {
        a = Mathf.Repeat(a, 360);
        b = Mathf.Repeat(b, 360);

        return Mathf.Abs(b - a);
    }

    private void SetUpRotation(Vector3 centerPos, Vector3 headPos)
    {
        Vector3 cameraPos = transform.position;
        Vector3 offsetToCenter = centerPos - cameraPos;

        Quaternion yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));

        Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
        transform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);

        Ray centerRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1));
        Ray topRay = camera.ViewportPointToRay(new Vector3(0.5f, clampHeadPositionScreenSpace, 1));

        Vector3 centerRayPos = centerRay.GetPoint(distance);
        Vector3 topRayPos = topRay.GetPoint(distance);

        float centerToTopAngle = Vector3.Angle(centerRay.direction, topRay.direction);
        float heightToAngle = centerToTopAngle / (centerRayPos.y - topRayPos.y);
        float extraLookAngle = heightToAngle * (centerRayPos.y - centerPos.y);
        
        if (extraLookAngle < centerToTopAngle)
        {
            extraLookAngle = 0;
        }
        else
        {
            extraLookAngle = extraLookAngle - centerToTopAngle;
            transform.rotation *= Quaternion.Euler(-extraLookAngle, 0, 0);
        }
    }

    public void SetTarget(Transform playerTransform) 
    {
        target = playerTransform;
        DidChangeTarget();
    }

}
    