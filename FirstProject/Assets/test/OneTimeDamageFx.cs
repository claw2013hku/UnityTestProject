using UnityEngine;
using System.Collections;

public class OneTimeDamageFx : IActorStatusEffect {
	public float damage = 0f;
	public float stayTime = 0f;
	private float stayTimer = 0f; 
	private bool dead = false;
	
	public bool applyForceOnDeath = true;
	public float explosionForce = 0f;
	public Vector3 explosionPosition;
	public float explosionRadius;
	
	private ActorStatus status;
	private Animator animator;

	private RagdollTurner ragdollTurner;
	
	private bool damageDone = false;
	
	private bool animationDone = false;
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if(animationDone && status != null){
			AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0);
			if(nextState.nameHash == Animator.StringToHash("Base Layer.hit")){
				Debug.Log ("set animation hit false");
				animator.SetBool("Hit", false);
				animationDone = false;
			}
		}
		if(damageDone){
			stayTimer += Time.deltaTime;
			if(stayTimer >= stayTime){
				dead = true;
			}
		}
	}
	
	public override void OnAttach(ActorStatus status){
//		Debug.Log ("OnAttach");
		this.status = status;
		ragdollTurner = status.GetComponent<RagdollTurner>();
		animator = status.GetComponent<Animator>();
	}
	
	public override void OnApply(ActorStatus status){
		if(damageDone){
			return;	
		}
		damageDone = true;
		
		if(status.ReadStatus.HP <= 0f) return;
		if(GetComponent<NetSyncObj>().mode == SFSNetworkManager.Mode.LOCAL){
			status.WriteStatus().BaseHP -= damage;
			float pHp = status.WriteStatus().BaseHP;
			//pHp -= damage;
			//Debug.Log ("HP:" + status.WriteStatus().BaseHP);
			if(pHp < 0) pHp = 0;	
		}
		else{
			Debug.Log ("Remote damage fx");	
		}
		
		//if(status.GetModifiedStatusf(ActorStatus.StatusType.HP) <= 0f){
			if(applyForceOnDeath){
				ragdollTurner.ReadyExplosion(explosionPosition, explosionForce, explosionRadius);	
			}
			else{
				ragdollTurner.UnreadyExplosion();
			}	
		//}
		animator.SetBool("Hit", true);
		animationDone = true;
	}
	
	public override bool IsDead(){
		return dead && !animationDone;
	}
	
	public override string GetName(){
		return "DeathToRagdoll";	
	}
}
