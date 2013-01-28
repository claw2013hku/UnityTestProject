
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
	
	private Dictionary<int, NetworkTransformReceiver> recipients = new Dictionary<int, NetworkTransformReceiver>();
	private Dictionary<int, GameObject> items = new Dictionary<int, GameObject>();
		
	void Awake() {
		instance = this;
	}

	public void SpawnPlayer(NetworkTransform ntransform, string name) {
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
	}
	
	public void SpawnEnemy(int id, NetworkTransform ntransform, string name) {
		
		GameObject playerObj = GameObject.Instantiate(enemyPrefab) as GameObject;
		playerObj.transform.position = ntransform.Position;
		playerObj.transform.localEulerAngles = ntransform.AngleRotationFPS;
				
		RemotePlayer remotePlayer = playerObj.GetComponent<RemotePlayer>();
		remotePlayer.Init(name);
		
		recipients[id] = playerObj.GetComponent<NetworkTransformReceiver>();
	}
	
	public NetworkTransformReceiver GetRecipient(int id) {
		if (recipients.ContainsKey(id)) {
			return recipients[id];
		}
		return null;
	}
	
	public void DestroyEnemy(int id) {
		Debug.Log ("Destory remote player : " + id);

		NetworkTransformReceiver rec = GetRecipient(id);
		if (rec == null) return;
		Destroy(rec.gameObject);
		recipients.Remove(id);
	}
	
	public void SyncAnimation(int id, string msg, int layer) {
		Debug.Log ("Synch player animation: " + id + ", " + msg + ", " + layer);

		NetworkTransformReceiver rec = GetRecipient(id);
		
		if (rec == null) return;
		rec.ReceiveAttackMove();
//		if (layer == 0) {
//			rec.GetComponent<AnimationSynchronizer>().RemoteStateUpdate(msg);
//		}
//		else if (layer == 1) {
//			rec.GetComponent<AnimationSynchronizer>().RemoteSecondStateUpdate(msg);
//		}
	}
}

