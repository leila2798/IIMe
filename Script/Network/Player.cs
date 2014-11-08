using System;
using System.IO;
using Badumna;
using Badumna.DataTypes;
using Badumna.SpatialEntities;
using UnityEngine;
using Vector3 = Badumna.DataTypes.Vector3;

// Class for representing a player's position and any additional properties.
//
// Defines all replicable properties and RPC methods in this class and 
// tags them with [Replicable] attribute.
//
// Replicable properties defined here will typicallly wrap a property on the
// Unity game object representing the player. For example, the existing Position
// and Orientation properties wrap the Unity game object's transform's position
// and rotation.
public class Player : MonoBehaviour, IReplicableEntity
{

	// Animation controller.
	private ThirdPersonSimpleAnimation animationController;
	
	// The scene.
	public Badumna.SpatialEntities.NetworkScene Scene;

	
	// Gets or sets the name for the player.
	public string CharacterName { get; set; }

	// Gets or sets number of prefab for the player.
	public int prefAvatar { get; set; }

	// Gets or sets prefab configuration string for the player.
	public string prefConfig { get; set; }


    // Gets or sets the position of the entity.
    // This property is automatically replicated by Badumna.
    [Smoothing(Interpolation = 200, Extrapolation = 0)]
    public Vector3 Position
    {
        get
        {
            var position = this.gameObject.transform.position;
            return new Vector3(position.x, position.y, position.z);
        }

        set
        {
            this.gameObject.transform.position = new UnityEngine.Vector3(value.X, value.Y, value.Z);
        }
    }

    // Gets or sets the direction the player is facing.
    [Replicable]
    public float Orientation
    {
        get
        {
            return this.gameObject.transform.rotation.eulerAngles.y;
        }

        set
        {
            this.gameObject.transform.rotation = Quaternion.AngleAxis(value, UnityEngine.Vector3.up);
        }
    }

	// Grab the ThirdPersonSimpleAnimation on Awake.
	private void Awake()
	{
		this.animationController = this.GetComponent<ThirdPersonSimpleAnimation>();
	}
	private void Update()
	{
		// only apply if this is a local player.
		if(this.GetComponent<ThirdPersonController>() != null)
		{
			// hnadle animation state
		}
	}
	    // Gets or sets the name of the player animation currently playing.
    [Replicable]
    public string AnimationName
    {
        get
        {
            return this.animationController.animationName;
        }

        set
        {
            this.animationController.animationName = value;
        }
    }
}
