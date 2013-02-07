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
	
	/// <summary>
	/// Send the request to server to spawn my player
	/// </summary>
	public void SendSpawnRequest() {
		Room room = smartFox.LastJoinedRoom;
		ExtensionRequest request = new ExtensionRequest("spawnMe", new SFSObject(), room);
		smartFox.Send(request);
	}
	
//	/// <summary>
//	///  Send a request to shoot
//	/// </summary>
//	public void SendShot() {
//		Room room = smartFox.LastJoinedRoom;
//		ExtensionRequest request = new ExtensionRequest("shot", new SFSObject(), room);
//		smartFox.Send(request);
//	}
//	
//	public void SendReload() {
//		Room room = smartFox.LastJoinedRoom;
//		ExtensionRequest request = new ExtensionRequest("reload", new SFSObject(), room);
//		smartFox.Send(request);
//	}
	/// <summary>
	/// Send local transform to the server
	/// </summary>
	/// <param name="ntransform">
	/// A <see cref="NetworkTransform"/>
	/// </param>
//	public void SendTransform(NetworkTransform ntransform) {
//		Room room = smartFox.LastJoinedRoom;
//		ISFSObject data = new SFSObject();
//		ntransform.ToSFSObject(data);
//		ExtensionRequest request = new ExtensionRequest("sendTransform", data, room, true); // True flag = UDP
//		smartFox.Send(request);
//	}
	
//	//Send Resultant Position Info
//	public void SendCharacterPositionResultant(CharPosEffComp.NetworkResultant result){
//		Room room = smartFox.LastJoinedRoom;
//		ISFSObject data = new SFSObject();
//		CharPosEffComp.ToSFSObject(result, data);
//		ExtensionRequest request = new ExtensionRequest("sendCharacterPositionResultant", data, room, true); // True flag = UDP
//		smartFox.Send(request);	
//	}
//	
//	//Send Control Movement Info
//	public void SendCharacterPositionMovement(CharPosEffComp.NetworkMoveDirection result){
//		Room room = smartFox.LastJoinedRoom;
//		ISFSObject data = new SFSObject();
//		CharPosEffComp.ToSFSObject(result, data);
//		ExtensionRequest request = new ExtensionRequest("sendCharacterPositionMovement", data, room, true); // True flag = UDP
//		smartFox.Send(request);
//	}
	
	//Send Animation State Info
