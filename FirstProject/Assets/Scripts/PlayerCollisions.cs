using UnityEngine;
using System.Collections;

public class PlayerCollisions : MonoBehaviour {
	private GameObject currentDoor = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, 3)){
			if(hit.collider.gameObject.tag == "playerDoor"){
				currentDoor = hit.collider.gameObject;
				currentDoor.SendMessage("DoorCheck");
			}
		}
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit){
//		Debug.Log("collision: " + hit.gameObject.name);
//		if(hit.collider.gameObject.tag == "bear"){
//			Debug.Log("adding force");
//			//hit.gameObject.rigidbody.velocity = ( hit.gameObject.transform.position - transform.position).normalized * 10f;
//			//hit.gameObject.rigidbody.AddForce((hit.gameObject.transform.position - transform.position).normalized * 100f);
//		}
	}
}
