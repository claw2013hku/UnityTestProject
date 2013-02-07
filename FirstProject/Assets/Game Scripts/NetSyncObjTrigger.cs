using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class NetSyncObjTrigger : NetSyncObj {
	public enum TriggerState{LEFT, MIDDLE, RIGHT};
	public TriggerState state = TriggerState.MIDDLE;
	public float animationSpeed = 1f;
	private Animation triggerAnimation;
	
	void Start () {
		triggerAnimation = animation;
		triggerAnimation.Play("turn_left");
	}
	
	// Update is called once per frame
	void Update () {
		Animate();
	}
	
	public override void HandleSync (ISFSObject obj)
	{
		if(obj.ContainsKey("trigger_state")){
			int newState = obj.GetInt("trigger_state");	
			switch(newState){
			case 0:
				state = TriggerState.LEFT;
				break;
			case 1:
				state = TriggerState.MIDDLE;
				break;
			case 2:
				state = TriggerState.RIGHT;
				break;
			}
		}
	}
	
	void Animate(){
		switch(state){
		case TriggerState.MIDDLE:
			if(0.45f <= triggerAnimation["turn_left"].normalizedTime && triggerAnimation["turn_left"].normalizedTime <= 0.55f){
				triggerAnimation["turn_left"].speed = 0f;	
			}
			else{
				if(triggerAnimation["turn_left"].normalizedTime > 0.5f && triggerAnimation["turn_left"].speed > 0f){
					triggerAnimation["turn_left"].speed = -animationSpeed;
				}
				if(triggerAnimation["turn_left"].normalizedTime < 0.5f && triggerAnimation["turn_left"].speed < 0f){
					triggerAnimation["turn_left"].speed = animationSpeed;		
				}
			}
			break;
		case TriggerState.LEFT:
			if(triggerAnimation["turn_left"].speed != animationSpeed){
				triggerAnimation["turn_left"].normalizedTime = Mathf.Clamp(triggerAnimation["turn_left"].normalizedTime, 0f, 1f);
				triggerAnimation["turn_left"].speed = animationSpeed;	
			}
			break;
		case TriggerState.RIGHT:
			if(triggerAnimation["turn_left"].speed != -animationSpeed){
				triggerAnimation["turn_left"].normalizedTime = Mathf.Clamp(triggerAnimation["turn_left"].normalizedTime, 0f, 1f);
				triggerAnimation["turn_left"].speed = -animationSpeed;	
			}
			break;
		}
	}
}