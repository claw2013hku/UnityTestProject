using UnityEngine;
using System.Collections;

public class SpeedModifierTest : IActorStatusEffect {
	public Collider area;
	private bool dead = false;
	private ActorStatus status;
	
	public float plusModifier1 = 0f;
	public float mulModifier2 = 1f;
	public float plusModifier3 = 0f;
	public float mulModifier4 = 1f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerExit(Collider col){
		if(area == null){
			Debug.LogError("area not set");	
		}
		else if (area == col){
			dead = true;
//			Debug.Log ("setdead");
		}
//		Debug.Log("Exited");
	}
	
	public override bool IsDead(){
		return dead;	
	}
	
	public override void OnAttach(ActorStatus _status){
//		Debug.Log("OnAttach");
		status = _status;
	}
	
	public override void OnApply(ActorStatus status){
//		Debug.Log ("OnApply");
		status.WriteStatus().MoveSpeedModifiers[0] += plusModifier1;
		status.WriteStatus().MoveSpeedModifiers[1] *= mulModifier2;
		status.WriteStatus().MoveSpeedModifiers[2] += plusModifier3;
		status.WriteStatus().MoveSpeedModifiers[3] *= mulModifier4;
//		Debug.Log (status.GetModifiers(ActorStatus.StatusType.MOVESPEED)[1]);
	}
	
	virtual public string GetName(){
		return "SpeedAreaModify";	
	}
}
