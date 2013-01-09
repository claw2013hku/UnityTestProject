using UnityEngine;
using System.Collections;

public class ProjectileAttacher : MonoBehaviour {
	public bool stickOnAnyCollider = true;
	private bool attached = false;
	public Transform localContactPoint;
	public bool takeContactPoint;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision col){
		if(!attached){
			if(stickOnAnyCollider && col.collider as CharacterController == null){
//				if(col.rigidbody != null){
//					//transform.position += col.contacts[1].point - localContactPoint.position;
//					gameObject.AddComponent<FixedJoint>();
//					GetComponent<FixedJoint>().connectedBody = col.rigidbody;
//					attached = true;
//				}
//				else{
//					//transform.position += col.contacts[1].point - localContactPoint.position;
//					transform.parent = col.gameObject.transform;
//					rigidbody.isKinematic = true;
//					if(collider != null){
//						collider.enabled = false;	
//					}
//					attached = true;
				Vector3 attachPoint = Vector3.zero;
				foreach(ContactPoint point in col.contacts){
					Debug.Log("Contact Point: " + point.point);
					if(attachPoint == Vector3.zero || (point.point - transform.position).sqrMagnitude < (attachPoint - transform.position).sqrMagnitude){
						attachPoint = point.point;
					}
				}
				if(takeContactPoint){
					transform.position = col.contacts[0].point - localContactPoint.localPosition;
				
				}
				else{
					transform.position = col.contacts[0].point;
				
				}
				
//				if(col.rigidbody != null){
					//transform.position += col.contacts[1].point - localContactPoint.position;
//					gameObject.AddComponent<FixedJoint>();
//					GetComponent<FixedJoint>().connectedBody = col.rigidbody;
//				}
//				else{
					transform.parent = col.gameObject.transform;
					Destroy(rigidbody);
//				}
//					//transform.position += col.contacts[1].point - localContactPoint.position;
//					gameObject.AddComponent<FixedJoint>();
//					GetComponent<FixedJoint>().connectedBody = col.rigidbody;
//					attached = true;
//				}
				if(collider != null){
					collider.enabled = false;	
				}
				attached = true;
			
			}
		}
	}
	
//	void OnTriggerEnter(Collider col){
//		if(!attached && col.rigidbody != null){
//			ProjectileReceiver receiver = col.GetComponent<ProjectileReceiver>();
//			if(receiver != null){
//				ProjectileAttacher attacher = GetComponent<ProjectileAttacher>();
//				if(receiver.Attach(attacher)){
//					//Vector3 position = receiver.GetAttachPosition(attacher);
//					//transform.position = position;
//					gameObject.AddComponent<FixedJoint>();
//					GetComponent<FixedJoint>().connectedBody = col.rigidbody;
//					attached = true;
//					//rigidbody.isKinematic = true;
//				}
//			}
//		}
//	}
	
	
}
