//-----------------------------------------------------------------------
// <copyright file="GameManager.cs" company="Scalify">
//     Copyright (c) 2012 Scalify Pty Ltd. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using UnityEngine;

// The game manager.
//
// This script provides a template game manager for use in a Badumna-enabled Unity3D game.
// It is intended that developers can add this script to a game manager object, and then
// customise it for their own game.
///
// It provides a place to add customizable fields that can be edited in the Unity editor,
// and takes care of loading and destroying the Network script.
// Since Badumna must maintain active connections to other peers, it will trigger
// re-initialization of Badumna when the Unity application stops/starts running.
public class GameManager : MonoBehaviour
{
    // The application Identifier should be set to your BadumnaCloud ID.
    public string ApplicationIdentifier;

    // Player name
    public string PlayerName;
	// Gets or sets number of prefab for the player.
	public int prefAvatar;	
	// Gets or sets prefab configuration string for the player.
	public string prefConfig;

    // Factor to scale GUI by.
    internal int GuiScale = 1;

    // A key pair generated automatically and used by Badumna for encryption.
    private string keyPairXml;

    // A file to cache a pre-generated key pair.
    private string keyFileName = "key.bin";

    // The Badumna network script.
    private Network network;

    // A value indicating whether Badumna is initialized.
    private bool isInitialized;

	// Prefab for player.
	public GameObject PlayerPrefab;
	
	// Prefab for on-screen joystick for mobile platforms.
	public GameObject JoystickPrefab;

	// Proximity chat.
	private Chat proximityChat;

    // Gets the GameManager.
    // This provides global access to the game manager for convenience.
    public static GameManager Manager
    {
        get;
        private set;
    }

    // Gets the key pair.
    public string KeyPairXml
    {
        get { return this.keyPairXml; }
    }

    // Handle IP address changes.
    public void AddressChangedEventHandler()
    {
        // If the local IP address has changed Badumna will need to be restarted.
        Debug.Log("Address changed dectected.");
        this.Shutdown();
        this.Initialize();
    }

    // Called by Unity when this script loads.
    private void Awake()
    {
        if (GameManager.Manager != null)
        {
            return;
        }

        GameManager.Manager = this;
        this.GenerateKeyPair();
    }

    // Called by Unity before the first time any Update method is called.
    private void Start()
    {
		// Initialize Badumna.
        this.Initialize();
	}

    // Called by Unity when the player is paused or unpaused.
    //  - pause: A value indicating if pausing or unpausing.
    private void OnApplicationPause(bool pause)
    {
        // Since Badumna needs to be updated regularly to maintain its connection
        // to the P2P network, it needs to be shutdown when the player is paused
        // and re-initialized when the player is un-paused.
        if (pause)
        {
            this.Shutdown();
        }
        else
        {
            if (Manager != null)
            {
                this.Initialize();
            }
        }
    }

    // Called by Unity when the application is closed.
    private void OnApplicationQuit()
    {
        this.Shutdown();
    }

    // Load the network script.
	private void Initialize()
	{
		if (this.isInitialized)
		{
			return;
		}
		
		if (Application.platform == RuntimePlatform.IPhonePlayer
		    || Application.platform == RuntimePlatform.Android)
		{
			this.GuiScale = 2;
		}

		this.proximityChat = gameObject.AddComponent<Chat>();
		this.network = gameObject.AddComponent<Network>();

		// Initialize
		this.prefConfig = PlayerPrefs.GetString(DB_avatarCustomization.prefConfig);
		this.prefAvatar = PlayerPrefs.GetInt(DB_avatarCustomization.prefAvatar);
		GameManager.Manager.PlayerName = PlayerPrefs.GetString(DB_avatarCustomization.prefUserName);
		GameManager.Manager.PlayerPrefab = DB_avatarCustomization.avatars[prefAvatar].gameObject;

		this.isInitialized = true;
	}
	
	// Unload the network script.
	private void Shutdown()
	{
		if (this.proximityChat != null)
			Destroy(this.proximityChat);
		if (this.network != null)
			Destroy(this.network);
		
		this.isInitialized = false;
	}

    // Since generating a key pair can be slow on some devices, this method attempts to load
    // a cached key pair from a file, and only generates (and caches) a key pair if none is
    // found.
    private void GenerateKeyPair()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            var saveFilePath = Path.Combine(Application.persistentDataPath, this.keyFileName);
            if (File.Exists(saveFilePath))
            {
                this.keyPairXml = File.ReadAllText(saveFilePath);
            }
            else
            {
                this.keyPairXml = Badumna.Security.UnverifiedIdentityProvider.GenerateKeyPair();
                File.WriteAllText(saveFilePath, this.keyPairXml);
            }
        }
        else
        {
            this.keyPairXml = Badumna.Security.UnverifiedIdentityProvider.GenerateKeyPair();
        }
    }
}
