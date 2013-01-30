using UnityEngine;
using System.Collections;

public class CameraMode : MonoBehaviour {
	public enum CameraModes
	{
		AimCamera,
		MoveCamera,
		Last
	}
	public CameraModes cameraMode;
	public AimCamera aimCamera;
	public GameCamera moveCamera;
	
	public GameObject focusObject;
	
	public float modeTransitionTime = 0.5f;
	public float indoorExitTransitionTime = 2f;
	private float transitionTime = 0.5f;
	
	private float transTimer;
	private Vector3 transPos;
	private Quaternion transRot;
	private bool transitioning = false;
	
	private CameraModes lastCameraMode;
	
	private bool useIndoorCamera = false;
	private Transform indoorCameraTransform;
	
	private GameObject currentIndoorTrigger;

	
	// Use this for initialization
	void Start () {
		aimCamera = GetComponent<AimCamera>();
		moveCamera = GetComponent<GameCamera>();
		
		if(focusObject != null){
			FocusTransform(focusObject);
		}
		
		for(int i = 0; i < (int)CameraModes.Last; i++){
			ToggleCameraMode((CameraModes)i, false);
		}
		ToggleCameraMode(cameraMode, true);
	}
	
	// Update is called once per frame
	void Update () {
		if(lastCameraMode != CameraModes.AimCamera && ControlSchemeInterface.instance.GetAxis(ControlAxis.AIMING) == 1.0f)
		{
			SwitchCameraMode(CameraModes.AimCamera);
		}
		else if (lastCameraMode != CameraModes.MoveCamera && ControlSchemeInterface.instance.GetAxis(ControlAxis.AIMING) == 0.0f){
			SwitchCameraMode(CameraModes.MoveCamera);	
		}
		
		Vector3 newPosition = transform.position;
		Quaternion newRotation = transform.rotation;
		switch(cameraMode){
			case CameraModes.AimCamera:
				newPosition = aimCamera.position;
				newRotation = aimCamera.rotation;
				break;
			case CameraModes.MoveCamera:
				if(!useIndoorCamera){
					newPosition = moveCamera.position;
					newRotation = moveCamera.rotation;
				}
				else{
					newPosition = indoorCameraTransform.position;
					newRotation = indoorCameraTransform.rotation;
				}
				break;
		}
		if(transitioning){
		Debug.Log("transitioning");
			if(transitionTime != 0f){
				transform.position = Vector3.Slerp(transPos, newPosition, transTimer / transitionTime);
				transform.rotation = Quaternion.Slerp(transRot, newRotation, transTimer / transitionTime);
			}
			transTimer += Time.deltaTime;
			if(transTimer >= transitionTime){
				transitioning = false;	
			}
		}
		else{
			transform.position = newPosition;
			transform.rotation = newRotation;
		}
		lastCameraMode = cameraMode;
	}
	
	void ToggleCameraMode(CameraModes mode, bool on){
		switch(mode){
		case CameraModes.AimCamera:
			aimCamera.enabled = on;
			if(on){
				aimCamera.SetCurrentAngle();
				aimCamera.Apply();
			}
			break;
		case CameraModes.MoveCamera:
			moveCamera.enabled = on;
			if(on)
				moveCamera.ResetDirection(transform.rotation);
			break;
		}
		if(on){
			StartTransition(transform.position, transform.rotation, modeTransitionTime);
		}
	}
	
	void NextCameraMode(){
		ToggleCameraMode(cameraMode, false);
		cameraMode++;
		cameraMode = cameraMode >= CameraModes.Last ? (CameraModes)((int)cameraMode % (int)CameraModes.Last) : cameraMode;
		ToggleCameraMode(cameraMode, true);
	}
			
	void SwitchCameraMode(CameraModes mode){
		ToggleCameraMode(cameraMode, false);
		cameraMode = mode;
		ToggleCameraMode(cameraMode, true);
	}
	
	public void ActivateIndoorCamera(Transform _transform, bool transition, GameObject caller){
		useIndoorCamera = true;
		indoorCameraTransform = _transform;
		currentIndoorTrigger = caller;
		if(transition){
			StartTransition(transform.position, transform.rotation, indoorExitTransitionTime);
		}
	}
	
	public void DeactivateIndoorCamera(Vector3 resetPosition, Quaternion resetRotation, bool transition, GameObject caller){
		DeactivateIndoorCamera(caller);
		moveCamera.Reset(resetPosition, resetRotation);
		if(transition)
			StartTransition(transform.position, transform.rotation, indoorExitTransitionTime);
	}
	
	public void DeactivateIndoorCamera(GameObject caller){
		if(currentIndoorTrigger == caller)
			useIndoorCamera = false;
	}
	
	private void StartTransition(Vector3 fromPos, Quaternion fromRot, float time){
		transPos = fromPos;
		transRot = fromRot;
		transTimer = 0;
		transitioning = true;	
		transitionTime = time;
	}

	public void FocusTransform(GameObject focus){
		focusObject = focus;
		if(aimCamera != null){
			aimCamera.player = focus.GetComponent<CameraFocus>().focus;
		}
		if(moveCamera != null){
			moveCamera.focusTransform = focus.GetComponent<CameraFocus>().focus;
		}
	}
}
