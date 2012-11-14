using UnityEngine;
using System.Collections;

public class TriggerZone : MonoBehaviour {
	public AudioClip lockedSound;
	public Light doorLight;
	public GUIText textHints;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider col){
//		if(col.gameObject.tag == "Player"){
//			if(Inventory.charge == 4){
//				
//			}
//			else if (Inventory.charge > 0){
//				transform.FindChild("door").audio.PlayOneShot(lockedSound);
//				col.gameObject.SendMessage("HUDon");
//				textHints.SendMessage("ShowHint", "This door seems locked... maybe more powers would help");
//			}
//			else{
//				transform.FindChild("door").audio.PlayOneShot(lockedSound);
//				col.gameObject.SendMessage("HUDon");
//				textHints.SendMessage("ShowHint", "This door seems locked... maybe that generator needs power");
//			}
//		}
	}
	
	void OnTriggerStay(Collider col){
		if(col.gameObject.tag == "Player" || col.gameObject.tag == "RemotePlayer"){
			transform.FindChild("door").SendMessage("DoorCheck");	
			if(GameObject.Find ("PowerGUI")){
				Destroy(GameObject.Find("PowerGUI"));
			}
//			if(Inventory.charge == 4){
//				transform.FindChild("door").SendMessage("DoorCheck");	
//				if(GameObject.Find ("PowerGUI")){
//					Destroy(GameObject.Find("PowerGUI"));
//					doorLight.color = Color.green;
//				}
//			}
//			else{
//			}
		}	
	}
}
