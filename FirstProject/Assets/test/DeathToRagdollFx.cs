using UnityEngine;
using System.Collections;

public class DeathToRagdollFx : IActorStatusEffect {
	private bool isAliveLastFrame = true;
	private RagdollTurner ragdollTurner;
	public GameObject[] shutdownChildren;
	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override void OnAttach(ActorStatus status){
//		Debug.Log ("OnAttach");
		ragdollTurner = status.GetComponent<RagdollTurner>();
	}
	
	public override void OnApply(ActorStatus status){
		float hp = status.GetModifiedStatusf(ActorStatus.StatusType.HP);
		if(isAliveLastFrame){
			if(hp <= 0f){
				foreach(GameObject gameObj in shutdownChildren){
					if(gameObj.activeSelf){
						gameObj.SetActive(false);	
					}
				}
				ragdollTurner.TurnRagdoll();	
			}
		}
		isAliveLastFrame = hp > 0f;
	}
	
	public override bool IsDead(){
		return false;
	}
	
	public override string GetName(){
		return "DeathToRagdoll";	
	}
}
