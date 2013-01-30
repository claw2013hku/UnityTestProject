using UnityEngine;
using System.Collections;

public class RunFx : IActorStatusEffect {
	private ActorStatus status;
	
	public float plusModifier1 = 0f;
	public float mulModifier2 = 1f;
	public float plusModifier3 = 0f;
	public float mulModifier4 = 1f;
	
	private bool isActivated = false;
	// Use this for initialization
	void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if(ControlSchemeInterface.instance.GetAxis(ControlAxis.RUN) > 0f){
			isActivated = true;	
		}
		else{
			isActivated = false;
		}	
	}
	
	public override bool IsDead(){
		return false;	
	}
	
	public override void OnAttach(ActorStatus _status){
//		Debug.Log("OnAttach");
		status = _status;
	}
	
	public override void OnApply(ActorStatus status){
//		Debug.Log ("OnApply");
		if(isActivated){
			status.WriteStatus().MoveSpeedModifiers[0] += plusModifier1;
			status.WriteStatus().MoveSpeedModifiers[1] *= mulModifier2;
			status.WriteStatus().MoveSpeedModifiers[2] += plusModifier3;
			status.WriteStatus().MoveSpeedModifiers[3] *= mulModifier4;
		}
//		Debug.Log (status.GetModifiers(ActorStatus.StatusType.MOVESPEED)[1]);
	}
	
	virtual public string GetName(){
		return "SpeedAreaModify";	
	}
}
