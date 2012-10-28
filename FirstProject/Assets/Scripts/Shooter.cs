using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour {
	public Rigidbody bullet = null;
	public float power = 1500f;
	public float moveSpeed = 2f;
	
	void Start () {
//		bullet = null;
//		power = 1500f;
//		moveSpeed = 2f;
	}
	
	void Update () {
		float v = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
		float h = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
		transform.Translate(h, v, 0);
		
		if(Input.GetButtonUp("Fire1")){
			Rigidbody body = Instantiate(bullet, transform.position, transform.rotation) as Rigidbody;
			Vector3 fwd = transform.TransformDirection(transform.forward);
			body.AddForce(fwd * power);
		}
	}
}
