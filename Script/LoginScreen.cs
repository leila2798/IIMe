using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System.Threading.Tasks;

// Parse:
// AppID: cK3CRQFP6HkphpER55IoZvg9miKGeCTFksbc0h4v
// NET Key: LgnG9OuQVhZzsMuMjO5eEN06yepXd2npYjQVeaeE

// The LoginScreen class is used to display the Login GUI window, and trigger login.
public class LoginScreen : MonoBehaviour
{

	public static string parseClass = "IIMe";

	public Texture2D GUIBackground;

    // Gets the user name chosen by the player.
    public string UserName { get; private set; }
	
	// Gets the password chosen by the player.
    public string PasswordToEdit { get; private set; }

    // Gets a value indicating whether the use have login.
    public bool HaveLogin { get; private set; }

	string tmpS = "";
	int tmpI = 0;
	static bool show = true;
		
    // Called by Unity before any Update method is called for the first time.
    private void Start()
    {
        this.UserName = "";
		this.PasswordToEdit = "";
    }

    // Called by Unity for rendering and GUI events.
    private void OnGUI()
    {
		/*
        if (Network.Badumna == null)
        {
            return;
        }
		*/
		if (show) 
		{
			GUI.BeginGroup (new Rect ((Screen.width/2) - 100, (Screen.height/2) - 125, 200, 250));

			GUI.Box (new Rect (0, 0, 200, 200), GUIBackground);

			GUILayout.BeginArea (new Rect (20, 20, 160, 250));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Username", GUILayout.Width(63));
	        this.UserName = GUILayout.TextField(this.UserName, 11, GUILayout.Width(92));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Password", GUILayout.Width(63));
			this.PasswordToEdit = GUILayout.PasswordField(this.PasswordToEdit, "*"[0], 11, GUILayout.Width(92));
			GUILayout.EndHorizontal();
	        var btnLogin = GUILayout.Button("Login");
			GUILayout.Space(70);
			var btnSignup = GUILayout.Button("Sign up");
			if (this.HaveLogin) 
			{
				PlayerPrefs.SetString(DB_avatarCustomization.prefConfig, tmpS);
				PlayerPrefs.SetInt(DB_avatarCustomization.prefAvatar, tmpI);
				PlayerPrefs.SetString(DB_avatarCustomization.prefUserName, this.UserName);
				PlayerPrefs.SetString("password", this.PasswordToEdit);
				show = false;
				Application.LoadLevel("main-menu");
			}
			if (btnLogin)        
			{
            	this.DoLogin();
        	}
			if (btnSignup) 
			{
				Application.LoadLevel("main-styled");
			}
			GUILayout.EndArea ();
			GUI.EndGroup ();
		}
	}

    // Trigger login.
    private void DoLogin()
    {
		var query = ParseObject.GetQuery(parseClass).WhereEqualTo(DB_avatarCustomization.prefUserName, this.UserName);
		query.WhereEqualTo("password", this.PasswordToEdit);
		query.FindAsync().ContinueWith(t =>
		{
			if (t.IsFaulted || t.IsCanceled)
			{
				Debug.Log("Login failed! "+ (ParseException)t.Exception.InnerExceptions[0]);
			}
			else
			{
				IEnumerator<ParseObject> enumerator = t.Result.GetEnumerator();
				enumerator.MoveNext();
				ParseObject obj = (ParseObject) enumerator.Current;
				if (obj != null)
				{
					Debug.Log("ID of user: " + obj.ObjectId);
					tmpI = obj.Get<int>(DB_avatarCustomization.prefAvatar);
					tmpS = obj.Get<string>(DB_avatarCustomization.prefConfig);
					this.HaveLogin = true;
				}
				else
				{
					Debug.Log("User does not exist.");
					this.UserName = "";
					this.PasswordToEdit = "";
				}
			}
		});
	}	
}
