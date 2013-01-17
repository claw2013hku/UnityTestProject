using UnityEngine;
using System.Collections;

public class IHitBox : MonoBehaviour {
	public bool activated;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	virtual public void Activate(bool activate){
		this.activated = activate;
		gameObject.SetActive(activate);
		if(activate){
			rigidbody.WakeUp();	
		}
	}
}
