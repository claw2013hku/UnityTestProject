using UnityEngine;
using System.Collections;

public class SlashHitTest : IHitBox {
	public GameObject owner;
	
	public bool applyForceOnDeath = true;
	public Transform explosionPosition;
	public float explosionForce = 50f;
	public float explosionRadius = 3f;
	public float damage = 10f;
	
	public AnimationCurve motionCurve;
	public Transform motionOrigin;
	
	private ArrayList frameRegisteredColliders = new ArrayList();
	
	public int networkId;
	public bool sendMessage;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(frameRegisteredColliders.Count > 0){
			frameRegisteredColliders.Clear();
		}
	}
	
	void OnTriggerEnter(Collider col){
		Debug.Log ("Damage Trigger Enter");
		if(col.gameObject == owner || frameRegisteredColliders.Contains(col)) return;
		frameRegisteredColliders.Add(col);
		
		if(!sendMessage) return;
		
		NetSyncObj nObj = col.GetComponent<NetSyncObj>();
		if(nObj == null) return;
		
		Debug.Log ("Sending Trigger Enter Message, collider ID: " + networkId + ", obj ID: " + nObj.ID);
		SFSNetworkManager.Instance.SendTriggerEnter(networkId, nObj.ID);
//		
//		ActorStatus status = col.GetComponent<ActorStatus>();
//		if(status != null){
//			//Debug.Log ("damageTrigger");
//			OneTimeDamageFx fx = (OneTimeDamageFx) status.gameObject.AddComponent("OneTimeDamageFx");
//			fx.applyForceOnDeath = applyForceOnDeath;
//			fx.explosionPosition = explosionPosition.position;
//			fx.explosionForce = explosionForce;
//			fx.explosionRadius = explosionRadius;
//			fx.damage = damage;
//			
//			KnockbackFx fx2 = (KnockbackFx) status.gameObject.AddComponent ("KnockbackFx");
//			fx2.curve = motionCurve;
//			fx2.initialMotion = (-motionOrigin.position + col.transform.position).normalized;
//			
//			status.AttachStatusEffects(fx, fx2);
//		}
	}
	
	public void OnReceiveTriggerEnter(GameObject target){
		NetSyncObj nObj = target.GetComponent<NetSyncObj>();
		if(nObj == null) return;
		
		Debug.Log ("Received trigger enter message, collider ID: " + networkId + ", obj ID: " + nObj.ID);
				
		ActorStatus status = target.GetComponent<ActorStatus>();
		if(status != null){
			//Debug.Log ("damageTrigger");
			OneTimeDamageFx fx = (OneTimeDamageFx) status.gameObject.AddComponent("OneTimeDamageFx");
			fx.applyForceOnDeath = applyForceOnDeath;
			fx.explosionPosition = explosionPosition.position;
			fx.explosionForce = explosionForce;
			fx.explosionRadius = explosionRadius;
			fx.damage = damage;
			
			KnockbackFx fx2 = (KnockbackFx) status.gameObject.AddComponent ("KnockbackFx");
			fx2.curve = motionCurve;
			fx2.initialMotion = (-motionOrigin.position + target.transform.position).normalized;
			
			status.AttachStatusEffects(fx, fx2);
		}
	}
}
