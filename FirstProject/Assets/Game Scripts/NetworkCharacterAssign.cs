using UnityEngine;
using System.Collections;

public class NetworkCharacterAssign : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("network assign");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info) {
	    if (networkView.isMine) 
		{
//			NetworkCharacterTest _NetworkCharacterTest = GetComponent<NetworkCharacterTest>();
//			_NetworkCharacterTest.enabled = false;
//			
//			NetworkCharacterReliableTest _NetworkCharacterReliableTest = GetComponent<NetworkCharacterReliableTest>();
//			_NetworkCharacterReliableTest.enabled = false;
//			NetworkTransform _NetworkTransform = GetComponent<NetworkTransform>();
//			_NetworkTransform.enabled = false;
			
			NetworkCharacterTest _NetworkCharacterTest2 = GetComponent<NetworkCharacterTest>();
			_NetworkCharacterTest2.enabled = true;
			_NetworkCharacterTest2.slashHitbox = GetComponent<HitboxController>().slashHitbox;
			
			NetworkCharacterReliableTest _NetworkCharacterReliableTest2 = GetComponent<NetworkCharacterReliableTest>();
			_NetworkCharacterReliableTest2.enabled = true;
			_NetworkCharacterReliableTest2.slashHitbox = GetComponent<HitboxController>().slashHitbox;
			
			GameObject mainCamera = GameObject.FindWithTag("MainCamera");
			if(mainCamera == null){
				Debug.Log("not found");
			}
			else{
				Debug.Log("found");
			}
			CameraMode cameraMode = mainCamera.GetComponent<CameraMode>();
			cameraMode.FocusTransform(gameObject);
			
	//		var aimCamera : AimCamera = mainCamera.GetComponent("AimCamera");
	//		aimCamera.player = transform;
	//		var gameCamera : GameCamera = mainCamera.GetComponent("GameCamera");
	//		gameCamera.player = transform;
	//		gameCamera.playerObj = gameObject;
	
			Debug.Log("assigned local player");
			//gameObject.tag = "Player";
		
	//		var thrower : NewCoconutThrower = launcher.GetComponent("NewCoconutThrower");
	//		thrower.enabled = true;
	//		thrower.SetCamera(mainCamera);
	//		var controller : ThirdPersonTouchController = GetComponent("ThirdPersonTouchController");
	//	 	controller.enabled = true;
		}
		else 
		{
			Debug.Log("assigned remote player");
			name += "Remote";
			NetworkCharacterTest _NetworkCharacterTest2 = GetComponent<NetworkCharacterTest>();
			_NetworkCharacterTest2.enabled = true;
			_NetworkCharacterTest2.slashHitbox = GetComponent<HitboxController>().slashHitbox;
			
			NetworkCharacterReliableTest _NetworkCharacterReliableTest2 = GetComponent<NetworkCharacterReliableTest>();
			_NetworkCharacterReliableTest2.enabled = true;
			_NetworkCharacterReliableTest2.slashHitbox = GetComponent<HitboxController>().slashHitbox;
//			NetworkTransform _NetworkTransform = GetComponent<NetworkTransform>();
//			_NetworkTransform.enabled = true;
			
			HitboxController _MovesController = GetComponent<HitboxController>();
			_MovesController.enabled = false;
			
			TestCharacterMotor _TestCharacterMotor = GetComponent<TestCharacterMotor>();
			_TestCharacterMotor.enabled = false;
		}
    }
}
