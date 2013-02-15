using UnityEngine;
using System.Collections;

public class SlashHitTest : NetHitbox {
	public GameObject owner;
	
	public bool applyForceOnDeath = true;
	public Transform explosionPosition;
	public float explosionForce = 50f;
	public float explosionRadius = 3f;
	public float damage = 10f;
	
	public AnimationCurve motionCurve;
	public Transform motionOrigin;
	
	private ArrayList frameRegisteredColliders = new ArrayList();
	
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
		if(col.gameObject == owner || frameRegisteredColliders.Contains(col)) return;
		frameRegisteredColliders.Add(col);
		
		if(mode != SFSNetworkManager.Mode.LOCAL) return;
		
		NetSyncObj nObj = col.GetComponent<NetSyncObj>();
		if(nObj == null) return;
		
		Debug.Log ("Sending Trigger Enter Message, collider ID: " + ID + ", obj ID: " + nObj.ID);
		SFSNetworkManager.Instance.SendTriggerEnter(ID, nObj.ID);
	}
	
	public override void HandleCollide (NetSyncObj nObj){
		Debug.Log ("Received trigger enter message, collider ID: " + ID + ", obj ID: " + nObj.ID);
				
		ActorStatus status = nObj.GetComponent<ActorStatus>();
		if(status != null){
			OneTimeDamageFx fx = (OneTimeDamageFx) status.gameObject.AddComponent("OneTimeDamageFx");
			fx.applyForceOnDeath = applyForceOnDeath;
			fx.explosionPosition = explosionPosition.position;
			fx.explosionForce = explosionForce;
			fx.explosionRadius = explosionRadius;
			fx.damage = damage;
			
			KnockbackFx fx2 = (KnockbackFx) status.gameObject.AddComponent ("KnockbackFx");
			fx2.curve = motionCurve;
			fx2.initialMotion = (-motionOrigin.position + nObj.transform.position).normalized;
			
			status.AttachStatusEffects(fx, fx2);
		}
	}
}
