
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ingame GUI class 
public class PingDisplay : MonoBehaviour {
	void Awake() {
		Application.runInBackground = true;
	}
	
	void Start() {
	}
	
	void OnGUI() {
		// GUI.Label(new Rect(10, 10, 300, 20), "RMB - reload");	
		if (TimeManager.Instance == null) return;
		GUI.Label(new Rect(10, 10, 300, 20), "Time: "+TimeManager.Instance.NetworkTime);
		GUI.Label(new Rect(10, 30, 300, 20), "Ping: "+TimeManager.Instance.AveragePing);
	}	
}

