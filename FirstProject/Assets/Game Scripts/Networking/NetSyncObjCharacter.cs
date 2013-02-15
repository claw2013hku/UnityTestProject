using UnityEngine;
using System.Collections;

public class NetSyncObjCharacter : NetSyncObj {
	public readonly static string statusDS = "char_status";
	public readonly static string posDS = "char_pos";
	public readonly static string movDS = "char_mov";
	public readonly static string animDS = "char_anim";
	
	private ActorStatusRecp statusRecp;
	private CharPosRecp posRecp;
	private CharAnimRecp animRecp;
	// Use this for initialization
	void Awake () {
		statusRecp = GetComponent<ActorStatusRecp>();
		posRecp = GetComponent<CharPosRecp>();
		animRecp = GetComponent<CharAnimRecp>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override void HandleSync (Sfs2X.Entities.Data.ISFSObject obj)
	{
		//Debug.Log ("Character Handling Sync");
		if(mode == SFSNetworkManager.Mode.LOCAL) return;
		
		bool consumed = false;
		
		if(obj.ContainsKey(statusDS)){
			statusRecp.ReceiveStatus(obj);
			consumed = true;
		}
		
		if (obj.ContainsKey(posDS)){
			posRecp.ReceiveResultant(obj);
			consumed = true;
		}
		
		if (obj.ContainsKey(movDS)){
			posRecp.ReceiveMoveDirection(obj);
			consumed = true;
		}
		
		if (obj.ContainsKey(animDS)){
			Debug.Log("Received animation update");
			animRecp.ReceiveState(obj);
			consumed = true;
		}
		
		if(!consumed){
			Debug.LogError("Unhandled sync");	
		}
	}
	
	public override void HandleInit (Sfs2X.Entities.Data.ISFSObject obj)
	{
		bool consumed = false;
		
		if(obj.ContainsKey(statusDS)){
			statusRecp.ReceiveStatus(obj);
			consumed = true;
		}
		
		if (obj.ContainsKey(posDS)){
			posRecp.ReceiveResultant(obj, true);
			consumed = true;
		}
//		else if (obj.ContainsKey(movDS)){
//			posRecp.ReceiveMoveDirection(obj);
//		}
//		else if (obj.ContainsKey(animDS)){
//			animRecp.ReceiveState(obj);
//		}
		if(!consumed){
			Debug.LogError("Unhandled init");	
		}
	}
}