//	public void SendAnimCompState(CharAnimEffComp.NetworkResultantState result){
//		Room room = smartFox.LastJoinedRoom;
//		ISFSObject data = new SFSObject();
//	 	result.ToSFSObject(data);
//		ExtensionRequest request = new ExtensionRequest("sendCharAnimCompState", data, room); // True flag = UDP
//		smartFox.Send(request);		
//	}
	
	public void SendNetObjSync(ISFSObject data){
		Room room = smartFox.LastJoinedRoom;
		ExtensionRequest request = new ExtensionRequest("send_obj_sync", data, room); // True flag = UDP
		smartFox.Send(request);				
//		Debug.Log ("Sending obj sync");
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
	
//	public void SendActorStatus(ISFSObject data){
//		Room room = smartFox.LastJoinedRoom;
//		ExtensionRequest request = new ExtensionRequest("sendActorStatus", data, room);
//		smartFox.Send(request);
//	}
	
//	public void HandleActorStatus(ISFSObject data){
//		Debug.Log ("handling actor status msg");
//		int userId = data.GetInt("id");
//		GameObject recipient = PlayerSpawner.Instance.GetRecipient(userId);
//		if(recipient != null){
//			if(recipient.GetComponent<ActorStatusRecp>() != null){
//				recipient.GetComponent<ActorStatusRecp>().ReceiveStatus(data);		
//			}
//		}
//	}
	
	public void HandleNetObjSync(ISFSObject data){
		Debug.Log ("Handling obj sync");
		int userId = data.GetInt("id");
		GameObject recipient = PlayerSpawner.Instance.GetRecipient(userId);
		if(recipient != null){
			recipient.GetComponent<NetSyncObj>().HandleSync(data);	
		}
	}
	
//	/// <summary>
//	/// Send local animation state to the server
//	/// </summary>
//	/// <param name="message">
//	/// A <see cref="System.String"/>
//	/// </param>
//	/// <param name="layer">
//	/// A <see cref="System.Int32"/>
//	/// </param>
//	public void SendAnimationState(string message, int layer) {
//		Room room = smartFox.LastJoinedRoom;
//		ISFSObject data = new SFSObject();
//		data.PutUtfString("msg", message);
//		data.PutInt("layer", layer);
//		ExtensionRequest request = new ExtensionRequest("sendAnim", data, room);
//		smartFox.Send(request);
//	}
//	
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
		try {
			string cmd = (string)evt.Params["cmd"];
			ISFSObject dt = (SFSObject)evt.Params["params"];
//			Debug.Log ("CMD: " + cmd);				
			if (cmd == "spawnPlayer") {
				HandleInstantiatePlayer(dt);
			}
//			else if (cmd == "transform") {
//				//HandleTransform(dt);
//			}
//			else if (cmd == "notransform") {
//				HandleNoTransform(dt);
//			}
//			else if (cmd == "anim") {
//				HandleAnimation(dt);
//			}
//			else if (cmd == "enemyShotFired") {
//				HandleShotFired(dt);
//			}
			else if (cmd == "time") {
				HandleServerTime(dt);
			}
//			else if (cmd == "reloaded"){
//				HandleReload(dt);
//			}
//			else if (cmd == "character_position_resultant"){
//				HandleCharacterPositionResultant(dt);
//			}
//			else if (cmd == "character_position_movement"){
//				HandleCharacterPositionMovement(dt);
//			}
//			else if (cmd == "charAnimCompState"){
//				HandleCharAnimCompState(dt);	
//			}
			else if (cmd == "triggerEnter"){
				HandleTriggerEnter(dt);	
			}
//			else if (cmd == "charStatus"){
//				HandleActorStatus(dt);	
//			}
			else if (cmd == "obj_sync"){
				HandleNetObjSync(dt);
			}
		}
		catch (Exception e) {
			Debug.Log("Exception handling response: "+e.Message+" >>> "+e.StackTrace);
		}
	}
	
	// Instantiating player (our local FPS model, or remote 3rd person model)
	private void HandleInstantiatePlayer(ISFSObject dt) {
		ISFSObject playerData = dt.GetSFSObject("player");
		int userId = playerData.GetInt("id");
		NetworkTransform ntransform = NetworkTransform.FromSFSObject(playerData);
						
		User user = smartFox.UserManager.GetUserById(userId);
		string name = user.Name;
		
		if (userId == smartFox.MySelf.Id) {
			PlayerSpawner.Instance.SpawnPlayer(userId, ntransform, name);
		}
		else {
			PlayerSpawner.Instance.SpawnEnemy(userId, ntransform, name);
		}                                                                                                                                                                                                                                                                                                                                                                                                      
	}
	
