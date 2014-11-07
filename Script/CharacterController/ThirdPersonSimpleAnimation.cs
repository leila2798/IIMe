//-----------------------------------------------------------------------
// <copyright file="ThirdPersonSimpleAnimation.cs" company="Scalify">
//     Copyright (c) 2012 Scalify. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
  
using UnityEngine;

/// <summary>
/// A simple third person controller which is basically it copied from ThirdPersonController.js scrip
/// </summary>
public class ThirdPersonSimpleAnimation : MonoBehaviour
{
    public ThirdPersonController controller;

    public float runSpeedScale = 1.0f;
    public float walkSpeedScale = 1.0f;
    public Transform torso;

    public string animationName;

    #region MonoBehaviour members
    
    void Awake()
    {
        animation.wrapMode = WrapMode.Loop;

        this.animation.Stop();
        this.animation.Play("Idle");
        this.animationName = "Idle";
    }

    void Start()
    {
        controller = GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        if (controller == null)
        {
            // sync replica animation.
            this.animation["Run"].normalizedSpeed = 1.0f;
            this.animation["Walk"].normalizedSpeed = 1.0f;

            this.animation.CrossFade(this.animationName);
        }
        else
        {

            float currentSpeed = controller.GetSpeed();

            if (currentSpeed > controller.walkSpeed)
            {
                animation.CrossFade("Run");
                animationName = "Run";
            }
            else if (currentSpeed > 0.1)
            {
                animation.CrossFade("Walk");
                animationName = "Walk";
            }
            else
            {
                animation.CrossFade("Idle");
                animationName = "Idle";
            }

            animation["Run"].normalizedSpeed = runSpeedScale;
            animation["Walk"].normalizedSpeed = walkSpeedScale;

            if (controller.IsJumping())
            {
				animation.CrossFade("Run");
				animationName = "Run";
            }
        }
		this.animation.Play(this.animationName);
    }
    #endregion
}
