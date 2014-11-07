using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public static string prefConfig = "AvatarConfig";
	public static string prefAvatar = "AvatarIdx";
	public static string prefUserName = "UserName";

	//all the available categories inc. their sub-values
	//int - idx of avatar, 
	//list of categories includes the category name and its materials
	public Dictionary<int, List<Categorie>> categoriesDB;

	public static string[] catNames = {"Eyes", "Hair", "Top", "Pants", "Shoes"};
	
	void Start () {
		avatars = aux_avatars;
		//TODO: init categories db!!!
	}

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
