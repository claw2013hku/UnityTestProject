#pragma strict
var launcher : GameObject;
var mainCamera : GameObject;
function OnNetworkInstantiate (msg : NetworkMessageInfo) {
	Debug.Log("onnetworkinstantiate");
	if (networkView.isMine) 
	{
		var _NetworkRigidbody : NetworkTransform = 
		GetComponent("NetworkTransform");
		_NetworkRigidbody.enabled = false;
		mainCamera = GameObject.FindWithTag("MainCamera");
		if(mainCamera == null){
			Debug.Log("not found");
		}
		else{
			Debug.Log("found");
		}
		var aimCamera : AimCamera = mainCamera.GetComponent("AimCamera");
		aimCamera.player = transform;
		var gameCamera : GameCamera = mainCamera.GetComponent("GameCamera");
		gameCamera.player = transform;
		gameCamera.playerObj = gameObject;
		Debug.Log("assigned local player");
		gameObject.tag = "Player";
	
		var thrower : NewCoconutThrower = launcher.GetComponent("NewCoconutThrower");
		thrower.enabled = true;
		thrower.SetCamera(mainCamera);
		var controller : ThirdPersonTouchController = GetComponent("ThirdPersonTouchController");
	 	controller.enabled = true;
	}
	else 
	{
		name += "Remote";
		var _NetworkRigidbody2 : NetworkTransform = 
		GetComponent("NetworkTransform");
		_NetworkRigidbody2.enabled = true;  
		gameObject.tag = "RemotePlayer";
	   	Debug.Log("assigned remote player");
	}
}