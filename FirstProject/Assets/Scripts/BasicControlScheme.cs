using UnityEngine;
using System.Collections;

public class BasicControlScheme : ControlSchemeInterface {
	public Joystick movementJoystick;
	public Joystick aimJoystick;
	public WindowPad windowPad;
	
	public Vector2 aimJoystickDeltaSensitivity = new Vector2(1, 1);
	public Vector2 aimJoystickSensitivity = new Vector2(1, 1);
	
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
		
		int count = Input.touchCount;
		for(int i = 0; i < count; ++i){
			if(!movementJoystick.Poll(Input.GetTouch(i))){// && !aimJoystick.Poll(Input.GetTouch(i))){
				windowPad.Poll(Input.GetTouch(i));
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
			case ControlAxis.THROW:
				return aimJoystick.IsJustUp() ? 1f : 0f;
			case ControlAxis.CAMERA_SCROLL_X:
				float value = 0f;
				if(windowPad.GetDeltaPositions().Count == 1){
					value += windowPad.GetAnyDeltaPositions().x;
				}
				if(aimJoystick.joystickAtEdge.x == 1.0f){
					value += aimJoystick.position.x * aimJoystickSensitivity.x;
				}
//				else
//					return aimJoystick.position.x * aimJoystickSensitivity.x + aimJoystick.positionDelta.x * aimJoystickDeltaSensitivity.x;
				
				return value;
			case ControlAxis.CAMERA_SCROLL_Y:
				float value2 = 0f;
				if(windowPad.GetDeltaPositions().Count == 1){
					value2 += windowPad.GetAnyDeltaPositions().y;
				}
				if(aimJoystick.joystickAtEdge.y == 1.0f){
					value2 += aimJoystick.position.y * aimJoystickSensitivity.y;
				}
				return value2;
//				else
//					return aimJoystick.position.y * aimJoystickSensitivity.y + aimJoystick.positionDelta.y * aimJoystickDeltaSensitivity.y;
			case ControlAxis.MOVE_X:
				return movementJoystick.position.x;
			case ControlAxis.MOVE_Y:
				return movementJoystick.position.y;
			case ControlAxis.AIMING:
				return aimJoystick.IsDown() ? 1f : 0f;
		}
		return 0f;
	}
}
