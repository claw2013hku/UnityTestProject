using UnityEngine;
using System.Collections;

public class KnockbackFx : IActorStatusEffect {
	public AnimationCurve curve;
	public Vector3 initialMotion = Vector3.zero;
	
	private ActorStatus status;
	private bool dead;
	private bool attached = false;
	private float timer = 0f;
	private float endTime = 0f;
	private Vector3 motion = Vector3.zero;

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if(attached){
			timer += Time.deltaTime;
			if(timer >= endTime){
				Debug.Log ("dead");
				dead = true;	
			}
			else{
				motion = initialMotion * curve.Evaluate(timer);
			}
		}
	}
	
	public override void OnAttach(ActorStatus status){
		this.status = status;
		attached = true;
		endTime = curve[curve.length-1].time;
		Debug.Log ("end time: " + curve[curve.length-1].time);
	}
	
	public override void OnApply(ActorStatus status){
		status.WriteStatus().SetMotionModifier(0, motion);
		Debug.Log ("apply motion: " + motion);
	}
	
	public override bool IsDead(){
		return dead;
	}
	
	public override string GetName(){
		return "Knockback";	
	}
}
