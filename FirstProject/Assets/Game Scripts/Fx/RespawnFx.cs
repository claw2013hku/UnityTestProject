using UnityEngine;
using System.Collections;

public class RespawnFx : IActorStatusEffect {
	private bool startDeathTimer = true;
	public float deathTime = 1f;
	private float deathTimer = 0f;
	private bool revive = false;
	private RagdollTurner ragdollTurner;
	
	private float lastFrameHP = 0f;
	// Use this for initialization
	protected override void Start () {
		base.Start();
		lastFrameHP = GetComponent<ActorStatusComponent>().HP;
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<NetSyncObj>().mode == SFSNetworkManager.Mode.LOCAL){
			if(startDeathTimer){
				deathTimer += Time.deltaTime;
				if(deathTimer >= deathTime){
					revive = true;
				}
			}	
		}
		else{
			lastFrameHP = GetComponent<ActorStatusComponent>().HP;
		}
	}
	
	public override void OnAttach(ActorStatus status){
//		Debug.Log ("OnAttach");
		ragdollTurner = status.GetComponent<RagdollTurner>();
	}
	
	public override void OnApply(ActorStatus status){
		float hp = status.ReadStatus.HP;
		if(GetComponent<NetSyncObj>().mode == SFSNetworkManager.Mode.LOCAL){
			if(hp <= 0f && !startDeathTimer){
			startDeathTimer = true;
			}
			if(hp > 0f && startDeathTimer){
				startDeathTimer = false;
				deathTimer = 0f;
			}
			if(revive){
				revive = false;
				status.WriteStatus().BaseHP = 1;
				ragdollTurner.UnturnRagdoll();
			}	
		}
		else{
			if(lastFrameHP <= 0 && hp > 0){
				Debug.Log ("unturn ragdoll");
				ragdollTurner.UnturnRagdoll();
			}	
		}
	}
	
	public override bool IsDead(){
		return false;
	}
	
	public override string GetName(){
		return "Respawn";	
	}
}
