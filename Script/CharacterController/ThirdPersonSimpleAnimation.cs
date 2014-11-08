using UnityEngine;

//
// A simple third person controller 
// </summary>
public class ThirdPersonSimpleAnimation : MonoBehaviour
{
    public ThirdPersonController controller;

    public float runSpeedScale = 1.0f;
    public float walkSpeedScale = 1.0f;
    public Transform torso;

    public string animationName = "Idle";

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
            this.animation["Walk"].normalizedSpeed = 1.0f;

            this.animation.CrossFade(this.animationName);
        }
        else
        {

            float currentSpeed = controller.GetSpeed();

			if (currentSpeed > 0.1)
            {
                animation.CrossFade("Walk");
                animationName = "Walk";
            }
            else
            {

                animation.CrossFade("Idle");
                animationName = "Idle";
            }
            animation["Walk"].normalizedSpeed = walkSpeedScale;
        }
    }
    #endregion
}
