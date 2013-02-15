using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;

public abstract class NetSyncObj : MonoBehaviour {
	private int id;
	public int ID {get {return id;} set {id = value;}}
	
	public SFSNetworkManager.Mode mode = SFSNetworkManager.Mode.LOCAL;
	
	public virtual void HandleSync(ISFSObject obj){
		Debug.LogError("Unhandled sync msg");
	}
	
	public virtual void HandleInit(ISFSObject obj){
		Debug.LogError("Unhandled init msg");	
	}
	
	public virtual void HandleCollide(NetSyncObj obj){
		Debug.LogError("Unhandled collide msg");	
	}
}
