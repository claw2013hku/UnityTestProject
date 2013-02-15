
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
	
	public NetSyncObj tent1;
	public NetSyncObj tent2;
	public NetSyncObj trigger;
	public NetSyncObj gate1;
	public NetSyncObj gate2;
	
	private Dictionary<int, NetSyncObj> recipients = new Dictionary<int, NetSyncObj>();
	
	void Awake() {
		instance = this;
	}
	
	void Start(){
		if(tent1 != null){
			recipients.Add(-1, tent1);
			tent1.ID = -1;
		}
		
		if(tent2 != null){
			recipients.Add(-2, tent2);
			tent2.ID = -2;
		}
		
		if(trigger != null){
			recipients.Add(-3, trigger);
			trigger.ID = -3;
		}
		
		if(gate1 != null){
			recipients.Add(-4, gate1);
			gate1.ID = -4;
		}
		
		if(gate2 != null){
			recipients.Add(-5, gate2);
			gate2.ID = -5;
		}
	}

	public void SpawnPlayer(int id, NetworkTransform ntransform, string name) {	
		playerObj = GameObject.Instantiate(playerPrefab) as GameObject;
		playerObj.transform.position = ntransform.Position;
		playerObj.transform.localEulerAngles = ntransform.AngleRotationFPS;
		playerObj.SendMessage("StartSendTransform");			
	
		RemotePlayer remotePlayer = playerObj.GetComponent<RemotePlayer>();
		remotePlayer.Init(name);
		
		CameraMode cameraMode = Camera.main.GetComponent<CameraMode>();
		cameraMode.FocusTransform(playerObj);
		
		int assignId = id;
		foreach(NetSyncObj nObj in playerObj.GetComponentsInChildren<NetSyncObj>(true)){
			nObj.ID = assignId;
			recipients[assignId] = nObj;
			Debug.Log ("Assigned Id: " + assignId);
			assignId += 1000;
		}	
	}
	
	public void SpawnEnemy(int id, NetworkTransform ntransform, string name) {
		GameObject playerObj = GameObject.Instantiate(enemyPrefab) as GameObject;
		playerObj.transform.position = ntransform.Position;
		playerObj.transform.localEulerAngles = ntransform.AngleRotationFPS;
				
		RemotePlayer remotePlayer = playerObj.GetComponent<RemotePlayer>();
		remotePlayer.Init(name);
		
		int assignId = id;
		foreach(NetSyncObj nObj in playerObj.GetComponentsInChildren<NetSyncObj>(true)){
			nObj.ID = id;
			recipients[assignId] = nObj;
			Debug.Log ("Assigned Id: " + assignId);
			assignId += 1000;
		}
	}
	
	public NetSyncObj GetRecipient(int id) {
		if (recipients.ContainsKey(id)) {
			return recipients[id];
		}
		return null;
	}
	
	public void DestroyEnemy(int id) {
		Debug.Log ("Destory remote player : " + id);

		GameObject rec = GetRecipient(id).gameObject;
		if (rec == null) return;
		Destroy(rec);
		recipients.Remove(id);
	}
}

