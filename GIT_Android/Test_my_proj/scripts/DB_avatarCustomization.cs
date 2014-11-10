using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UMA;

public class DB_avatarCustomization : MonoBehaviour{

	#region aux non-static members
	//all the available base avatars
	public Transform[] aux_avatars;
	#endregion

	//assign its own collection to each prefab 
	//then I can get a Dictionary<int, List<Categorie>> -> 
	//for each prefab its collection of categories

	//all the available base avatars!
	public static Transform[] avatars;

	#region Customizations
/*
	//colors may be needed for gui thus public
	public static Color32[] eyesColors = {new Color32(10,10,200,255), new Color32(10,25,20,255), new Color32(253,255,200,255), new Color32(10,255,20,255)};

	public static Color32[] skinColors = {new Color32(253,255,200,255),new Color32(145, 85, 61, 255), new Color32(252,220,234,255),new Color32(217,202,221)};

	public static Color32[] hairColors = {new Color32(242,218,145, 255), new Color32(102,79,60,255),new Color32(10,25,20,255),new Color32(100, 25, 0, 255)};

	public static Color32[] clothesColors = {
				new Color32 (10, 25, 20, 255),
				new Color32 (10, 10, 200, 255),
				new Color32 (245, 10, 20, 255),
				new Color32 (10, 255, 20, 255),
				new Color32 (Color.magenta.r, Color.magenta.g, Color.magenta.b, Color.magenta.a)
		};
*/
	//colors may be needed for gui thus public
	public static Color32[] eyesColors = {new Color32(10,10,200,255), new Color32(10,25,20,255), new Color32(253,255,200,255), new Color32(10,255,20,255)};
	
	public static Color32[] skinColors = {new Color32(253,255,200,255),new Color32(145, 85, 61, 255), new Color32(252,220,234,255),new Color32(217,202,221, 255)};
	
	public static Color32[] hairColors = {new Color32(242,218,145, 255), new Color32(102,79,60,255),new Color32(10,25,20,255),new Color32(100, 25, 0, 255)};
	
	public static Color32[] clothesColors = {
		new Color32 (10, 25, 20, 255),
		new Color32 (10, 10, 200, 255),
		new Color32 (245, 10, 20, 255),
		new Color32 (10, 255, 20, 255)
	};



	/*
	private class Hairstyle
	{
		public Color hairColor{ get; set;}
		//if empty = bold
		public string overlayName{ get; set;}
		//if exists = long hair long part
		public string overlayName2{ get; set;}
		public bool maleStyle;

		public Hairstyle(){
			this.hairColor = Color.clear;
			this.overlayName = string.Empty;
			this.overlayName2 = string.Empty;
			this.maleStyle = true;
		}
	} 
	public Hairstyle[] hStyles;
*/


	/*
	 * slot --------- overlay to change ----Color to change
	 * MALE:
	 * MaleEyes ----- EyeOverlayAdjust --------eyecolor
	 * MaleFace ----- MaleHead01, MaleHead02 --SkinColor
	 * 					MaleHair01, MaleHair02, w/o -- HairColor
	 * 					MailBeard01, 02, 03
	 * MaleTorso -----MaleBody01--- - color = headcolor------ don't need 02 in library!
	 * 					MaleShirt01 -----color
	 * 					MaleUnderware01--color
	 * 					MaleJeans01 -----color
	 * 
	 * FEMALE
	 * FemaleEyes ----- EyeOverlayAdjust --------eyecolor
	 * FemaleFace ----- FemaleHead01 --SkinColor
	 * 					FemailLongHair01, FemaleShortHair01 -- color
	 * 					
	 * FemaleTorso -----MaleBody01--- - color = headcolor------ don't need 02 in library!
	 * 					FemaleShirt01, 02 -----color
	 * 					//MaleUnderware01--color
	 * 				FemaleJeans01 -----color
	 * FemaleLongHair01_Module - FemaleLongHair01_Module - color of the long part only
	 */


	#endregion

	void Start () {
		avatars = aux_avatars;
		//TODO: init categories db!!!
	}

	public static string prefConfig = "AvatarConfig";
	public static string prefAvatar = "AvatarIdx";
	public static string prefUserName = "UserName";

	//all the available categories inc. their sub-values
	//int - idx of avatar, 
	//list of categories includes the category name and its materials
	public Dictionary<int, List<Categorie>> categoriesDB;

	public static string[] catNames = {"Skin", "Eyes", "Hair", "Top", "Pants"};
	

	//change string config of avatar according to user request
	public string ChangeElement(string category, bool next, string currConfig, int avatarIdx)
	{
		List<Categorie> list;
		if (categoriesDB.TryGetValue (avatarIdx, out list)) {
			foreach (Categorie c in list) {
				if (c.name == category)	{
					//get idx of material for this category
					//TODO: coud've used JSON converter, 
					//but may get problems for not using pro
					int idx = currConfig.IndexOf(category);
					string s = ((currConfig.Substring(idx)).Split(';'))[0];
					idx = s.IndexOf(':');
					int mat_idx;
					if (int.TryParse(s.Substring(idx+1), out mat_idx)){
						//set next mat idx
						//CIRCULAR!!!
						if (next){
							mat_idx = (mat_idx==c.materials.Length-1)?0:mat_idx++;
						}
						else{
							mat_idx = (mat_idx==0)?c.materials.Length-1:mat_idx--;
						}
						currConfig = currConfig.Replace(s, category+":" + mat_idx);

						Debug.Log(currConfig);
						return currConfig;
					}
				}	
			}
		}
		return getEmptyConfig();
	}

	//each category has at least one material/mesh for each avatar
	public static string getEmptyConfig()
	{
		string conf = string.Empty;
		for (int i=0; i<catNames.Length; i++) {
			conf+=catNames[i]+":0;";	
		}
		return conf;
	}

}
