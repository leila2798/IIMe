using System;
using UnityEngine;
using Badumna;
using Badumna.SpatialEntities;
using BadumnaId = Badumna.DataTypes.BadumnaId;
using BadumnaVector3 = Badumna.DataTypes.Vector3;

public class Scene : MonoBehaviour
{
    // The entity type for the player character.
    private const uint EntityType = 0;

    // A Badumna network scene.
    private NetworkScene networkScene;

    // The local player character.
    private Player localPlayer;

    // A value indicating if the local player is registered with a Badumna network scene.
    private bool isRegistered;

	public void JoinScene(string sceneName)
	{
		this.CreateLocalPlayer();
		
		if (this.localPlayer != null && this.networkScene == null)
		{
			// Join the normal scene or a mini scene.
			this.networkScene = Network.Badumna.JoinScene(sceneName, this.CreateReplica, this.RemoveReplica);
			this.networkScene.RegisterEntity(this.localPlayer, EntityType, 2f, 20f);
			this.isRegistered = true;
			this.localPlayer.Scene = this.networkScene;
			
			gameObject.GetComponent<Chat>().Subscribe(this.localPlayer);
		}
		else
		{
			Debug.LogWarning("Local player or network scene is null");
		}
	}
    // Create a local player.
	private void CreateLocalPlayer()
	{
		Debug.Log("Creating local player...");
		//////////////////////////////// PLAYER INITIALIZATION ////////////////////////////////////////
		var gameManagerO = gameObject.GetComponent<GameManager>();
		var player = GameObject.Instantiate(GameManager.Manager.PlayerPrefab, new Vector3(3.2f, 8.5f, -0.7f), transform.rotation) as GameObject;
		player = CharacterGenerator.Generate(player, gameManagerO.prefConfig);
		////////////////////////////////////////////////////////////////////////////////////////////////

		Debug.Log("Creating local player... player generated");
		
		
        // create all the components required
        player.AddComponent<ThirdPersonController>();
        player.AddComponent<ThirdPersonSimpleAnimation>();
        var controller = player.GetComponent<CharacterController>();

        // set the center and radius of the character
        controller.radius = 0.4f;
        controller.center = new UnityEngine.Vector3(0, 1.1f, 0);

        this.localPlayer = player.AddComponent<Player>();
		this.localPlayer.CharacterName = GameManager.Manager.PlayerName;
		this.localPlayer.prefAvatar = GameManager.Manager.prefAvatar;
		this.localPlayer.prefConfig = GameManager.Manager.prefConfig;
		
		#if UNITY_IPHONE || UNITY_ANDROID
		var thirdPersonController = player.GetComponent<ThirdPersonController>();
		var joystick = (GameObject)Instantiate(GameManager.Manager.JoystickPrefab);
		thirdPersonController.joystickController = joystick.GetComponent<Joystick>();
		#endif
		Debug.Log("Creating local player... finished");
	}

	// Called by Badumna to create a replica entity.
	//  - scene: The scene the replica entity belongs to.
	//  - entityId: An ID for the entity.
	//  - entityType: An integer indicating the type of entity to create.
	// Returns: A new replica entity.
	private IReplicableEntity CreateReplica(NetworkScene scene, BadumnaId entityId, uint entityType)
	{
		//////////////////////////////// OTHER PREFABS INITIALIZATION ////////////////////////////////////////
		var gameManagerO = gameObject.GetComponent<GameManager>();
		var remotePlayer = GameObject.Instantiate(GameManager.Manager.PlayerPrefab, new Vector3(3.2f, 8.5f, -0.7f), transform.rotation) as GameObject;
		remotePlayer = CharacterGenerator.Generate(remotePlayer, gameManagerO.prefConfig);
		////////////////////////////////////////////////////////////////////////////////////////////////

		if (remotePlayer == null)
		{
			Debug.LogError("Failed to instantiate avatar prefab for entity type " + entityType);
			return null;
		}

		remotePlayer.AddComponent<ThirdPersonSimpleAnimation>();

		var replica = remotePlayer.AddComponent<Player>();
		return replica;
	}

	// Called by Badumna to remove a replica when it moves out of your local
	// entities' interest area.
	//  - scene: The scene the replica is being removed from.
	//  - replica: The replica to removed.
	private void RemoveReplica(NetworkScene scene, IReplicableEntity replica)
	{
		var player = replica as Player;
		if (player != null)
		{
			Destroy(player.gameObject);
		}
	}

	// Leave the Badumna network scene.
	public void LeaveScene()
	{
		if (this.isRegistered && this.localPlayer != null && this.networkScene != null)
		{
			this.networkScene.UnregisterEntity(this.localPlayer);
			this.networkScene.Leave();
			this.networkScene = null;
			this.isRegistered = false;
			
			gameObject.GetComponent<Chat>().Unsubscribe();
			
			Destroy(this.localPlayer.gameObject);
		}
	}
	
}
