using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UMA;

public class CharacterGenerator : MonoBehaviour {

	public UMAGeneratorBase generator;
	public UMAData umaData;
	public SlotLibrary slotLibrary;
	public OverlayLibrary overlayLibrary;
	public RaceLibrary raceLibrary;
	public RuntimeAnimatorController animationController;
	
	public float atlasResolutionScale;


	public GameObject generateUMA(GameObject root, string config){
		if (root == null)
			root = new GameObject ("UMA");
		//newGO.transform.parent = transform;
		var avatar = root.AddComponent<UMADynamicAvatar> ();
		avatar.Initialize ();
		umaData = avatar.umaData;
		avatar.umaGenerator = generator;
		umaData.umaGenerator = generator;
		//var umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
		//var avatar = umaData.gameObject.GetComponent<UMAAvatarBase>();
		if (config != "") {
			var asset = ScriptableObject.CreateInstance<UMATextRecipe> ();
			asset.recipeString = config;
			avatar.Load (asset);
			Destroy (asset);
		}

		return root;
		//umaData = null;
	}
}
