//-----------------------------------------------------------------------
// <copyright file="Network.cs" company="Scalify">
//     Copyright (c) 2012 Scalify. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Badumna;
using Badumna.Chat;
using Badumna.Security;
using Badumna.SpatialEntities;
using UnityEngine;
using BadumnaId = Badumna.DataTypes.BadumnaId;
using BadumnaVector3 = Badumna.DataTypes.Vector3;

// This class provides a template network script for a Badumna enabled game.
// It takes care of initializing the Badumna network, and has template code
// for creating a local player character and joining a Badumna network scene.
///
// The steps required to set up basic replication are listed below.
// To use this script in your game, you must fill in the code for creating the
// Unity game object for the player character and creating and removing Unity
// game objects for replicas.
///
// Summary:
// 1. Call NetworkFacade.BeginCreate and pass your cloud application identifier to
//    start asynchronously initializing Badumna.
// 2. Call NetworkFacade.EndCreate when the IsCompleted property on the AsyncResult
//    object returned by NetworkFacade.BeginCreate has been set to true. This will
//    return the network facade for accessing Badumna functionailty.
// 3. Login to Badumna by calling INetworkFacade.Login(characterName, xmlKeyPair).
// 4. Register Entity Details and register any custom types for replicable properties
//    before joining a Badumna scene.
// 5. Join into a Badumna scene and register the local player with the scene.
// 6. Regularly update Badumna by calling INetworkFacade.ProcessNetworkState()
//    in the game's update loop (e.g. in FixedUpdate()).
// 7. Call INetworkFacade.Shutdown() to shutdown Badumna network.
public class Network : MonoBehaviour
{
	// Scene instance.
	private Scene scene;

    // The Badumna network.
    private static INetworkFacade network;

    // For displaying initialization status on screen.
    private GUIText status;


    // Result of initializing Badumna.
    IAsyncResult initializationResult;

    // Gets the Badumna network.
    public static INetworkFacade Badumna
    {
        get { return Network.network; }
    }

    // Called by Unity when this script is loaded.
    private void Awake()
    {
        var go = new GameObject();
		go.transform.position = new Vector3(0.8f, 5.0f, -9.0f);//(0.0f, 18.0f, 9);
        this.status = go.AddComponent<GUIText>();
        this.status.anchor = TextAnchor.LowerLeft;
        this.status.font.material.color = Color.black;
        this.status.text = "Initializing Badumna...";

        this.initializationResult = NetworkFacade.BeginCreate(GameManager.Manager.ApplicationIdentifier,
            null);
    }

	private IEnumerator Start()
	{
		while (!this.initializationResult.IsCompleted)
		{
			yield return null;
		}
		
		if (Network.network == null)
		{
			try
			{
				Network.network = NetworkFacade.EndCreate(this.initializationResult);
				Network.network.AddressChangedEvent += GameManager.Manager.AddressChangedEventHandler;
				Destroy(this.status.gameObject);
			}
			catch (Exception ex)
			{
				var errorMessage = "Badumna initialization failed: " + ex.Message;
				this.status.text = errorMessage;
				Debug.LogError(errorMessage);
				yield break;
			}
		}

		if (GameManager.Manager.PlayerName == null || GameManager.Manager.PlayerName.Length == 0)
		{
			GameManager.Manager.PlayerName = PlayerPrefs.GetString(DB_avatarCustomization.prefUserName);
		}
		
		// we must login before attempting to use the network
		var loginResult = Badumna.Login(GameManager.Manager.PlayerName, GameManager.Manager.KeyPairXml);
		
		if (!loginResult)
		{
			Debug.LogError("Failed to login");
			yield break;
		}
		
		// Register Entity Details.
		Network.network.RegisterEntityDetails(20, 6);
		
		// Register any custom types for replicable properties here.
		//// Network.network.TypeRegistry.RegisterValueType( ... );
		this.scene = gameObject.AddComponent<Scene>();
		this.scene.JoinScene("MyRoom");
	}

    // Called by Unity every fixed frame.
    private void FixedUpdate()
    {
        if (Network.network == null || !Network.network.IsLoggedIn)
        {
            return;
        }

        Network.network.ProcessNetworkState();
    }

	private void OnDisable()
	{
		if (!this.initializationResult.IsCompleted)
		{
			try
			{
				Network.network = NetworkFacade.EndCreate(this.initializationResult);
			}
			catch { }
		}
		
		this.scene.LeaveScene();
		
		if(this.status != null)
		{
			Destroy(this.status.gameObject);
		}
		
		if (Network.network != null)
		{
			Network.network.Shutdown();
			Network.network = null;
		}
	}
}
