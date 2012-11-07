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
	
	public float transitionTime = 0.5f;
	private float transTimer;
	private Vector3 transPos;
	private Quaternion transRot;
	private bool transitioning = false;
	
	// Use this for initialization
	void Start () {
		aimCamera = GetComponent<AimCamera>();
		moveCamera = GetComponent<GameCamera>();
		for(int i = 0; i < (int)CameraModes.Last; i++){
			ToggleCameraMode((CameraModes)i, false);
		}
		ToggleCameraMode(cameraMode, true);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Camera"))
		{
			SwitchCameraMode();
		}
		Vector3 newPosition = transform.position;
		Quaternion newRotation = transform.rotation;
		switch(cameraMode){
			case CameraModes.AimCamera:
				newPosition = aimCamera.position;
				newRotation = aimCamera.rotation;
				break;
			case CameraModes.MoveCamera:
				newPosition = moveCamera.position;
				newRotation = moveCamera.rotation;
				break;
		}
		if(transitioning){
			transform.position = Vector3.Slerp(transPos, newPosition, transTimer / transitionTime);
			transform.rotation = Quaternion.Slerp(transRot, newRotation, transTimer / transitionTime);
			transTimer += Time.deltaTime;
			if(transTimer >= transitionTime){
				transitioning = false;	
			}
		}
		else{
			transform.position = newPosition;
			transform.rotation = newRotation;
		}
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
			transPos = transform.position;
			transRot = transform.rotation;
			transTimer = 0;
			transitioning = true;
		}
	}
	
	void SwitchCameraMode(){
		ToggleCameraMode(cameraMode, false);
		cameraMode++;
		cameraMode = cameraMode >= CameraModes.Last ? (CameraModes)((int)cameraMode % (int)CameraModes.Last) : cameraMode;
		ToggleCameraMode(cameraMode, true);
	}
}
