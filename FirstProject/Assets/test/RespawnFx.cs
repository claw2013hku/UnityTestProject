using UnityEngine;
using System.Collections;

public class RespawnFx : IActorStatusEffect {
	private bool startDeathTimer = true;
	public float deathTime = 1f;
	private float deathTimer = 0f;
	private bool revive = false;
	private RagdollTurner ragdollTurner;

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if(startDeathTimer){
			deathTimer += Time.deltaTime;
			if(deathTimer >= deathTime){
				revive = true;
			}
		}
	}
	
	public override void OnAttach(ActorStatus status){
//		Debug.Log ("OnAttach");
		ragdollTurner = status.GetComponent<RagdollTurner>();
	}
	
	public override void OnApply(ActorStatus status){
		float hp = status.GetModifiedStatusf(ActorStatus.StatusType.HP);
		if(hp <= 0f && !startDeathTimer){
			startDeathTimer = true;
		}
		if(hp > 0f && startDeathTimer){
			startDeathTimer = false;
			deathTimer = 0f;
		}
		if(revive){
			revive = false;
			status.baseHP = 1;
			ragdollTurner.UnturnRagdoll();
		}
	}
	
	public override bool IsDead(){
		return false;
	}
	
	public override string GetName(){
		return "Respawn";	
	}
}
