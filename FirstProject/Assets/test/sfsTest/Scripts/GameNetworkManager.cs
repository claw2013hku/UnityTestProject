using System;
using System.Collections;
using UnityEngine;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;

// Sends and receive client or host messages
public class GameNetworkManager : MonoBehaviour
{
	public readonly static string ExtName = "Siege";  // The server extension we work with
	public readonly static string ExtClass = "claw.Siege1Extension"; // The class name of the extension
	
	private static GameNetworkManager instance;
	public static GameNetworkManager Instance {
		get {
			return instance;
		}
	}
	
	private bool running = false;
	public bool IsRunning {
		get{
			return running;	
		}
	}
	
	private SmartFox smartFox;  // The reference to SFS client
	
	void Awake(){
		instance = this;	
	}
	
	void Start(){
		smartFox = SmartFoxConnection.Connection;
		if (smartFox == null) {
			Debug.Log ("not connected to server");
			return;
		}
//		SubscribeDelegates();
//		TimeManager.Instance.Init();
	}
	
//	private void SubscribeDelegates() {
//		smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
//		smartFox.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserLeaveRoom);
//		smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
//	}
}
