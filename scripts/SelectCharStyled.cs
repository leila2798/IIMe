using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Parse;
using System.Threading.Tasks;


/**
 * Class to do a character selection on Unity3D. Styled Version
 * 
 * @author Jefferson Henrique
 * */
public class SelectCharStyled : MonoBehaviour {
	
	// The left marker out of visible scence
	public Transform markerLeft2;
	// The left marker of visible scence
	public Transform markerLeft;
	// The middle marker of visible scence
	public Transform markerMiddle;
	// The right marker of visible scence
	public Transform markerRight;
	// The right marker out of visible scence
	public Transform markerRight2;
	
	// The characters prefabs to pick
	//public Transform[] charsPrefabs;
	// An aux array to be used on ShowSelectedChar.cs
	//public static Transform[] charsPrefabsAux;

	//public textures for buttons
	public Texture2D iconNext;
	public Texture2D iconPrev;

	#region sign up screen 
	// A value indicating whether to show the login window.
	private bool hideSignUpWindow;
	
	// The position of the window.
	private Rect windowRect;
	
	// Gets the user name chosen by the player.
	public string UserName { get; private set; }
	
	// Gets a value indicating whether the use have login.
	public bool haveSignUp { get; private set; }

	public string password { get; private set; }
	public string repeatPassword { get; private set; }
	public string email { get; private set; }

	#endregion


	//GUI skin
	//public GUISkin mainGUISkin;

	// The game objects created to be showed on screen
	private GameObject[] chars;
	
	// The index of the current character
	public static int currentChar = 0;
	
	void Start() {

		//charsPrefabsAux = DB_avatarCustomization.avatars;
		// We initialize the chars array
		chars = new GameObject[DB_avatarCustomization.avatars.Length];
		
		// We create game objects based on characters prefabs
		int index = 0;
		Debug.Log ("length "+ DB_avatarCustomization.avatars.Length);
		foreach (Transform t in DB_avatarCustomization.avatars) {
			chars[index++] = GameObject.Instantiate(t.gameObject, markerRight2.position, Quaternion.identity) as GameObject;
		}

		this.UserName = string.Empty;
		this.email = string.Empty;
		this.repeatPassword = string.Empty;
		this.password = string.Empty;
		this.hideSignUpWindow = true;
		
		this.windowRect = new Rect(
			((Screen.width / 2) - 100),
			((Screen.height / 2) - 100),
			250,
			200);

	}
	
	void OnGUI() {

		//GUI.skin = mainGUISkin;

		//TODO: add BADUMNA
		if ((!this.hideSignUpWindow))// && (Network.Badumna != null))
		{
			this.windowRect = GUI.Window(0, this.windowRect, this.SignUpWindow, "Sign Up");
		}

		else
		{
			//main menu button
			//if (GUI.Button(new Rect(20, 20, 67, 67), "", "mainMenuButton")) {
				//Application.LoadLevel("main-menu-screen");
			//}

			// Here we create a button to choose a prev char
			if (GUI.Button(new Rect(10, (Screen.height - 50) / 2, 70, 30), iconPrev)) {
				currentChar--;
				
				if (currentChar < 0) {
					currentChar = 0;
				}
			}
			
			// Now we create a button to choose a next char
			if (GUI.Button(new Rect(Screen.width - 100 - 10, (Screen.height - 50) / 2, 70, 30), iconNext)) {
				currentChar++;
				
				if (currentChar >= chars.Length) {
					currentChar = chars.Length - 1;
				}
			}
			
			// Shows a label with the name of the selected character
			/*GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GameObject selectedChar = chars[currentChar];
			string labelChar = selectedChar.name;
			GUI.Label(new Rect((Screen.width - 200) / 2, 20, 200, 50), labelChar);*/
			
			if (GUI.Button(new Rect((Screen.width - 100) / 2, Screen.height - 50, 70, 30), "Choose")) {
				this.hideSignUpWindow = false;

			}
			
			// The index of the middle character
			int middleIndex = currentChar;	
			// The index of the left character
			int leftIndex = currentChar - 1;
			// The index of the right character
			int rightIndex = currentChar + 1;
			
			// For each character we set the position based on the current index
			for (int index = 0; index < chars.Length; index++) {
				Transform transf = chars[index].transform;
				
				// If the index is less than left index, the character will dissapear in the left side
				if (index < leftIndex) {
					transf.position = Vector3.Lerp(transf.position, markerLeft2.position, Time.deltaTime);
					
				// If the index is less than right index, the character will dissapear in the right side
				} else if (index > rightIndex) {
					transf.position = Vector3.Lerp(transf.position, markerRight2.position, Time.deltaTime);
					
				// If the index is equals to left index, the character will move to the left visible marker
				} else if (index == leftIndex) {
					transf.position = Vector3.Lerp(transf.position, markerLeft.position, Time.deltaTime);
					
				// If the index is equals to middle index, the character will move to the middle visible marker
				} else if (index == middleIndex) {
					transf.position = Vector3.Lerp(transf.position, markerMiddle.position, Time.deltaTime);
					
				// If the index is equals to right index, the character will move to the right visible marker
				} else if (index == rightIndex) {
					transf.position = Vector3.Lerp(transf.position, markerRight.position, Time.deltaTime);
				}
			}
		}
	}

