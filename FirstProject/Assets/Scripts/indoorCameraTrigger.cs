using UnityEngine;
using System.Collections;

public class indoorCameraTrigger : MonoBehaviour {
	public Transform cameraTransform;
	public CameraMode cameraMaster;
	public Transform onExitTransform;
	public bool transitionOnExit = false;
	public bool transitionOnEnter = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player")
			cameraMaster.ActivateIndoorCamera(cameraTransform, transitionOnEnter, gameObject);
	}
	
	void OnTriggerExit(Collider col){
		if(col.gameObject.tag == "Player"){
			if(onExitTransform == null){	
				cameraMaster.DeactivateIndoorCamera(gameObject);
			}
			else{
				cameraMaster.DeactivateIndoorCamera(onExitTransform.position, onExitTransform.rotation, transitionOnExit, gameObject);	
			}
		}
			
	}
}
