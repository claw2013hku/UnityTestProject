using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindowPad : MonoBehaviour {
	public Rect normalizedBoundary;
	public bool autoPolling = false;
	
	private Rect hitWindow;
	
	public int tapCount;
	static public float tapTimeDelta = 0.3f;
	private float tapTimeWindow;
	
	public List<int> fingerIds = new List<int>();
	private Dictionary<int, Vector2> deltaFingerPositions = new Dictionary<int, Vector2>();
	private Dictionary<int, Vector2> currentFingerPositions = new Dictionary<int, Vector2>();
	private Dictionary<int, Vector2> lastFingerPositions = new Dictionary<int, Vector2>();
	private Dictionary<int, float> fingerEnterTimes = new Dictionary<int, float>();
	
	public List<Vector2> deltas = new List<Vector2>();
	public List<float> times = new List<float>();
	public int fingerCount = 0;
	
	public float sensitivityX = 1f;
	public float sensitivityY = 1f;
	
	// Use this for initialization
	void Start () {
		hitWindow = new Rect(
			normalizedBoundary.x * Screen.width,
			normalizedBoundary.y * Screen.height,
			normalizedBoundary.width * Screen.width, 
			normalizedBoundary.height * Screen.height);
	}
	
	// Update is called once per frame
	void Update () {
		deltas.Clear();
		times.Clear();
		foreach(KeyValuePair<int, Vector2> deltaKV in deltaFingerPositions){
			deltas.Add(deltaKV.Value);	
		}
		foreach(KeyValuePair<int, float> timeKV in fingerEnterTimes){
			times.Add(timeKV.Value);	
		}
		
		if(tapTimeWindow > 0){
			tapTimeWindow -= Time.deltaTime;
		}
		else{
			tapCount = 0;
		}	
		
		if (autoPolling){
			for(int i = 0; i < Input.touchCount; ++i){
				Poll(Input.GetTouch(i));
			}
		}
		fingerCount = GetFingerCount();
	}
	
	public bool Poll(Touch touch){
		bool hasPolled = false;
		//Debug.Log("hitWindow : " + hitWindow + " touch.position: " + touch.position);
		if(hitWindow.Contains(touch.position) && !fingerIds.Contains(touch.fingerId)){
			hasPolled = true;
			fingerIds.Add(touch.fingerId);
			deltaFingerPositions.Add (touch.fingerId, Vector2.zero);
			currentFingerPositions.Add (touch.fingerId, new Vector2(touch.position.x, touch.position.y));
			lastFingerPositions.Add (touch.fingerId, new Vector2(touch.position.x, touch.position.y));
			fingerEnterTimes.Add (touch.fingerId, Time.time);
		}
		if(fingerIds.Contains(touch.fingerId)){
			hasPolled = true;
			deltaFingerPositions[touch.fingerId] = new Vector2(touch.deltaPosition.x * sensitivityX * Screen.width, touch.deltaPosition.y * sensitivityY * Screen.height);
			//Debug.Log("deltaFingerPositions[touch.fingerId] : " + deltaFingerPositions[touch.fingerId]);
			if(touch.phase == TouchPhase.Canceled || 
				touch.phase == TouchPhase.Ended){
				RemoveFinger(touch.fingerId);	
			}
		}
		return hasPolled;
	}
	
	void RemoveFinger(int fingerId){
		fingerIds.Remove(fingerId);
		deltaFingerPositions.Remove(fingerId);
		currentFingerPositions.Remove(fingerId);
		lastFingerPositions.Remove(fingerId);
		if(Time.time - fingerEnterTimes[fingerId] < tapTimeDelta){
			if(tapTimeWindow > 0){
				tapCount++;	
			}
			else{
				tapCount = 1;
				tapTimeWindow = tapTimeDelta;
			}
		}
		fingerEnterTimes.Remove(fingerId);
	}
	
	public Dictionary<int, Vector2> GetDeltaPositions(){
		return deltaFingerPositions;	
	}
	
	public int GetFingerCount(){
		return fingerIds.Count;
	}
	
	public Vector2 GetAnyDeltaPositions(){
		return deltaFingerPositions[fingerIds[0]];
	}
	
	public List<int> GetFingerIds(){
		return fingerIds;
	}
}
