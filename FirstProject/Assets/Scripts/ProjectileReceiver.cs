using UnityEngine;
using System.Collections;

public class ProjectileReceiver : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public bool Attach(ProjectileAttacher attacher){
		return true;	
	}
	
	public Vector3 GetAttachPosition(ProjectileAttacher attacher){
		return attacher.transform.position;	
	}
}
