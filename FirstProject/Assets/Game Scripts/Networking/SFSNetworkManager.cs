using System;
using System.Collections;
using UnityEngine;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;

// The Neywork manager sends the messages to server and handles the response.
public class SFSNetworkManager : MonoBehaviour
{ 
	public CharPosRecp testRecipient;
	
	public enum Mode {LOCAL, HOSTREMOTE, REMOTE, PREDICT}
	
	private bool running = false;
	
	public readonly static string ExtName = "fps3";  // The server extension we work with
	public readonly static string ExtClass = "dk2.fullcontrol.fps.FpsExtension"; // The class name of the extension
	
	private static SFSNetworkManager instance;
	public static SFSNetworkManager Instance {
		get {
			return instance;
		}
	}
	
	private SmartFox smartFox;  // The reference to SFS client
	
	void Awake() {
		instance = this;	
	}
	
	void Start() {
		smartFox = SmartFoxConnection.Connection;
		if (smartFox == null) {
			Application.LoadLevel("lobby");
			return;
		}	
		
		SubscribeDelegates();
		SendSpawnRequest();
		
		TimeManager.Instance.Init();
		
		running = true;
	}
			
	// This is needed to handle server events in queued mode
	void FixedUpdate() {
		if (!running) return;
		smartFox.ProcessEvents();
	}
		
	private void SubscribeDelegates() {
		smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
		smartFox.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserLeaveRoom);
		smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
	}
	
	private void UnsubscribeDelegates() {
		smartFox.RemoveAllEventListeners();
	}
	
	public void SendSpawnRequest() {
		Room room = smartFox.LastJoinedRoom;
		ExtensionRequest request = new ExtensionRequest("spawnMe", new SFSObject(), room);
		Debug.Log ("Sending spawn request");
		smartFox.Send(request);
	}
	
	public void SendNetObjSync(ISFSObject data){
		Room room = smartFox.LastJoinedRoom;
		ExtensionRequest request = new ExtensionRequest("send_obj_sync", data, room); // True flag = UDP
		smartFox.Send(request);				
	}
	
	public void SendTriggerEnter(int colliderId, int targetId){
		Room room = smartFox.LastJoinedRoom;
		ISFSObject data = new SFSObject();
		
		ISFSObject tr = new SFSObject();
		
		tr.PutInt("colliderId", colliderId);
		tr.PutInt("targetId", targetId);
		tr.PutLong("t", Convert.ToInt64(0));
			
		data.PutSFSObject("collide_info", tr);
		
		ExtensionRequest request = new ExtensionRequest("sendTriggerEnter", data, room);
		smartFox.Send(request);
	}
	
	public void HandleNetObjSync(ISFSObject data){
//		Debug.Log ("Handling obj sync");
		int id = data.GetInt("id");
		NetSyncObj recipient = PlayerSpawner.Instance.GetRecipient(id);
		if(recipient != null){
			recipient.HandleSync(data);	
		}
//		Debug.Log ("Handle obj sync, id: " + id);
	}
	
	public void HandleNetObjInit(ISFSObject data){
		Debug.Log ("Handling obj init");
		int id = data.GetInt("id");
		NetSyncObj recipient = PlayerSpawner.Instance.GetRecipient(id);
		if(recipient != null){
			recipient.HandleInit(data);	
		}
	}
	/// <summary>
	/// Request the current server time. Used for time synchronization
	/// </summary>	
	public void TimeSyncRequest() {
		Room room = smartFox.LastJoinedRoom;
		ExtensionRequest request = new ExtensionRequest("getTime", new SFSObject(), room);
		smartFox.Send(request);
	}
	
	/// <summary>
	/// When connection is lost we load the login scene
	/// </summary>
	private void OnConnectionLost(BaseEvent evt) {
		UnsubscribeDelegates();
		Screen.lockCursor = false;
		Screen.showCursor = true;
		Application.LoadLevel("lobby");
	}
	
	// This method handles all the responses from the server
	private void OnExtensionResponse(BaseEvent evt) {
//		try {
			string cmd = (string)evt.Params["cmd"];
			ISFSObject dt = (SFSObject)evt.Params["params"];
			if (cmd == "spawnPlayer") {
				HandleInstantiatePlayer(dt);
			}
			else if (cmd == "time") {
				HandleServerTime(dt);
			}
			else if (cmd == "triggerEnter"){
				HandleTriggerEnter(dt);	
			}
			else if (cmd == "obj_sync"){
				HandleNetObjSync(dt);
			}
			else if (cmd == "obj_init"){
				HandleNetObjInit(dt);
			}
			else if (cmd == "obj_destory"){
				HandleDestoryObject(dt);	
			}
//		}
//		catch (Exception e) {
//			Debug.Log("Exception handling response: "+e.Message+" >>> "+e.StackTrace);
//		}
	}
	
	// Instantiating player (our local FPS model, or remote 3rd person model)
	private void HandleInstantiatePlayer(ISFSObject dt) {
		int userId = dt.GetInt("owner");
		int id = dt.GetInt("id");
						
		User user = smartFox.UserManager.GetUserById(userId);
		string name = user.Name;
		
		if (userId == smartFox.MySelf.Id) {
			PlayerSpawner.Instance.SpawnPlayer(id, name, dt);
			Debug.Log ("Handle spawn request (local), user ID: " + userId + ", id: " + id);
		}
		else {
			PlayerSpawner.Instance.SpawnEnemy(id, name, dt);
			Debug.Log ("Handle spawn request (remote), user ID: " + userId + ", id: " + id);
		}
	}
	
	private void HandleDestoryObject(ISFSObject dt){
		int id = dt.GetInt("id");
		PlayerSpawner.Instance.DestroyEnemy(id);
		Debug.Log ("Destroying object id: " + id);
	}
	
	private void HandleTriggerEnter(ISFSObject dt){
		ISFSObject sObj = dt.GetSFSObject("collide_info");
		
		int colliderId = sObj.GetInt("colliderId");
		int targetId = sObj.GetInt ("targetId");
		
	 	NetSyncObj recipient = PlayerSpawner.Instance.GetRecipient(colliderId);
		NetSyncObj obj = PlayerSpawner.Instance.GetRecipient(targetId);
		if(recipient != null && obj != null){
			recipient.HandleCollide(obj);
		}
		else{
			Debug.LogError("network trigger enter error, collider: " + recipient + ", target: " + obj);
		}
	}
	
	// Synchronize the time from server
	private void HandleServerTime(ISFSObject dt) {
		long time = dt.GetLong("t");
		TimeManager.Instance.Synchronize(Convert.ToDouble(time));
	}
	
	// When a user leaves room destroy his object
	private void OnUserLeaveRoom(BaseEvent evt) {
		User user = (User)evt.Params["user"];
//		Room room = (Room)evt.Params["room"];
//				
//		PlayerSpawner.Instance.DestroyEnemy(user.Id);
		Debug.Log("User "+user.Name+" left");
	}
	
	void OnApplicationQuit() {
		UnsubscribeDelegates();
	}
}
