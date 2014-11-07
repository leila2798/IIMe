using UnityEngine;
using System.Collections;

using Parse;
using System.Threading.Tasks;

public class ShowSelectedChar : MonoBehaviour {

	#region Variables
	//db-related
	DB_avatarCustomization db;

	//gui
	const float fadeLength = .6f;
	const int typeWidth = 80;
	const int buttonWidth = 20;
	//for modal window
	//private int windowID = 0;
	private bool drawWindow = false;
	private Rect modalWindowRect = new Rect(Screen.width/2 - 150, Screen.height/2 - 50, 300, 100);

	//private vars to handle the avatar 
	GameObject character;
	bool changes = false;

	#endregion

	#region main functions

	void Start () {
		if (!loadAvatar ())
			//to sign up
			Application.LoadLevel("main-styled");
	}

	void Update (){
		string config = PlayerPrefs.GetString(DB_avatarCustomization.prefConfig);
		//TODO: do I need idx? 
		//int idx = PlayerPrefs.GetInt(db.prefAvatar);
		CharacterGenerator.Generate(character, config);
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width - (typeWidth + 2 * buttonWidth + 25), 10, typeWidth + 2 * buttonWidth + 8, 500));

		// Buttons for changing character elements.

		for (int i=0; i<DB_avatarCustomization.catNames.Length; i++) {
			AddCategory(DB_avatarCustomization.catNames[i]);	
		}

		// Buttons for saving and deleting configurations.
		if (GUILayout.Button("Save"))
			if (changes){
				//save avatar config
				saveAvatar();
			} 
			
		if (GUILayout.Button("Reset"))
			if (changes){
				//reset -- load the existing config
				loadAvatar();
			}
		//main menu button
		if (GUILayout.Button("Main Menu")) {
			drawWindow = true;
			//load main-menu screen
			Application.LoadLevel("main-menu");
		}
		
		if (drawWindow){
			//show a dialog whether to save changes, if yes - save
			modalWindowRect = GUI.ModalWindow(0, modalWindowRect, drawModalWindow, "Save Changes");
			
		}
		
		GUILayout.EndArea();

	}

	#endregion

	#region GUI-helper functions

	void drawModalWindow(int windowID) {
		GUI.Label (new Rect (50, 20, 200, 20), "Do you want to save changes?");

		var buttonSaveDown = GUI.Button (new Rect (35, 50, 100, 30), "Save");
		var buttonCancelDown = GUI.Button (new Rect (165, 50, 100, 30), "Cancel");
		var keyDown = Event.current.type == EventType.keyDown &&
			Event.current.character == '\n';
		if (buttonSaveDown || keyDown)
		{
			saveAvatar();
			drawWindow = false;
		}

		else if (buttonCancelDown)
			drawWindow = false;
	}

	//send local config to server
	void saveAvatar(){
		string config = PlayerPrefs.GetString(DB_avatarCustomization.prefConfig);
		//TODO send it to server

		ParseObject avatarStr = new ParseObject("avatarStr");
		avatarStr[DB_avatarCustomization.prefConfig] = config;
		avatarStr[DB_avatarCustomization.prefUserName] = ParseUser.CurrentUser.Username;
		if (avatarStr.SaveAsync().IsCompleted) {
			changes = false;
		}
	}

	bool loadAvatar(){
		try{
			//get string config from pref 
			//TODO: apply immediately or on update?
			string config = PlayerPrefs.GetString(DB_avatarCustomization.prefConfig);
			int idx = PlayerPrefs.GetInt(DB_avatarCustomization.prefAvatar);
			//GameObject root = GameObject.Instantiate(
			// We just have to Instantiate a new GameObject based on the selected character
			character = GameObject.Instantiate(DB_avatarCustomization.avatars[idx].gameObject, new Vector3(0.15f, 0.01f, 0.217f), Quaternion.identity) as GameObject;
			//update avatar acc to config
			CharacterGenerator.Generate(character, config);
			changes = false;
			return true;
		}
		catch (System.Exception){
			return false;
		}

	}

	// Draws buttons for configuring a specific category of items
	void AddCategory(string category)
	{
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("<", GUILayout.Width(buttonWidth)))
			ChangeElement(category, false);
		
		GUILayout.Box(category, GUILayout.Width(typeWidth));
		
		if (GUILayout.Button(">", GUILayout.Width(buttonWidth)))
			ChangeElement(category, true);
		
		GUILayout.EndHorizontal();
	}

	void ChangeElement(string category, bool next)
	{
		int idx = PlayerPrefs.GetInt(DB_avatarCustomization.prefAvatar); 
		string currConfig = PlayerPrefs.GetString(DB_avatarCustomization.prefConfig);
		PlayerPrefs.SetString(DB_avatarCustomization.prefConfig, db.ChangeElement(category, next, currConfig, idx));
		changes = true;
	}

	#endregion
}