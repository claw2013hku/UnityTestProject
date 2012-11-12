using UnityEngine;
using System.Collections;
[RequireComponent (typeof(AudioSource))]

public class NewCoconutThrower : MonoBehaviour {
	public AudioClip throwSound;
	public Rigidbody coconutPrefab;
	public float throwSpeed = 30f;
	public static bool canThrow = false;
	public GameObject mainCamera;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(ControlSchemeInterface.instance.GetAxis(ControlAxis.THROW) == 1.0f /*&& canThrow*/){
			audio.PlayOneShot(throwSound);	
			Rigidbody newCoconut = Instantiate(coconutPrefab, transform.position, transform.rotation) as Rigidbody;
			newCoconut.name = "coconut";
			if(newCoconut.rigidbody == null){
				newCoconut.gameObject.AddComponent<Rigidbody>();
			}
			newCoconut.position += mainCamera.transform.forward * 2;
			newCoconut.rigidbody.velocity = mainCamera.transform.forward * throwSpeed;
		}
	}
	
//	void ShootCoconut(){
//		audio.PlayOneShot(throwSound);	
//		Rigidbody newCoconut = Instantiate(coconutPrefab, transform.position, transform.rotation) as Rigidbody;
//		newCoconut.name = "coconut";
//		if(newCoconut.rigidbody == null){
//			newCoconut.gameObject.AddComponent<Rigidbody>();
//		}
//		newCoconut.position += mainCamera.transform.forward * 2;
//		newCoconut.rigidbody.velocity = mainCamera.transform.forward * throwSpeed;
//	}
}
