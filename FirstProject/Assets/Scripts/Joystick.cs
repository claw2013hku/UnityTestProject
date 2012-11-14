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
	
	public bool joystickAtEdgeCompensate = true;
	public Vector2 joystickAtEdgeOffset = new Vector2(0.1f, 0.05f);
	public Vector2 realNormalizedPosition = Vector2.zero;
	public bool realPositionValid = false;
	public Vector2 joystickAtEdge = Vector2.zero;
	
	static Joystick[] joysticks;
	static private bool enumeratedJoysticks = false;
	
	// Use this for initialization
	void Start () {
		gui = GetComponent<GUITexture>();
		float width = gui.pixelInset.width * Screen.width;
		gui.pixelInset = new Rect(
			gui.pixelInset.x * Screen.width - width / 2, 
			gui.pixelInset.y * Screen.height - width / 2, 
			width, 
			width);
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
		bool xAtEdge = false;
		bool yAtEdge = false;
		joystickAtEdge.x = 0f;
		joystickAtEdge.y = 0f;
		if(joystickAtEdgeCompensate && realPositionValid){
			if(realNormalizedPosition.x < joystickAtEdgeOffset.x || 1 - joystickAtEdgeOffset.x < realNormalizedPosition.x){
				xAtEdge = true;
				position.x = realNormalizedPosition.x < joystickAtEdgeOffset.x ? -1f : 1f;
				joystickAtEdge.x = 1.0f;
			}
			
			if(realNormalizedPosition.y < joystickAtEdgeOffset.y || 1 - joystickAtEdgeOffset.y < realNormalizedPosition.y){
				yAtEdge = true;
				position.y = realNormalizedPosition.y < joystickAtEdgeOffset.y ? -1f : 1f;
				joystickAtEdge.y = 1.0f;
			}
		}
		
		if(!xAtEdge){
			position.x = (gui.pixelInset.x + guiTouchOffset.x - guiCenter.x) / guiTouchOffset.x;
			float absoluteX = Mathf.Abs(position.x);
			if(absoluteX < deadZone.x){
				position.x = 0;	
			}
			else if (normalize){
				position.x = Mathf.Sign(position.x) * (absoluteX - deadZone.x) / (1 - deadZone.x);
			}
		}
		
		if(!yAtEdge){
			position.y = (gui.pixelInset.y + guiTouchOffset.y - guiCenter.y) / guiTouchOffset.y;
			float absoluteY = Mathf.Abs(position.y);
			if(absoluteY < deadZone.y){
				position.y = 0;	
			}
			else if(normalize){
				position.y = Mathf.Sign(position.y) * (absoluteY - deadZone.y) / (1 - deadZone.y);	
			}
		}
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
			lastFingerId == -1 && touch.phase == TouchPhase.Began){
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
			//Debug.Log("IsJustDown");
			realNormalizedPosition = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
			isJustDown = true;
			realPositionValid = true;
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
				//Debug.Log("IsJustUp");
				isJustUp = true;
				realPositionValid = false;
			}
			realNormalizedPosition = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
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


