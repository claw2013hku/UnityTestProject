using UnityEngine;
using System.Collections;
[RequireComponent (typeof(GUITexture))]

public class Joystick : MonoBehaviour {
	private GUITexture gui;
	private Rect defaultRect;
	private Vector2 guiTouchOffset;
	private Boundary guiBoundary = new Boundary();
	private Vector2 guiCenter;
	
	static private float tapTimeDelta = 0.3f;
	private float tapTimeWindow;
	private int lastFingerId = -1;
	
	public Vector2 position = Vector2.zero;
	public Vector2 deadZone = Vector2.zero;
	public int tapCount;
	public bool normalize;
	public bool autoPolling;
	public bool isJustDown = false;
	public bool isJustUp = false;
	
	static Joystick[] joysticks;
	static private bool enumeratedJoysticks = false;
	
	// Use this for initialization
	void Start () {
		gui = GetComponent<GUITexture>();
		//get where the gui texture was originally placed
		defaultRect = gui.pixelInset;
		// get our offset for center instead of corner
		guiTouchOffset.x = defaultRect.width * 0.5f;
		guiTouchOffset.y = defaultRect.height * 0.5f;
		
		guiBoundary.min.x = defaultRect.x - guiTouchOffset.x;
		guiBoundary.min.y = defaultRect.y - guiTouchOffset.y;
		guiBoundary.max.x = defaultRect.x + guiTouchOffset.x;
		guiBoundary.max.y = defaultRect.y + guiTouchOffset.y;
		
		guiCenter.x = defaultRect.x + guiTouchOffset.x;
		guiCenter.y = defaultRect.y + guiTouchOffset.y;
	}
	
	// set our gui texture back to the original location
	void Reset() {
		gui.pixelInset = defaultRect;	
		lastFingerId = -1;
	}
	
	// Update is called once per frame
	void Update () {
		if(!enumeratedJoysticks){
			joysticks = (Joystick[])FindObjectsOfType(typeof(Joystick));
			enumeratedJoysticks = true;
		}
		
		int count = Input.touchCount;
		
		if(tapTimeWindow > 0){
			tapTimeWindow -= Time.deltaTime;
		}
		else{
			tapCount = 0;
		}	
		
		if(autoPolling){
			FrameAdvance();	
		}
		
		if(count == 0){
			Reset();	
		}
		else if (autoPolling){
			// account for the offset in our calculations
			for(int i = 0; i < count; ++i){
				Poll(Input.GetTouch(i));
			}
		}
		
		position.x = (gui.pixelInset.x + guiTouchOffset.x - guiCenter.x) / guiTouchOffset.x;
		position.y = (gui.pixelInset.y + guiTouchOffset.y - guiCenter.y) / guiTouchOffset.y;
		float absoluteX = Mathf.Abs(position.x);
		float absoluteY = Mathf.Abs(position.y);
		
		if(absoluteX < deadZone.x){
			position.x = 0;	
		}
		else if (normalize){
			position.x = Mathf.Sign(position.x) * (absoluteX - deadZone.x) / (1 - deadZone.x);
		}
		
		if(absoluteY < deadZone.y){
			position.y = 0;	
		}
		else if(normalize){
			position.y = Mathf.Sign(position.y) * (absoluteY - deadZone.y) / (1 - deadZone.y);	
		}
		if(IsJustDown()){
			Debug.Log("IsJustDown()");
		}
		if(isJustUp){
			Debug.Log("IsJustUp()");
		}
		//Debug.Log("IsJustDown() : " + IsJustDown() + " IsJustUp() : " + IsJustUp());
	}
	
	void LatchedFinger(int fingerId){
		if(fingerId == lastFingerId){	
			Reset ();	
		}
	}
	
	void Disable(){
		gameObject.active = false;
		enumeratedJoysticks = false;
	}
	
	protected class Boundary{
		public Vector2 min = Vector2.zero;
		public Vector2 max = Vector2.zero;
	}	
	
	public void FrameAdvance(){
		isJustDown = false;
		isJustUp = false;
	}
	
	public bool Poll(Touch touch){
		bool hasPolled = false;
		// account for the offset in our calculations
		if(gui.HitTest(touch.position) && 
			lastFingerId == -1){
			lastFingerId = touch.fingerId;
			
			hasPolled = true;
			if(tapTimeWindow > 0){
				tapCount++;	
			}
			else{
				tapCount = 1;
				tapTimeWindow = tapTimeDelta;
			}
			
			foreach(Joystick j in joysticks){
				if(j != this){
					j.LatchedFinger(touch.fingerId);	
				}
			}
			isJustDown = true;
		}
		else if(lastFingerId == touch.fingerId){
//					if(touch.tapCount > tapCount){
//						tapCount = touch.tapCount;	
//					}
			hasPolled = true;
			Vector2 guiTouchPos = touch.position - guiTouchOffset;
			Rect pixelInset = gui.pixelInset;
			pixelInset.x = Mathf.Clamp(guiTouchPos.x, guiBoundary.min.x, guiBoundary.max.x);
			pixelInset.y = Mathf.Clamp(guiTouchPos.y, guiBoundary.min.y, guiBoundary.max.y);
			gui.pixelInset = pixelInset;
			
			// another check to see if fingers are touching
			if(touch.phase == TouchPhase.Canceled || 
				touch.phase == TouchPhase.Ended){
				Reset ();	
				isJustUp = true;
			}
		}
		return hasPolled;
	}

	public bool IsJustDown(){
		return isJustDown;	
	}
	
	public bool IsJustUp(){
		return isJustUp;
	}
	
	public bool IsDown(){
		return lastFingerId != -1;
	}
}


