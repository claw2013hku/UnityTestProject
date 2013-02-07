using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;


public class NetSyncObjGate : NetSyncObj {
	public bool open = true;
	public GameObject gate;
	private Animation gateAnimation;
	
	public override void HandleSync (ISFSObject obj)
	{
		if(obj.ContainsKey("gate_state")){
			int newState = obj.GetInt("gate_state");	
			open = (newState == 0) ? true : false;
		}
	}
	
	public void Start () {
		gateAnimation = gate.animation;
		gateAnimation.Play("gate_open");
	}
	
	// Update is called once per frame
	void Update () {
		Animate();
	}
	
	void Animate(){
		if(!gateAnimation.IsPlaying("gate_open")){
			gateAnimation.Play("gate_open");	
		}
		if(open){
			if(gateAnimation["gate_open"].speed != 1){
				gateAnimation["gate_open"].normalizedTime = Mathf.Clamp(gateAnimation["gate_open"].normalizedTime, 0f, 1f);
				gateAnimation["gate_open"].speed = 1;
			}
		}
		else {
			if(gateAnimation["gate_open"].speed != -1){
				gateAnimation["gate_open"].normalizedTime = Mathf.Clamp(gateAnimation["gate_open"].normalizedTime, 0f, 1f);
				gateAnimation["gate_open"].speed = -1;	
			}
		}
	}
}
