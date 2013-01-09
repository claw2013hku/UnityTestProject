using UnityEngine;
using System.Collections;
[RequireComponent (typeof(AudioSource))]

public class CoconutThrower : MonoBehaviour {
	public AudioClip throwSound;
	public Rigidbody coconutPrefab;
	public float throwSpeed = 30f;
	public static bool canThrow = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(ControlSchemeInterface.instance.GetAxis(ControlAxis.THROW) > 0f /*&& canThrow*/){
			audio.PlayOneShot(throwSound);	
			Rigidbody newCoconut = Instantiate(coconutPrefab, transform.position, transform.rotation) as Rigidbody;
			newCoconut.name = "coconut";
			if(newCoconut.rigidbody == null){
				newCoconut.gameObject.AddComponent<Rigidbody>();
			}
			newCoconut.rigidbody.velocity = Camera.main.transform.forward * throwSpeed;
			newCoconut.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
		}
	}
}
