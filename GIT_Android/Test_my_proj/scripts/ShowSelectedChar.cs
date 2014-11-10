using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UMA;
using Parse;
using System.Threading.Tasks;
using LitJson;

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
	//GameObject character;
	bool changes = false;

	#endregion

	//category - cur idx
	Dictionary<string, int> categoriesIdxs;

	#region UMA
	public UMAData umaData;
	public UMADynamicAvatar umaDynamicAvatar;
	public UMADnaHumanoid umaDna;
	
	public SliderControl[] sliderControlList;
	
	public SlotLibrary mySlotLibrary;
	public OverlayLibrary myOverlayLibrary;
	public bool editing = false;

	#region sliders val

	float height;//0,0
	float upperMuscle; //1,0);
	float lowerMuscle;//0,1);
	float upperWeight;//1,1);
	float lowerWeight;//0,2);	
	float legsSize;//1,2);
	float gluteusSize;//0,3);
	float breastSize;//1,3);
	float belly;//0,4);
	float waist;//1,4);
	float lastValue;

	#endregion

	public void ReceiveValues(){

		UMAData.UMARecipe r = umaData.umaRecipe;
		umaDna = r.umaDna[typeof(UMADnaHumanoid)] as UMADnaHumanoid;
		
		if(umaDna != null){
			height = umaDna.height;
			upperMuscle = umaDna.upperMuscle ;
			lowerMuscle = umaDna.lowerMuscle;
			upperWeight = umaDna.upperWeight;
			lowerWeight = umaDna.lowerWeight;
			legsSize = umaDna.legsSize;
			gluteusSize = umaDna.gluteusSize;
			breastSize = umaDna.breastSize;
			belly = umaDna.belly;
			waist = umaDna.waist;
		}

	}

	public void TransferValues(){
		umaData = GameObject.FindWithTag ("Player").GetComponent<UMAData>();
		umaDna = umaData.umaRecipe.umaDna[typeof(UMADnaHumanoid)] as UMADnaHumanoid;
		
		if(umaDna != null){
			Debug.Log ("Transfering values");
			umaDna.height = height;
			umaDna.upperMuscle = upperMuscle;
			umaDna.lowerMuscle = lowerMuscle;
			umaDna.upperWeight = upperWeight;
			umaDna.lowerWeight = lowerWeight;
			umaDna.legsSize = legsSize;
			umaDna.gluteusSize = gluteusSize;
			umaDna.breastSize = breastSize;
			umaDna.belly = belly;
			umaDna.waist = waist;
		}

	}

	#endregion

	#region main functions

	void Start () {

		//init dictionary for customization colors
		categoriesIdxs = new Dictionary<string, int> ();

		categoriesIdxs.Add ("Skin", 0);
		categoriesIdxs.Add ("Eyes", 0);
		categoriesIdxs.Add ("Hair", 0);
		categoriesIdxs.Add ("Top", 0);
		categoriesIdxs.Add ("Pants", 0);

		//if (!loadAvatar ()){
			//to login
			//Application.LoadLevel("login-screen");
			Debug.Log ("can't load avatar");
		//}
	}

	//recipeStr is in pref. 
	void setCategoryIdxFromRecipe (){
		categoriesIdxs.Clear ();
		//get recipe
		string recipe = PlayerPrefs.GetString(DB_avatarCustomization.prefConfig);

		UMAPackedRecipeBase.UMAPackRecipe pr = JsonMapper.ToObject<UMAPackedRecipeBase.UMAPackRecipe>(recipe);

		UMAPackedRecipeBase.packedSlotData[] slots = pr.packedSlotDataList;
		//add idx for each category
		for (int i=0; i<slots.Length; i++) {
			UMAPackedRecipeBase.packedOverlayData[] overlays = slots[i].OverlayDataList;
			Color32 c = new Color32();
			for (int j=0; j<overlays.Length; j++){
				switch (overlays[j].overlayID){
					//get skin color
				case "FemaleHead01":
				case "FemaleHead02":
				case "MaleHead01":
				case "MaleHead02":
					c.r=(byte)overlays[j].colorList[0];
					c.g=(byte)overlays[j].colorList[1];
					c.b=(byte)overlays[j].colorList[2];
					c.a=(byte)overlays[j].colorList[3];
					for (int k=0; k<DB_avatarCustomization.skinColors.Length; k++){
						if (c.Equals(DB_avatarCustomization.skinColors[k]))
							categoriesIdxs.Add("Skin", k);
						break;
					}
					if (!categoriesIdxs.ContainsKey("Skin"))
						categoriesIdxs.Add("Skin", 0);
					break;
					//eyes
				case "EyeOverlayAdjust":
					c.r=(byte)overlays[j].colorList[0];
					c.g=(byte)overlays[j].colorList[1];
					c.b=(byte)overlays[j].colorList[2];
					c.a=(byte)overlays[j].colorList[3];
					for (int k=0; k<DB_avatarCustomization.eyesColors.Length; k++){
						if (c.Equals(DB_avatarCustomization.eyesColors[k]))
							categoriesIdxs.Add("Eyes", k);
						break;
					}
					if (!categoriesIdxs.ContainsKey("Eyes"))
						categoriesIdxs.Add("Eyes", 0);
					break;
				//hair
				case "MaleHair01":
				case "MaleHair02":
				case "FemaleLongHair01":
				case "FemaleShortHair01":
					c.r=(byte)overlays[j].colorList[0];
					c.g=(byte)overlays[j].colorList[1];
					c.b=(byte)overlays[j].colorList[2];
					c.a=(byte)overlays[j].colorList[3];
					for (int k=0; k<DB_avatarCustomization.hairColors.Length; k++){
						if (c.Equals(DB_avatarCustomization.hairColors[k]))
							categoriesIdxs.Add("Hair", k);
						break;
					}
					if (!categoriesIdxs.ContainsKey("Hair"))
						categoriesIdxs.Add("Hair", 0);
					break;
					//top
				case "MaleShirt01":
				case "FemaleShirt01":
				case "FemaleShirt02":
				case "FemaleTshirt01":
					c.r=(byte)overlays[j].colorList[0];
					c.g=(byte)overlays[j].colorList[1];
					c.b=(byte)overlays[j].colorList[2];
					c.a=(byte)overlays[j].colorList[3];
					for (int k=0; k<DB_avatarCustomization.clothesColors.Length; k++){
						if (c.Equals(DB_avatarCustomization.clothesColors[k]))
							categoriesIdxs.Add("Top", k);
						break;
					}
					if (!categoriesIdxs.ContainsKey("Top"))
						categoriesIdxs.Add("Top", 0);
					break;
					//pants
				case "MaleJeans01":
				case "FemaleJeans01":
				case "MaleUnderware01":
					c.r=(byte)overlays[j].colorList[0];
					c.g=(byte)overlays[j].colorList[1];
					c.b=(byte)overlays[j].colorList[2];
					c.a=(byte)overlays[j].colorList[3];
					for (int k=0; k<DB_avatarCustomization.clothesColors.Length; k++){
						if (c.Equals(DB_avatarCustomization.clothesColors[k]))
							categoriesIdxs.Add("Pants", k);
						break;
					}
					if (!categoriesIdxs.ContainsKey("Pants"))
						categoriesIdxs.Add("Pants", 0);
					break;
				}
			}
		}

	}

	void Update (){
		umaDynamicAvatar = GameObject.FindWithTag ("Player").GetComponent<UMADynamicAvatar>();
		umaData = GameObject.FindWithTag ("Player").GetComponent<UMAData>();

		if (changes||editing){
			umaData.isTextureDirty = changes;
			umaData.isShapeDirty = editing;

			umaData.Dirty();
		}
		if (editing)
			TransferValues ();
		else
			ReceiveValues();

	}
	
	void OnGUI()
	{
		#region sliders
	
		GUI.color = Color.black;
		GUILayout.BeginArea(new Rect(Screen.width/1.5f, 10, 100, Screen.height - 100));

		GUILayout.BeginVertical();
		GUILayout.Label("Height");
		lastValue = height;
		height = GUILayout.HorizontalSlider(height, 0.0F, 1.0F);
		if (height!=lastValue)
			editing = true;
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Upper Muscle");
		lastValue = upperMuscle;
		upperMuscle = GUILayout.HorizontalSlider(upperMuscle, 0.0F, 1.0F); //1,0);
		if (upperMuscle!=lastValue)
			editing = true;
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Lower Muscle");
		lastValue = lowerMuscle;
		lowerMuscle = GUILayout.HorizontalSlider(lowerMuscle, 0.0F, 1.0F);//0,1);
		if (lowerMuscle!=lastValue)
			editing = true;
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Upper Weight");
		lastValue = upperWeight;
		upperWeight = GUILayout.HorizontalSlider(upperWeight, 0.0F, 1.0F);//1,1);
		if (upperWeight!=lastValue)
			editing = true;
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Lower Weight");
		lastValue = lowerWeight;
		lowerWeight = GUILayout.HorizontalSlider(lowerWeight, 0.0F, 1.0F);//0,2);	
		if (lowerWeight!=lastValue)
			editing = true;
		GUILayout.EndVertical();

		GUILayout.EndArea();

		GUILayout.BeginArea(new Rect(Screen.width/1.5f+ 2*buttonWidth + 100, 10, 100, Screen.height - 100));

		GUILayout.BeginVertical();
		GUILayout.Label("Legs Size");
		lastValue = legsSize;
		legsSize = GUILayout.HorizontalSlider(legsSize, 0.0F, 1.0F);//1,2);
		if (legsSize!=lastValue)
			editing = true;
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Gluteus Size");
		lastValue = gluteusSize;
		gluteusSize = GUILayout.HorizontalSlider(gluteusSize, 0.0F, 1.0F);//0,3);
		if (gluteusSize!=lastValue)
			editing = true;
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Breast Size");
		lastValue = breastSize;
		breastSize = GUILayout.HorizontalSlider(breastSize, 0.0F, 1.0F);//1,3);
		if (breastSize!=lastValue)
			editing = true;
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Belly");
		lastValue = belly;
		belly = GUILayout.HorizontalSlider(belly, 0.0F, 1.0F);//0,4);
		if (belly!=lastValue)
			editing = true;
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Waist");
		lastValue = waist;
		waist = GUILayout.HorizontalSlider(waist, 0.0F, 1.0F);//1,4);
		if (waist!=lastValue)
			editing = true;
		GUILayout.EndVertical();

		GUILayout.EndArea();

		#endregion
		GUI.color = Color.white;
		//categories
		GUILayout.BeginArea(new Rect(Screen.width - (typeWidth + 2 * buttonWidth + 25), 10, typeWidth + 2 * buttonWidth + 8, 700));

		// Buttons for changing character elements.

		for (int i=0; i<DB_avatarCustomization.catNames.Length; i++) {
			AddCategory(DB_avatarCustomization.catNames[i]);	
		}

		// Buttons for saving and deleting configurations.
		if (GUILayout.Button("Save"))
			if (changes||editing){
				//save avatar config
				saveAvatar();
			} 
			
		if (GUILayout.Button("Reset"))
			if (changes||editing){
				//reset -- load the existing config
				loadAvatar();
			}
		//main menu button
		if (GUILayout.Button("Main Menu")) {
			drawWindow = true;

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
		if (buttonSaveDown || keyDown) {
			saveAvatar ();
			drawWindow = false;
			//load main-menu screen
			Application.LoadLevel ("main-menu");
		} 
		else if (buttonCancelDown) {
			drawWindow = false;
			//load main-menu screen
			Application.LoadLevel ("main-menu");
		}
	}

	//send local config to server
	void saveAvatar(){
		umaData = GameObject.FindWithTag ("Player").GetComponent<UMAData>();
		var avatar = umaData.gameObject.GetComponent<UMAAvatarBase>();
		if( avatar != null )
		{
			var asset = ScriptableObject.CreateInstance<UMATextRecipe>();
			asset.Save(umaData.umaRecipe, avatar.context);
			string config = asset.recipeString;

			Debug.Log("Saving recipe: "+ config);
			//save to prefs
			PlayerPrefs.SetString(DB_avatarCustomization.prefConfig, config);
			editing = false;
			changes=false;

			//save to server
			//TODO CHECK OBJ NAME 
			//ParseObject avatarStr = new ParseObject("avatarStr");
			//avatarStr[DB_avatarCustomization.prefConfig] = config;
			//avatarStr[DB_avatarCustomization.prefUserName] = ParseUser.CurrentUser.Username;
			//avatarStr.SaveAsync();

		}
	}

	bool loadAvatar(){
		try{
			//get string config from pref -- saved on login
			string recipe = PlayerPrefs.GetString(DB_avatarCustomization.prefConfig);
			Debug.Log("Recipe: "+recipe);

			//character = GameObject.Instantiate(DB_avatarCustomization.avatars[idx].gameObject, new Vector3(0.15f, 0.01f, 0.217f), Quaternion.identity) as GameObject;
			umaDynamicAvatar = GameObject.FindWithTag ("Player").GetComponent<UMADynamicAvatar>();

			if( umaDynamicAvatar != null && recipe != "")
			{
				var asset = ScriptableObject.CreateInstance<UMATextRecipe>();
				asset.recipeString = recipe;
				umaDynamicAvatar.Load(asset);
				Debug.Log("Loaded");
				Destroy(asset);
			}
			//umaData = null;

			changes = false;
			editing = false;
			umaData = GameObject.FindWithTag ("Player").GetComponent<UMAData>();
			Debug.Log("Data is null? "+(umaData==null));
			ReceiveValues ();
			setCategoryIdxFromRecipe();
			return true;
		}
		catch (JsonException){
			Debug.Log ("JSON exception");
			return false;
		}
		catch (System.Exception){
			Debug.Log ("system exception");
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
		int idx;
		if (categoriesIdxs.TryGetValue(category, out idx)){
			//update dictionary
			categoriesIdxs.Remove(category);
			int i = ChangeUMAElement (category, next, idx);
			categoriesIdxs.Add(category, i);
			changes = true;
		}

	}
		
	public int ChangeUMAElement (string category, bool next, int idx){
		umaData = GameObject.FindWithTag ("Player").GetComponent<UMAData>();

		UMAData.UMARecipe r = umaData.umaRecipe;
		SlotData[] slots = r.GetAllSlots ();
		
		SlotData slot = r.GetSlot (0);
		
		Debug.Log("idx beg "+idx);
		
		//SlotData[] slots = data.umaRecipe.slotDataList;
		bool male = umaData.umaRecipe.raceData.raceName == "HumanMale";
		
		//idx set in switch
		switch (category){
		case "Skin":
			//only skin colors
			if (next)
				idx = (idx+1==DB_avatarCustomization.skinColors.Length)?0:(++idx);
			else
				idx = (idx==0)?DB_avatarCustomization.skinColors.Length-1:(--idx);
			for (int i=0; i<slots.Length;i++)
			{
				slot = r.GetSlot (i);
				if (slot&&slot.slotName!=null){
					switch(slot.slotName){
					case "MaleFace":
						slot.SetOverlayColor(DB_avatarCustomization.skinColors[idx], "MaleHead02");
						break;
					case "MaleTorso":
						slot.SetOverlayColor(DB_avatarCustomization.skinColors[idx], "MaleBody01");
						break;
					case "FemaleFace":
						slot.SetOverlayColor(DB_avatarCustomization.skinColors[idx], "FemaleHead01");
						break;
					case "FemaleTorso":
						slot.SetOverlayColor(DB_avatarCustomization.skinColors[idx], "FemaleBody01");
						slot.SetOverlayColor(DB_avatarCustomization.skinColors[idx], "FemaleBody02");
						
						break;
					}
				}
			}
			
			break;
		case "Eyes":
			
			//only color
			if (next)
				idx = (idx+1==DB_avatarCustomization.eyesColors.Length)?0:(++idx);
			else
				idx = (idx==0)?DB_avatarCustomization.eyesColors.Length-1:(--idx);
			for (int i=0; i<slots.Length;i++)
			{
				slot = r.GetSlot (i);
				if (slot&&slot.slotName!=null){
					if (slot.slotName == "MaleEyes" || slot.slotName == "FemaleEyes"){
						slot.SetOverlayColor(DB_avatarCustomization.eyesColors[idx], "EyeOverlayAdjust");
					}
				}
			}
			
			break;
		case "Hair":
			//hair colors only!!!
			if (next)
				idx = (idx+1==DB_avatarCustomization.hairColors.Length)?0:(++idx);
			else
				idx = (idx==0)?DB_avatarCustomization.hairColors.Length-1:(--idx);
			
			//+ I need diff hear styles ---later!
			for (int i=0; i<slots.Length;i++)
			{
				slot = r.GetSlot (i);
				if (slot&&slot.slotName!=null){
					switch(slot.slotName){
					case "MaleFace":
						if (slot.GetOverlay("MaleHair02")!=null)
							slot.SetOverlayColor(DB_avatarCustomization.hairColors[idx], "MaleHair02");
						break;
					case "FemaleFace":
						slot.SetOverlayColor(DB_avatarCustomization.hairColors[idx], "FemailLongHair01");
						break;
						
					case "FemailLongHair01":
						slot.SetOverlayColor(DB_avatarCustomization.hairColors[idx], "FemailLongHair01");
						break;
					case "FemaleLongHair01_Module":
						slot.SetOverlayColor(DB_avatarCustomization.hairColors[idx], "FemaleLongHair01_Module");
						break;
					}
				}
			}
			
			break;
		case "Top":
			//colors only!!!
			if (next)
				idx = (idx+1==DB_avatarCustomization.clothesColors.Length)?0:(++idx);
			else
				idx = (idx==0)?DB_avatarCustomization.clothesColors.Length-1:(--idx);
			
			//+ I need diff styles ---later!
			for (int i=0; i<slots.Length;i++)
			{
				slot = r.GetSlot (i);
				if (slot&&slot.slotName!=null){
					if(slot.slotName=="MaleTorso"){
						slot.SetOverlayColor(DB_avatarCustomization.clothesColors[idx], "MaleShirt01");
					}
					else{
						//				case "FemaleTorso":
						if (slot.GetOverlay("FemaleShirt01")!=null)
							slot.SetOverlayColor(DB_avatarCustomization.clothesColors[idx], "FemaleShirt01");
						else//02
							slot.SetOverlayColor(DB_avatarCustomization.clothesColors[idx], "FemaleShirt02");
						if (slot.GetOverlay("FemaleTshirt01")!=null)
							slot.SetOverlayColor(DB_avatarCustomization.clothesColors[idx], "FemaleTshirt01");
						
					}
				}
			}
			break;
		case "Pants":
			//colors only!!!
			if (next)
				idx = (idx+1==DB_avatarCustomization.clothesColors.Length)?0:(++idx);
			else
				idx = (idx==0)?DB_avatarCustomization.clothesColors.Length-1:(--idx);
			
			Debug.Log("idx pants "+idx);
			
			//+ I need diff styles ---later!
			for (int i=0; i<slots.Length;i++)
			{
				slot = r.GetSlot (i);
				if (slot&&slot.slotName!=null){
					if(slot.slotName=="FemaleTorso"){
						slot.SetOverlayColor(DB_avatarCustomization.clothesColors[idx], "FemaleJeans01");
					}
					else{
						//case "maleTorso":
						if (slot.GetOverlay("MaleJeans01")!=null)
							slot.SetOverlayColor(DB_avatarCustomization.clothesColors[idx], "MaleJeans01");
						slot.SetOverlayColor(DB_avatarCustomization.clothesColors[idx], "MaleUnderware01");
					}
				}
				
			}
			
			break;
		default:
			break;
		}
		
		Debug.Log("idx end "+idx);
		
		return idx;
	}

	#endregion
}