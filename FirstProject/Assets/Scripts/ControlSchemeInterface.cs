using UnityEngine;
using System.Collections;

public class ControlSchemeInterface : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public float GetAxis(string axisName){
		return 0f;	
	}
	
	public bool GetButtonDown(string buttonName){
		return false;
	}
	
	public bool GetButtonUp(string buttonName){
		return false;
	}
	
	public bool GetButton(string buttonName){
		return false;	
	}
}
