
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns player and items objects, stores them in collections and provides access to them
public class PlayerSpawner : MonoBehaviour {
	public GameObject enemyPrefab;
	public GameObject playerPrefab;
	
	private GameObject playerObj;
	
	private static PlayerSpawner instance;
	public static PlayerSpawner Instance {
		get {
			return instance;
		}
	}
	
	public GameObject GetPlayerObject() {
		return playerObj;
	}
	
	public GameObject tent1;
	public GameObject tent2;
	public GameObject trigger;
	public GameObject gate1;
	public GameObject gate2;
	
	private Dictionary<int, GameObject> recipients = new Dictionary<int, GameObject>();
	private Dictionary<int, GameObject> items = new Dictionary<int, GameObject>();
	private Dictionary<int, SlashHitTest> colliders = new Dictionary<int, SlashHitTest>();
	
	void Awake() {
		instance = this;
	}
	
	void Start(){
		if(tent1 != null){
			recipients.Add(-1, tent1);
			tent1.GetComponent<NetSyncObj>().ID = -1;
		}
		
		if(tent2 != null){
			recipients.Add(-2, tent2);
			tent2.GetComponent<NetSyncObj>().ID = -2;
		}
		
		if(trigger != null){
			recipients.Add(-3, trigger);
			trigger.GetComponent<NetSyncObj>().ID = -3;
		}
		
		if(gate1 != null){
			recipients.Add(-4, gate1);
			gate1.GetComponent<NetSyncObj>().ID = -4;
		}
		
		if(gate2 != null){
			recipients.Add(-5, gate2);
			gate2.GetComponent<NetSyncObj>().ID = -5;
		}
	}

	public void SpawnPlayer(int id, NetworkTransform ntransform, string name) {
//		if (Camera.main!=null) {
//			Destroy(Camera.main.gameObject);
//		}
			
		playerObj = GameObject.Instantiate(playerPrefab) as GameObject;
		playerObj.transform.position = ntransform.Position;
		playerObj.transform.localEulerAngles = ntransform.AngleRotationFPS;
		playerObj.SendMessage("StartSendTransform");			
	
		RemotePlayer remotePlayer = playerObj.GetComponent<RemotePlayer>();
		remotePlayer.Init(name);
		
		CameraMode cameraMode = Camera.main.GetComponent<CameraMode>();
		cameraMode.FocusTransform(playerObj);
		recipients[id] = playerObj;
		
		foreach(NetSyncObj nObj in playerObj.GetComponentsInChildren<NetSyncObj>(true)){
			Debug.Log ("has nObj");
			nObj.ID = id;
		}
		int cId = id;
		foreach(SlashHitTest hitTest in playerObj.GetComponentsInChildren<SlashHitTest>(true)){
			Debug.Log ("has hitTest");
			hitTest.networkId = cId;
			hitTest.sendMessage = true;
			colliders[cId] = hitTest;
			cId++;
		}
		Debug.Log("Assign player ID: " + id + ", collider id: " + cId);
	}
	
	public void SpawnEnemy(int id, NetworkTransform ntransform, string name) {
		GameObject playerObj = GameObject.Instantiate(enemyPrefab) as GameObject;
		playerObj.transform.position = ntransform.Position;
		playerObj.transform.localEulerAngles = ntransform.AngleRotationFPS;
				
		RemotePlayer remotePlayer = playerObj.GetComponent<RemotePlayer>();
		remotePlayer.Init(name);
		
		recipients[id] = playerObj;
		
		foreach(NetSyncObj nObj in playerObj.GetComponentsInChildren<NetSyncObj>(true)){
			nObj.ID = id;
		}
		int cId = id;
		foreach(SlashHitTest hitTest in playerObj.GetComponentsInChildren<SlashHitTest>(true)){
			hitTest.networkId = cId;
			hitTest.sendMessage = false;
			colliders[cId] = hitTest;
			cId++;
		}
		Debug.Log("Assign remote player ID: " + id + ", collider id: " + cId);
	}
	
	public GameObject GetRecipient(int id) {
		if (recipients.ContainsKey(id)) {
			return recipients[id];
		}
		return null;
	}
	
	public SlashHitTest GetCollider(int id){
		if(colliders.ContainsKey(id)){
			return colliders[id];	
		}
		return null;
	}
	
	public void DestroyEnemy(int id) {
		Debug.Log ("Destory remote player : " + id);

		GameObject rec = GetRecipient(id);
		if (rec == null) return;
		Destroy(rec);
		recipients.Remove(id);
	}
	
	public void SyncAnimation(int id, string msg, int layer) {
//		Debug.Log ("Synch player animation: " + id + ", " + msg + ", " + layer);
//
//		CharacterPositionReceptor rec = GetRecipient(id);
//		
//		if (rec == null) return;
		//rec.ReceiveAttackMove();
//		if (layer == 0) {
//			rec.GetComponent<AnimationSynchronizer>().RemoteStateUpdate(msg);
//		}
//		else if (layer == 1) {
//			rec.GetComponent<AnimationSynchronizer>().RemoteSecondStateUpdate(msg);
//		}
	}
}