	// Function for creating Login window.
	//  - id: ID not used (but required for passing to Gui.Window).
	private void SignUpWindow(int id)
	{
		GUILayoutOption[] options =
		{
			//2 is for android. otherwise 1
			GUILayout.Width(100),// * 2),
			GUILayout.Height(20)// * 2)
		};
		GUILayout.BeginHorizontal();
		GUILayout.Label("All fields are mandatory");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("email");
		this.email = GUILayout.TextField(this.email, options);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("UserName");
		this.UserName = GUILayout.TextField(this.UserName, options);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUI.SetNextControlName("password");
		GUILayout.Label("Password");
		this.password = GUILayout.PasswordField(this.password, "*"[0], 25, options);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Repeat Password");
		GUI.SetNextControlName("repeatPassword");
		this.repeatPassword = GUILayout.PasswordField(this.repeatPassword, "*"[0], 25, options);
		GUILayout.EndHorizontal();
		var buttonDown = GUILayout.Button("Sign Up");
		var keyDown = Event.current.type == EventType.keyDown &&
			Event.current.character == '\n';
		if (buttonDown || keyDown)
		{
			this.DoSignUp();
		}
	}
	
	// Trigger login.
	private void DoSignUp()
	{
		if (this.UserName==string.Empty||this.email == string.Empty) {
			return;		
		}

		if (this.password!=string.Empty && this.password == this.repeatPassword) {
			this.hideSignUpWindow = true;
			this.haveSignUp = true;
			//make config to save on server and locally
			string conf = DB_avatarCustomization.getEmptyConfig();
			int avatarIdx = currentChar;

			//save to server
			var user = new ParseUser()
			{
				Username = UserName,
				Password = password,
				Email = email
			};
			user[DB_avatarCustomization.prefAvatar] = avatarIdx.ToString();
			user [DB_avatarCustomization.prefConfig] = conf;
			Task signUpTask = user.SignUpAsync();

			if (!signUpTask.IsCompleted){
				//TODO: display msg? and load login screen
				//Application.LoadLevel("login");
				if (signUpTask.IsCanceled)
					Debug.Log("canceled");
				if (signUpTask.IsFaulted)
					Debug.Log("faulted");
				else
					Debug.Log("problema");
			}

			else{
			//save config:
			//PlayerPrefs.SetString(DB_avatarCustomization.prefUserName, UserName);
			PlayerPrefs.SetString(DB_avatarCustomization.prefConfig, conf);
			PlayerPrefs.SetInt(DB_avatarCustomization.prefAvatar, avatarIdx);

			Application.LoadLevel("main-menu");
			}
		}

		//display worning
		GUI.FocusControl ("repeatPassword");
		this.repeatPassword = string.Empty;

		GUI.FocusControl ("password");

	}

}