using UnityEngine;
using System.Collections;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;


public class ActorStatusRecp : MonoBehaviour {
	private ActorStatusComponent component;
	// Use this for initialization
	void Start () {
		component = GetComponent<ActorStatusComponent>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ReceiveStatus(ISFSObject sObj){
		ISFSObject statusObj = sObj.GetSFSObject(NetSyncObjCharacter.statusDS);
			
		if(statusObj.ContainsKey("currentHP")){
			component.HP = statusObj.GetFloat("currentHP");
			Debug.Log ("Receiving HP Change : " + component.HP);
		}
	}
}
