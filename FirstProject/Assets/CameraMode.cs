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
	
	// Use this for initialization
	void Start () {
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
	}
	
	void ToggleCameraMode(CameraModes mode, bool on){
		switch(mode){
		case CameraModes.AimCamera:
			GetComponent<AimCamera>().enabled = on;
			if(!on)
				SendMessage("LoadDirection");
			else
				SendMessage("SaveDirection");
			break;
		case CameraModes.MoveCamera:
			GetComponent<GameCamera>().enabled = on;
			if(!on)
				SendMessage("ResetAngle");
			break;
		}
	}
	
	void SwitchCameraMode(){
		ToggleCameraMode(cameraMode, false);
		cameraMode++;
		cameraMode = cameraMode >= CameraModes.Last ? (CameraModes)((int)cameraMode % (int)CameraModes.Last) : cameraMode;
		ToggleCameraMode(cameraMode, true);
	}
}
