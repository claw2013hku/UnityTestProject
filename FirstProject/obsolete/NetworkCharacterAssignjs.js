//#pragma strict
//function OnNetworkInstantiate (msg : NetworkMessageInfo) {
//	Debug.Log("onnetworkinstantiate");
//	if (networkView.isMine) 
//	{
//		var _NetworkCharacterTest : NetworkCharacterTest = 
//		GetComponent("NetworkCharacterTest");
//		_NetworkCharacterTest.enabled = false;
//		var mainCamera = GameObject.FindWithTag("MainCamera");
//		if(mainCamera == null){
//			Debug.Log("not found");
//		}
//		else{
//			Debug.Log("found");
//		}
//		var cameraMode : CameraMode = mainCamera.GetComponent("CameraMode");
//		cameraMode.FocusTransform(gameObject);
//		
////		var aimCamera : AimCamera = mainCamera.GetComponent("AimCamera");
////		aimCamera.player = transform;
////		var gameCamera : GameCamera = mainCamera.GetComponent("GameCamera");
////		gameCamera.player = transform;
////		gameCamera.playerObj = gameObject;
//
//		Debug.Log("assigned local player");
//		//gameObject.tag = "Player";
//	
////		var thrower : NewCoconutThrower = launcher.GetComponent("NewCoconutThrower");
////		thrower.enabled = true;
////		thrower.SetCamera(mainCamera);
////		var controller : ThirdPersonTouchController = GetComponent("ThirdPersonTouchController");
////	 	controller.enabled = true;
//	}
//	else 
//	{
//		name += "Remote";
//		var _NetworkCharacterTest2 : NetworkCharacterTest = 
//		GetComponent("NetworkCharacterTest");
//		_NetworkCharacterTest2.enabled = true;
//		
//		var _MovesController : MovesControllerTest = 
//		GetComponent("MovesControllerTest");
//		_MovesController.enabled = false;
//		
//		var _TestCharacterMotor : TestCharacterMotor = 
//		GetComponent("TestCharacterMotor");
//		_TestCharacterMotor.enabled = false;
//	}
//}