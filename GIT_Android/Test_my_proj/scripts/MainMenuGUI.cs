using UnityEngine;
using System.Collections;
using Parse;

public class MainMenuGUI : MonoBehaviour {
	
	#region variables declarations
	
	public Texture2D GUIBackground;
	
	//const int buttonWidth = 160;
	//const int buttonHeight = 35;
	
	#endregion //vars
	
	#region main functions

	void OnGUI()
	{
		GUI.BeginGroup (new Rect ((Screen.width/2) - 100, (Screen.height/2) - 125, 200, 250));
		
		GUI.Box (new Rect (0, 0, 200, 200), GUIBackground);
		
		GUILayout.BeginArea (new Rect (20, 20, 160, 250));
		
		if (GUILayout.Button("Explore World")) {
			//TODO: change from the under-construction to the real scene
			//load map screen
			Application.LoadLevel("under-construction");
		} 
		
		if (GUILayout.Button("Avatar Customization")) {
			//load avatar customization screen
			Application.LoadLevel("main-styled-selected-char");
		}
		
		if (GUILayout.Button("Events")) {
			//TODO: change from the under-construction to the real scene
			//load events screen
			Application.LoadLevel("under-construction");
		}
		
		if (GUILayout.Button("Account Settings")) {
			//TODO: change from the under-construction to the real scene
			//load account settings screen
			Application.LoadLevel("main-styled");
		} 
		
		if (GUILayout.Button("Tutorials")) {
			//TODO: change from the under-construction to the real scene
			//load tutorials screen
			Application.LoadLevel("under-construction");
		}
		
		if (GUILayout.Button("Log out")) {
			//TODO: change from the under-construction to the real scene
			ParseUser.LogOut();
			//load login screen
			//Application.LoadLevel("login");
		}
		
		GUILayout.EndArea ();
		GUI.EndGroup ();
	}
	
	#endregion //main func
}
