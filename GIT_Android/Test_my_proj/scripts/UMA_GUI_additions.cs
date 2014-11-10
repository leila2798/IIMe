using UnityEngine;
using System.Collections;
using UMA;

public class UMA_GUI_Additions : MonoBehaviour {
	
	public Transform sliderPrefab;
	
	public UMAData umaData;
	public UMADynamicAvatar umaDynamicAvatar;
	public CameraTrack cameraTrack;
	public UMADnaHumanoid umaDna;
	
	public SliderControl[] sliderControlList;
	
	public SlotLibrary mySlotLibrary;
    public OverlayLibrary myOverlayLibrary;
	public bool editing = false;

	void Start () {
		sliderControlList = new SliderControl[10];	
		//Changed slider order
		
		sliderControlList[0] = InstantiateSlider("height",0,0);
		sliderControlList[1] = InstantiateSlider("upper muscle",1,0);
		sliderControlList[2] = InstantiateSlider("lower muscle",2,0);
		
		sliderControlList[3] = InstantiateSlider("upper weight",0,1);
		sliderControlList[4] = InstantiateSlider("lower weight",1,1);	

		sliderControlList[5] = InstantiateSlider("legsSize",2,2);
		sliderControlList[6] = InstantiateSlider("Gluteus Size",3,2);
		
		sliderControlList[7] = InstantiateSlider("breatsSize",0,3);
		sliderControlList[8] = InstantiateSlider("belly",1,3);
		sliderControlList[9] = InstantiateSlider("waist",2,3);
	}
	

	void Update () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Input.GetMouseButtonDown(1)){
			if (Physics.Raycast(ray, out hit, 100)){

				umaData = hit.collider.transform.parent.parent.GetComponent<UMAData>();
				if(umaData){
					AvatarSetup();
				}
			}
		}
		
		if(umaData){
			TransferValues();
			editing = false;
			for(int i = 0; i < sliderControlList.Length; i++){
				if(sliderControlList[i].pressed == true){
					editing = true;
					UpdateUMAShape();
				}
			}
		}
	}

	public void AvatarSetup(){
		umaDynamicAvatar = umaData.gameObject.GetComponent<UMADynamicAvatar>();

		if(cameraTrack){
			cameraTrack.target = umaData.umaRoot.transform;
		}
		
		umaDna = umaData.umaRecipe.umaDna[typeof(UMADnaHumanoid)] as UMADnaHumanoid;
		ReceiveValues();
	}

	public SliderControl InstantiateSlider(string name, int X, int Y){
		Transform TempSlider;
		TempSlider = Instantiate(sliderPrefab,Vector3.zero, Quaternion.identity) as Transform;
		TempSlider.parent = transform;
		TempSlider.gameObject.name = name;
		SliderControl tempSlider = TempSlider.GetComponent("SliderControl") as SliderControl;
		tempSlider.actualValue = 0.5f;
		tempSlider.descriptionText.text = name;
		tempSlider.sliderOffset.x = 20 + X*100;
		tempSlider.sliderOffset.y = -20 - Y*60;
		return tempSlider;
	}
	
	public SliderControl InstantiateStepSlider(string name, int X, int Y){
		SliderControl tempSlider = InstantiateSlider(name,X,Y);
		tempSlider.stepSlider = true;
		
		return tempSlider;
	}
	
	
	public void UpdateUMAAtlas(){
		umaData.isTextureDirty = true;
		umaData.Dirty();	
	}
	
	public void UpdateUMAShape(){
		umaData.isShapeDirty = true;
		umaData.Dirty();
	}
	
	public void ReceiveValues(){
		if(umaDna != null){
			sliderControlList[0].actualValue = umaDna.height;
			sliderControlList[1].actualValue = umaDna.upperMuscle ;
			sliderControlList[2].actualValue = umaDna.lowerMuscle;
			sliderControlList[3].actualValue = umaDna.upperWeight;
			sliderControlList[4].actualValue = umaDna.lowerWeight;
			sliderControlList[5].actualValue = umaDna.legsSize;
			sliderControlList[6].actualValue = umaDna.gluteusSize;
			sliderControlList[7].actualValue = umaDna.breastSize;
			sliderControlList[8].actualValue = umaDna.belly;
			sliderControlList[9].actualValue = umaDna.waist;

		}
	}
	
	
	public void TransferValues(){
		if(umaDna != null){
			umaDna.height = sliderControlList[0].actualValue;
			umaDna.upperMuscle = sliderControlList[1].actualValue;
			umaDna.lowerMuscle = sliderControlList[2].actualValue;
			umaDna.upperWeight = sliderControlList[3].actualValue;
			umaDna.lowerWeight = sliderControlList[4].actualValue;
			umaDna.legsSize = sliderControlList[5].actualValue;
			umaDna.gluteusSize = sliderControlList[6].actualValue;
			umaDna.breastSize = sliderControlList[7].actualValue;
			umaDna.belly = sliderControlList[8].actualValue;
			umaDna.waist = sliderControlList[9].actualValue;
		}
	}
}