//	private void HandleCharacterPositionResultant(ISFSObject dt){
//		int userId = dt.GetInt("id");
//		CharPosEffComp.NetworkResultant result = CharPosEffComp.ResultantFromSFSObject(dt);
//		
//		GameObject recipient = PlayerSpawner.Instance.GetRecipient(userId);
//		if(recipient != null){
//			recipient.GetComponent<CharPosRecp>().ReceiveResultant(result);
//		}
//		
//		if(testRecipient != null){
//			testRecipient.GetComponent<CharPosRecp>().ReceiveResultant(result);	
//		}
//	}
//	
//	private void HandleCharacterPositionMovement(ISFSObject dt){
//		int userId = dt.GetInt("id");
//		CharPosEffComp.NetworkMoveDirection result = CharPosEffComp.MoveDirFromSFSObject(dt);
//		GameObject recipient = PlayerSpawner.Instance.GetRecipient(userId);
//		if(recipient != null){
//			recipient.GetComponent<CharPosRecp>().ReceiveMoveDirection(result);
//		}
//	}
//	
//	private void HandleCharAnimCompState(ISFSObject dt){
//		int userId = dt.GetInt("id");
//		CharAnimEffComp.NetworkResultantState result = CharAnimEffComp.NetworkResultantState.FromSFSObject(dt);
//		CharAnimRecp recipient = PlayerSpawner.Instance.GetRecipient(userId).GetComponent<CharAnimRecp>();
////		Debug.Log ("Receive Anim State: " + result.nameHash + ", slash: " + result.state.Slash + ", slashV: " + result.state.SlashVariant);
//		if(recipient != null){
//			recipient.ReceiveState(result);
//		}
//		else{
//			Debug.LogError("no recipient found");			
//		}
//		
//		if(testRecipient != null){
//			if(testRecipient.GetComponent<CharAnimRecp>() != null){
//				testRecipient.GetComponent<CharAnimRecp>().ReceiveState(result);	
//			}
//		}
//	}
	
	private void HandleTriggerEnter(ISFSObject dt){
		ISFSObject sObj = dt.GetSFSObject("collide_info");
		
		int colliderId = sObj.GetInt("colliderId");
		int targetId = sObj.GetInt ("targetId");
		
	 	SlashHitTest recipient = PlayerSpawner.Instance.GetCollider(colliderId);
		GameObject obj = PlayerSpawner.Instance.GetRecipient(targetId);
		if(recipient != null && obj != null){
			recipient.OnReceiveTriggerEnter(obj);
		}
		else{
			Debug.LogError("network trigger enter error, collider: " + recipient + ", target: " + obj);
		}
	}
	
//	// Server rejected transform message - force the local player object to what server said
//	private void HandleNoTransform(ISFSObject dt) {
//		int userId = dt.GetInt("id");
//		NetworkTransform ntransform = NetworkTransform.FromSFSObject(dt);
//		
//		if (userId == smartFox.MySelf.Id) {
//			// Movement restricted!
//			// Update transform of the local object
//			//ntransform.Update(PlayerSpawner.Instance.GetPlayerObject().transform);
//		}
//	}
	
	// Synchronize the time from server
	private void HandleServerTime(ISFSObject dt) {
		long time = dt.GetLong("t");
		TimeManager.Instance.Synchronize(Convert.ToDouble(time));
	}
	
//	// Synchronizing remote animation
//	private void HandleAnimation(ISFSObject dt) {
//		int userId = dt.GetInt("id");
//		string msg = dt.GetUtfString("msg");
//		int layer = dt.GetInt("layer");
//		
//		if (userId != smartFox.MySelf.Id) {
//			PlayerSpawner.Instance.SyncAnimation(userId, msg, layer);
//		}
//	}
//	
//	// When someon shots handle it and play corresponding animation 
//	private void HandleShotFired(ISFSObject dt) {
//		int userId = dt.GetInt("id");
//		if (userId != smartFox.MySelf.Id) {
//			PlayerSpawner.Instance.SyncAnimation(userId, "Shot", 1);
//		}
//		else {
//		}
//	}
//	
//	// When someone reloaded the weapon - play corresponding animation
//	private void HandleReload(ISFSObject dt) {
//		Debug.Log ("Handle Reload");
//		int userId = dt.GetInt("id");
//		if (userId != smartFox.MySelf.Id) {
////			SoundManager.Instance.PlayReload(PlayerManager.Instance.GetRecipient(userId).audio);
//			PlayerSpawner.Instance.SyncAnimation(userId, "Reload", 1);
//		}
//		else {
////			GameObject obj = PlayerManager.Instance.GetPlayerObject();
////			if (obj == null) return;
////			
////			SoundManager.Instance.PlayReload(obj.audio);
////			obj.GetComponent<AnimationSynchronizer>().PlayReloadAnimation();
//		}
//	}
	
	// When a user leaves room destroy his object
	private void OnUserLeaveRoom(BaseEvent evt) {
		User user = (User)evt.Params["user"];
		Room room = (Room)evt.Params["room"];
				
		PlayerSpawner.Instance.DestroyEnemy(user.Id);
		Debug.Log("User "+user.Name+" left");
	}
	
	void OnApplicationQuit() {
		UnsubscribeDelegates();
	}
}
