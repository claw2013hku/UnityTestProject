using UnityEngine;
using System.Collections;

public class ControlSchemeInterface : MonoBehaviour {
	public static ControlSchemeInterface instance;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	virtual public float GetAxis(ControlAxis axis){
		return 0f;	
	}
}

public enum ControlAxis{THROW, CAMERA_SCROLL_X, CAMERA_SCROLL_Y, MOVE_X, MOVE_Y, AIMING, DEBUG, ATTACK1, RUN};
