using UnityEngine;
using System.Collections;

public class IActorStatusEffect : MonoBehaviour {
	public bool attachOnStart;
	// Use this for initialization
	virtual protected void Start () {
		if(attachOnStart){
			ActorStatus status = GetComponent<ActorStatus>();
			if(GetComponent<ActorStatus>() != null){
				status.AttachStatusEffect(this);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	virtual public void OnAttach(ActorStatus status){
	}
	
	virtual public void OnApply(ActorStatus status){
	}
	
	virtual public void OnDetach(ActorStatus status){
	}
	
	virtual public bool IsDead(){
		return true;
	}
	
	virtual public string GetName(){
		return "";	
	}
}
