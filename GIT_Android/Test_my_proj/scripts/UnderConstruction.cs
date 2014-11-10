using UnityEngine;
using System.Collections;

public class UnderConstruction : MonoBehaviour {

	public Texture2D back_button;

	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2-100, 200, 200), back_button)) {
			Application.LoadLevel("main-menu");
		}
	}
}
