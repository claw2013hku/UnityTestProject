#define PCTEST
using UnityEngine;
using System.Collections;

public class BasicSiegeControlScheme : ControlSchemeInterface {
	public Joystick movementJoystick;
	public Joystick aimJoystick;
	public Joystick runJoystick;
	public WindowPad windowPad;
	
	public Vector2 aimJoystickDeltaSensitivity = new Vector2(1, 1);
	public Vector2 aimJoystickSensitivity = new Vector2(1, 1);
	public Vector2 mouseSenitivity = new Vector2(1,1);
	
	public float axis_throw = 0f;
	public float axis_camera_scroll_x = 0f;
	public float axis_camera_scroll_y = 0f;
	public float axis_move_x = 0f;
	public float axis_move_y = 0f;
	public float axis_aiming = 0f;
	public float axis_raw_x = 0f;
	public float axis_raw_y = 0f;
	
	public Vector2 joystickAtEdgeOffset = new Vector2(0.1f, 0.05f);
	public Vector2 joystickAtEdgePlusDelta = new Vector2(1f, 1f);
	
	// Use this for initialization
	void Start () {
		ControlSchemeInterface.instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		movementJoystick.FrameAdvance();
		aimJoystick.FrameAdvance();
		runJoystick.FrameAdvance();
		
		int count = Input.touchCount;
		for(int i = 0; i < count; ++i){
			if(!movementJoystick.Poll(Input.GetTouch(i))){// && !aimJoystick.Poll(Input.GetTouch(i))){
				windowPad.Poll(Input.GetTouch(i));
				runJoystick.Poll(Input.GetTouch(i));
			}
			aimJoystick.Poll(Input.GetTouch(i));
		}
		axis_throw = GetAxis(ControlAxis.THROW);
		axis_move_x = GetAxis(ControlAxis.MOVE_X);
		axis_move_y = GetAxis(ControlAxis.MOVE_Y);
		axis_camera_scroll_x = GetAxis(ControlAxis.CAMERA_SCROLL_X);
		axis_camera_scroll_y = GetAxis(ControlAxis.CAMERA_SCROLL_Y);
		axis_aiming = GetAxis(ControlAxis.AIMING);
		axis_raw_x = Input.GetAxisRaw("Horizontal");
		axis_raw_y = Input.GetAxisRaw("Vertical");
	}
	
	override public float GetAxis(ControlAxis axis){
		switch(axis){
//			case ControlAxis.THROW:
//#if UNITY_ANDROID || UNITY_IPHONE
//				return aimJoystick.IsJustUp() ? 1f : 0f;
//#else
//				return Input.GetButtonUp("Fire1")? 1f : 0f;
//#endif
			case ControlAxis.RUN:
#if PCTEST
				return Input.GetKey(KeyCode.LeftShift) ? 1f : 0f;
#elif UNITY_ANDROID || UNITY_IPHONE
				return runJoystick.IsDown() ? 1f : 0f;
#else
				return Input.GetKey(KeyCode.LeftShift) ? 1f : 0f;
#endif
			case ControlAxis.ATTACK1:
#if PCTEST
				return Input.GetButtonDown("Fire1")? 1f : 0f;
#elif UNITY_ANDROID || UNITY_IPHONE
				return aimJoystick.IsJustDown() ? 1f : 0f;
#else
				return Input.GetButton("Fire1")? 1f : 0f;
#endif
			case ControlAxis.CAMERA_SCROLL_X:
				float value = 0f;
#if PCTEST
				value = Input.GetAxis("Mouse X") * mouseSenitivity.x;
#elif UNITY_ANDROID || UNITY_IPHONE
				if(windowPad.GetDeltaPositions().Count == 1){
					value += windowPad.GetAnyDeltaPositions().x;
				}
				if(aimJoystick.joystickAtEdge.x == 1.0f){
					value += aimJoystick.position.x * aimJoystickSensitivity.x;
				}
#else
				value = Input.GetAxis("Mouse X") * mouseSenitivity.x;
#endif
				return value;
			
			case ControlAxis.CAMERA_SCROLL_Y:
				float value2 = 0f;
#if PCTEST
				value2 = Input.GetAxis("Mouse Y") * mouseSenitivity.y;
#elif UNITY_ANDROID || UNITY_IPHONE
				if(windowPad.GetDeltaPositions().Count == 1){
					value2 += windowPad.GetAnyDeltaPositions().y;
				}
				if(aimJoystick.joystickAtEdge.y == 1.0f){
					value2 += aimJoystick.position.y * aimJoystickSensitivity.y;
				}
#else
				value2 = Input.GetAxis("Mouse Y") * mouseSenitivity.y;
#endif
				return value2;

			case ControlAxis.MOVE_X:
#if PCTEST
				return Input.GetAxisRaw("Horizontal");
#elif UNITY_ANDROID || UNITY_IPHONE
				return movementJoystick.position.x;
#else
				return Input.GetAxisRaw("Horizontal");
#endif
			case ControlAxis.MOVE_Y:
#if PCTEST
				return Input.GetAxisRaw("Vertical");
#elif UNITY_ANDROID || UNITY_IPHONE
				return movementJoystick.position.y;
#else
				return Input.GetAxisRaw("Vertical");
#endif
//			case ControlAxis.AIMING:
//#if UNITY_ANDROID || UNITY_IPHONE
//				return aimJoystick.IsDown() ? 1f : 0f;
//#else
//				return Input.GetButton("Fire1")? 1f : 0f;
//#endif
			case ControlAxis.DEBUG:
				return Input.GetButtonDown("Fire2")? 1f : 0f;
		}
		return 0f;
	}
}
