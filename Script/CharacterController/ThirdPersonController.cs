//-----------------------------------------------------------------------
// <copyright file="ThirdPersonController.cs" company="Scalify">
//     Copyright (c) 2012 Scalify. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
  
using UnityEngine;

/// <summary>
/// A simple third person controller which is basically it copied from ThirdPersonController.js script.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    public Joystick joystickController;

    public float walkSpeed = 2.0f;
    public float trotSpeed = 4.0f;
    public float runSpeed = 6.0f;

    public float inAirControlAcceleration = 3.0f;

    public float jumpHeight = 0.5f;
    public float extraJumpHeight = 2.5f;

    public float gravity = 20.0f;
    public float capeFlyGravity = 2.0f;
    public float smoothingSpeed = 10.0f;
    public float rotateSpeed = 500.0f;
    public float trotAfterSeconds = 3.0f;

    public bool canJump = true;
    public bool canCapeFly = false;
    public bool canWallJump = false;

    private float jumpRepeatTime = 0.05f;
    private float wallJumpTimeout = 0.15f;
    private float jumpTimeout = 0.15f;
    private float groundedTimeout = 0.25f;

    private float mLockCameraTimer = 0.0f;

    private UnityEngine.Vector3 mMoveDirection = Vector3.zero;
    private float mVerticalSpeed = 0.0f;
    private float moveSpeed = 0.0f;

    private CollisionFlags mCollisionFlags;

    private bool jumping = false;
    private bool jumpingReachedApex = false;

    private bool movingBack = false;
    public bool isMoving = false;

    private float walkTimeStart = 0.0f;
    private float lastJumpButtonTime = -10.0f;
    private float lastJumpTime = -1.0f;

    private Vector3 wallJumpContactNormal;
    private float wallJumpContactNormalHeight;

    private float lastJumpStartHeight = 0.0f;
    private float touchWallJumpTime = -1.0f;

    private Vector3 inAirVelocity = Vector3.zero;
    private float lastGroundedTime = 0.0f;

    public float verticalInput;
    public float horizontalInput;
    public bool jumpButton;

    public bool getUserInput = true;

    public bool isPunching = false;
    public bool isGotHit = false;

    #region MonoBehaviour members
    
    void Start()
    {
        PrepareCamera();
    }

    void Update()
    {
        if (getUserInput)
        {
#if !UNITY_IPHONE && !UNITY_ANDROID
            if (Input.GetButtonDown("Jump"))
                lastJumpButtonTime = Time.time;

            isPunching = Input.GetButton("Fire1");
#endif
        }
        else
        {
            if (jumpButton)
                lastJumpButtonTime = Time.time;
        }

        // get the input from the keyboard
        UpdateSmoothedMovementDirection();

        ApplyGravity();

        if (canWallJump)
        {
            ApplyWallJump();
        }

        ApplyJumping();

        UnityEngine.Vector3 movement = mMoveDirection * moveSpeed + new Vector3(0, mVerticalSpeed, 0);
        movement *= Time.deltaTime;

        CharacterController controller = (CharacterController)GetComponent(typeof(CharacterController));
        mCollisionFlags = controller.Move(movement);

        if (IsGrounded() && mMoveDirection != UnityEngine.Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(mMoveDirection);
        }
        else
        {
            UnityEngine.Vector3 xzMove = movement;
            xzMove.y = 0;
            if (xzMove.magnitude > 0.001)
            {
                transform.rotation = Quaternion.LookRotation(xzMove);
            }
        }

        if (IsGrounded())
        {
            lastGroundedTime = Time.time;
            inAirVelocity = Vector3.zero;
            if (jumping)
            {
                jumping = false;
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.moveDirection.y > 0.01)
        {
            return;
        }

        wallJumpContactNormal = hit.normal;
    }
    #endregion

    private void UpdateSmoothedMovementDirection()
    {
        Transform cameraTransform = Camera.main.transform;
        bool grounded = IsGrounded();

        UnityEngine.Vector3 forward = cameraTransform.TransformDirection(UnityEngine.Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;

        UnityEngine.Vector3 right = new Vector3(forward.z, 0f, -forward.x);

        if (getUserInput)
        {
#if UNITY_IPHONE || UNITY_ANDROID
            verticalInput = joystickController.position.y;
            horizontalInput = joystickController.position.x;
#else
            verticalInput = Input.GetAxisRaw("Vertical");
            horizontalInput = Input.GetAxisRaw("Horizontal");
#endif
        }

        bool wasMoving = isMoving;
        isMoving = Mathf.Abs(horizontalInput) > 0.1 || Mathf.Abs(verticalInput) > 0.1;

        UnityEngine.Vector3 targetDirection = horizontalInput * right + verticalInput * forward;

        if (grounded)
        {
            mLockCameraTimer += Time.deltaTime;
            if (isMoving != wasMoving)
            {
                mLockCameraTimer = 0.0f;
            }

            if (targetDirection != UnityEngine.Vector3.zero)
            {
                if (moveSpeed < walkSpeed * 0.9 && grounded)
                {
                    mMoveDirection = targetDirection.normalized;
                }
                else
                {
                    mMoveDirection = Vector3.RotateTowards(mMoveDirection, targetDirection,
                                        rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
                    mMoveDirection = mMoveDirection.normalized;
                }

            }
            float currentSmooth = smoothingSpeed * Time.deltaTime;

            float targetSpeed = Mathf.Min(targetDirection.magnitude, 1);

            if (Input.GetButton("Fire3"))
            {
                targetSpeed *= runSpeed;
            }
            else if (Time.time - trotAfterSeconds > walkTimeStart)
            {
                targetSpeed *= trotSpeed;
            }
            else
            {
                targetSpeed *= walkSpeed;
            }

            moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, currentSmooth);

            if (moveSpeed < walkSpeed * 0.3)
                walkTimeStart = Time.time;
        }
        else
        {
            if (jumping)
                mLockCameraTimer = 0.0f;

            if (isMoving)
                inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
        }
    }

    private void ApplyWallJump()
    {
        if (!jumping)
            return;

        if (mCollisionFlags == CollisionFlags.CollidedSides && touchWallJumpTime < 0)
        {
            touchWallJumpTime = Time.time;
        }

        bool mayJump = lastJumpButtonTime > touchWallJumpTime - wallJumpTimeout && lastJumpButtonTime < touchWallJumpTime + wallJumpTimeout;
        if (!mayJump)
            return;

        if (lastJumpTime + jumpRepeatTime > Time.time)
            return;

        wallJumpContactNormal.y = 0;
        if (wallJumpContactNormal != Vector3.zero)
        {
            mMoveDirection = wallJumpContactNormal.normalized;
            moveSpeed = Mathf.Clamp(moveSpeed * 1.5f, trotSpeed, runSpeed);
        }
        else
        {
            moveSpeed = 0;
        }

        mVerticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
        DidJump();
    }

    private void ApplyJumping()
    {
        if (lastJumpTime + jumpRepeatTime > Time.time)
        {
            return;
        }

        if (IsGrounded())
        {
            if (canJump && Time.time < lastJumpButtonTime + jumpTimeout)
            {
                mVerticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
                DidJump();
            }
        }
    }

    private void ApplyGravity()
    {
        if (getUserInput)
        {
            jumpButton = Input.GetButton("Jump");
        }

        bool capeFly = canCapeFly && jumpButton && mVerticalSpeed <= 0.0f && jumping;

        if (jumping && !jumpingReachedApex && mVerticalSpeed <= 0.0)
        {
            jumpingReachedApex = true;
        }

        bool extraPowerJump = IsJumping() && mVerticalSpeed > 0.0 && jumpButton && transform.position.y < lastJumpStartHeight + extraJumpHeight;

        if (capeFly)
        {
            mVerticalSpeed -= capeFlyGravity * Time.deltaTime;
        }
        else if (extraPowerJump)
            return;
        else if (IsGrounded())
        {
            mVerticalSpeed = -gravity * 0.2f;
        }
        else
        {
            mVerticalSpeed -= gravity * Time.deltaTime;
        }
    }

    private float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        return Mathf.Sqrt(2 * targetJumpHeight * gravity);
    }

    private void DidJump()
    {
        jumping = true;
        jumpingReachedApex = false;
        lastJumpTime = Time.time;
        lastJumpStartHeight = transform.position.y;
        touchWallJumpTime = -1;
        lastJumpButtonTime = -10;
    }

    private bool IsGrounded()
    {
        return (mCollisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

    public float GetSpeed()
    {
        return moveSpeed;
    }

    public bool IsJumping()
    {
        return jumping;
    }

    private void SuperJump(float height, Vector3 jumpVelocity)
    {
        mVerticalSpeed = CalculateJumpVerticalSpeed(height);
        inAirVelocity = jumpVelocity;

        mCollisionFlags = CollisionFlags.None;
        DidJump();
    }

    public Vector3 GetDirection()
    {
        return mMoveDirection;
    }

    public bool IsMovingBackwards()
    {
        return movingBack;
    }

    public float GetLockCameraTimer()
    {
        return mLockCameraTimer;
    }

    public float GetLean()
    {
        return 0.0f;
    }

    public bool HasJumpReachedApex()
    {
        return jumpingReachedApex;
    }

    public bool IsGroundedWithTimeout()
    {
        return lastGroundedTime + groundedTimeout > Time.time;
    }

    public bool IsCapeFlying()
    {
        if (getUserInput)
        {
            jumpButton = Input.GetButton("Jump");
        }
        return canCapeFly && mVerticalSpeed <= 0.0 && jumpButton && jumping;
    }

    private void PrepareCamera()
    {
        var playerTransform = gameObject.GetComponent<Transform>();
        if (playerTransform == null) return;

        var sfc = GameObject.Find("Main Camera").GetComponent<SpringFollowCamera>();
        if (sfc == null) return;

        sfc.SetTarget(playerTransform.transform);
    }
}
