using UnityEngine;
using System.Collections;

public class DoorManager : MonoBehaviour {

	private bool doorIsOpen = false;
	private float doorTimer = 0.0f;
	private float doorOpenTime = 3.0f;
	private GameObject currentDoor = null;
	public AudioClip doorOpenSound;
	public AudioClip doorShutSound;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(doorIsOpen){
			doorTimer += Time.deltaTime;
			if(doorTimer >= doorOpenTime){
				Door (doorShutSound, false, "doorshut");
				doorTimer = 0.0f;	
			}
		}
	}
	
	void DoorCheck(){
		if(!doorIsOpen){
			Door (doorOpenSound, true, "dooropen");
		}
		else{
		   	doorTimer = 0.0f;	
		}
	}
	
	void Door(AudioClip aClip, bool openCheck, string animName){
		audio.PlayOneShot(aClip);
		transform.parent.animation.Play(animName);
		doorIsOpen = openCheck;
	}
}
