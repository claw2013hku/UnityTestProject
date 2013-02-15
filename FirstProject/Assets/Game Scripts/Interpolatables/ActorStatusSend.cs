using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;

public class ActorStatusSend : MonoBehaviour {
	private ActorStatusComponent component;
	private NetSyncObjCharacter syncObj;
	
	private bool pendingSend = false;
	private bool sendHP = false;
	
	// Use this for initialization
	void Start () {
		component = GetComponent<ActorStatusComponent>();
		component.HasChangedStatus += HandleComponentHasChangedStatus;
		syncObj = GetComponent<NetSyncObjCharacter>();
	}

	void HandleComponentHasChangedStatus (bool isBase, ActorStatusComponent.StatusType type, float oldVal, float newVal)
	{
		if(!isBase){
			if(type == ActorStatusComponent.StatusType.HP){
				pendingSend = true;
				sendHP = true;	
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(pendingSend){
			SendStatusChange();
		}
	}
	
	void SendStatusChange(){
		pendingSend = false;
		ISFSObject data = new SFSObject();
		ISFSObject tr = new SFSObject();
		if(sendHP){
			Debug.Log("Sending status change: " + component.HP);
			tr.PutFloat("currentHP", component.HP);	
		}	
		data.PutSFSObject(NetSyncObjCharacter.statusDS, tr);
		data.PutInt("id", syncObj.ID);
		SFSNetworkManager.Instance.SendNetObjSync(data);
	}
}
