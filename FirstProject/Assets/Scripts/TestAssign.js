#pragma strict
var launcher : GameObject;
var mainCamera : GameObject;
function Start () {
	var _NetworkRigidbody : NetworkTransform = GetComponent("NetworkTransform");
  	_NetworkRigidbody.enabled = false;
  	mainCamera = GameObject.FindWithTag("MainCamera");
 	var aimCamera : AimCamera = mainCamera.GetComponent("AimCamera");
    aimCamera.player = transform;
 	var gameCamera : GameCamera = mainCamera.GetComponent("GameCamera");
    gameCamera.focusTransform = transform;
    gameObject.tag = "Player";

	var thrower : NewCoconutThrower = launcher.GetComponent("NewCoconutThrower");
	thrower.enabled = true;
	thrower.SetCamera(mainCamera);
		
	var controller : ThirdPersonTouchController = GetComponent("ThirdPersonTouchController");
	controller.enabled = true;
}