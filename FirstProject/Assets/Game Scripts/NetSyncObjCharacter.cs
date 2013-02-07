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
	void Start () {
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
		
		if(obj.ContainsKey(statusDS)){
			statusRecp.ReceiveStatus(obj);
		}
		else if (obj.ContainsKey(posDS)){
			posRecp.ReceiveResultant(obj);
		}
		else if (obj.ContainsKey(movDS)){
			posRecp.ReceiveMoveDirection(obj);
		}
		else if (obj.ContainsKey(animDS)){
			animRecp.ReceiveState(obj);
		}
		else{
			Debug.LogError("Unhandled sync");	
		}
	}
}
