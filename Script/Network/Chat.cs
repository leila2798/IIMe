//-----------------------------------------------------------------------
// <copyright file="Chat.cs" company="Scalify">
//     Copyright (c) 2012 Scalify. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Badumna.Chat;
using Badumna.DataTypes;
using UnityEngine;
using System.Collections.Generic;

// The Chat class demonstrates a simple way to use Badumna proximity
// chat functionality.
///
// To enable proximity chat you need to do these two steps:
// 1. Subscribe your local player to the proximity channel and pass a delegate for
//    receiving proximity chat message, by calling:
//    NetworkFacade.ChatSession.SubscribeToProximityChannel(
//        IReplicableEntity,
//        ChatMessageHandler);
// 2. Then you can send proximity chat message by calling SendMessage(string) on
//    that proximity channel object that you get from the previous step.
public class Chat : MonoBehaviour
{
    // The proximity chat channel.
    private IChatChannel channel;

    // The local player.
    private Player localPlayer;

    // A value indicating whether the local player is subscribed to a proximity channel.
    private bool isSubscribed;


	// The chat window.
	private Rect chatRect;
	
	// The scroll position of the chat window.
	private Vector2 scrollPosition;
	
	// The text of a message to send.
	private string messageText = string.Empty;
	
	// The log of sent messages.
	private List<string> messageHistory = new List<string>();

    // Called by Unity before the first time any Update method is called.
	private void Start()
	{
		this.chatRect = new Rect(15,15 * GameManager.Manager.GuiScale,Screen.width,	200 * GameManager.Manager.GuiScale);
	}
    // Called by Unity when this behaviour becomes disabled.
    private void OnDisable()
    {
        this.Unsubscribe();
        Destroy(this);
    }
	private void OnGUI()
	{
		if (!this.isSubscribed)
		{
			return;
		}
		//GUI.skin = GameManager.Manager.Skin;
		
		GUILayout.BeginArea(this.chatRect);
		GUILayout.BeginHorizontal();
		GUI.SetNextControlName("chat");
		this.messageText = GUILayout.TextField(
			this.messageText,
			new GUILayoutOption[]
			{
			GUILayout.Width(200 * GameManager.Manager.GuiScale),
			GUILayout.Height(20 * GameManager.Manager.GuiScale)
		});
		
		if(GUILayout.Button(
			"Send",
			new GUILayoutOption[]
			{
			GUILayout.Width(98 * GameManager.Manager.GuiScale),
			GUILayout.Height(20 * GameManager.Manager.GuiScale)
		}))
		{
			this.SendMessage();
		}
		
		if (GUILayout.Button(
			"Clear",
			new GUILayoutOption[]
			{
			GUILayout.Width(98 * GameManager.Manager.GuiScale),
			GUILayout.Height(20 * GameManager.Manager.GuiScale)
		}))
		{
			this.messageHistory.Clear();
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);
		foreach(var message in this.messageHistory)
		{
			GUILayout.Label(message);
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		GUI.SetNextControlName("");
		
		if(Event.current.type == EventType.keyDown && Event.current.character == '\n')
		{
			if(GUI.GetNameOfFocusedControl() != "chat")
			{
				GUI.FocusControl("chat");
			}
			else
			{
				this.SendMessage();
				GUI.FocusControl("");
			}
		}
	}
    // Subscribe a local player to proximity chat.
    //  - localPlayer: The local player.
    public void Subscribe(Player localPlayer)
    {
        this.isSubscribed = true;
        this.localPlayer = localPlayer;
        this.channel = Network.Badumna.ChatSession.SubscribeToProximityChannel(
            localPlayer,
            this.HandleChatMessage);
    }

    // Unsubscribe from proximity chat.
    public void Unsubscribe()
    {
        if (this.channel == null)
        {
            return;
        }

        this.isSubscribed = false;
        this.channel.Unsubscribe();
        this.channel = null;
    }

    // Send a chat message.
	private void SendMessage()
	{
		this.channel.SendMessage(
			string.Format("[{0}] says: {1}", this.localPlayer.CharacterName, this.messageText));
		this.HandleChatMessage(
			this.channel,
			null,
			string.Format("You says: {0}", this.messageText));
		this.messageText = string.Empty;
	}

    // Handle incoming chat messages.
    //  - channel: The channel the message was received on.
    //  - userId: The ID of the message sender.
    //  - message: The message text.
	private void HandleChatMessage(IChatChannel channel, BadumnaId userId, string message)
	{
		this.scrollPosition += new Vector2(0, 25);
		this.messageHistory.Add(message);
	}
}
