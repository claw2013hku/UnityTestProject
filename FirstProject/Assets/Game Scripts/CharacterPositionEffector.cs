using UnityEngine;
using System.Collections;
[RequireComponent (typeof(CharacterPositionEffectorComponent))]
[RequireComponent (typeof(CharAnimEff))]
[RequireComponent (typeof(CharAnimEffComp))]
[RequireComponent (typeof(ActorStatusComponent))]

public class CharacterPositionEffector: MonoBehaviour {
	private CharacterPositionEffectorComponent component;
	private CharAnimEffComp stateComponent;
	private CharAnimEff stateEff;
	private ActorStatusComponent statusComponent;
	private CharacterController charController;
	private Vector3 gravitationalVelocity;
	public Vector3 gravitationalAcceleration = new Vector3(0, -10, 0);
	
	//local logic
	private CollisionFlags collisionFlags;
	
	//remote logic
	public bool compensatePositionErrors = true;
	public bool compensateRotationErrors = true;
	public bool useResultantRotation = false;
	public float positionSmoothingLag = 0.45f;
	private Vector3 positionSmoothingVelocity;
	public float predictionErrorMinRange = 1f;
	public float predictionErrorMaxRange = 4f;
	public float rotationSmoothingLag = 0.45f;
	private float rotationSmoothingVelocity;
	public float predictionErrorMinAngle = 25f;
	public float predictionErrorMaxAngle = 90f;

	
	public bool showGUI = true;
	public SFSNetworkManager.Mode mode = SFSNetworkManager.Mode.LOCAL;
	
	// Use this for initialization
	void Start () 
	{
		component = GetComponent<CharacterPositionEffectorComponent>();
		stateComponent = GetComponent<CharAnimEffComp>();
		stateEff = GetComponent<CharAnimEff>();
		statusComponent = GetComponent<ActorStatusComponent>();
		charController = GetComponent<CharacterController>();
	}
		
	// Update is called once per frame
	void Update () 
	{
		Vector3 motion = Vector3.zero;
		//local logic
		if(SFSNetworkManager.Mode.LOCAL == mode || SFSNetworkManager.Mode.HOSTREMOTE == mode || SFSNetworkManager.Mode.PREDICT == mode){
			if(!charController.enabled){
				return;
			}	
			
			if(IsGrounded()){	
				if(component.MoveDirection.sqrMagnitude != 0 && 
					(stateComponent.StateInfoNameHash == stateEff.IdleAnimationNameHash
					|| stateComponent.StateInfoNameHash == stateEff.RunAnimationNameHash)){
					motion += component.MoveDirection.normalized * statusComponent.MoveSpeed * Time.deltaTime;
					transform.rotation = Quaternion.LookRotation(component.MoveDirection);
				}
			}
			
			motion *= statusComponent.MotionModifier1m;
			motion += statusComponent.MotionModifier2p;
			
			if(!IsGrounded()){
				gravitationalVelocity += gravitationalAcceleration * Time.deltaTime;
			}
			else{
				gravitationalVelocity = gravitationalAcceleration * Time.deltaTime;
			}
			motion += gravitationalVelocity;
			collisionFlags = charController.Move(motion);
		}
		
		//remote logic
		if(SFSNetworkManager.Mode.REMOTE == mode || SFSNetworkManager.Mode.PREDICT == mode){
			if(SFSNetworkManager.Mode.REMOTE == mode){
				transform.position = component.ResultantPosition;
				transform.rotation = component.ResultantQuaternion;
			}
			else{	//Compensate prediction errors
				if(compensatePositionErrors){
					float distanceSqr = (transform.position - component.ResultantPosition).sqrMagnitude;
					if(predictionErrorMinRange * predictionErrorMinRange <= distanceSqr && distanceSqr <= predictionErrorMaxRange * predictionErrorMaxRange){
						transform.position = Vector3.SmoothDamp(transform.position, component.ResultantPosition, ref positionSmoothingVelocity, positionSmoothingLag);	
					}
					else if (predictionErrorMaxRange * predictionErrorMaxRange < distanceSqr){
						transform.position = component.ResultantPosition;	
					}
					else{
						//use current transform
					}	
				}
				else{
					//use current transform	
				}
				
				if(compensateRotationErrors){
					float angleDiff = transform.localRotation.eulerAngles.y - component.ResultantQuaternion.eulerAngles.y;
					angleDiff = Mathf.Abs(angleDiff);
					angleDiff = angleDiff > 180f ? 360f - angleDiff : angleDiff;
					if(predictionErrorMinAngle <= angleDiff && angleDiff <= predictionErrorMaxAngle){
						transform.rotation = Quaternion.Euler(
						component.ResultantQuaternion.eulerAngles.x,
						Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, component.ResultantQuaternion.eulerAngles.y, ref rotationSmoothingVelocity, rotationSmoothingLag),
						component.ResultantQuaternion.eulerAngles.z);
					}
					else if (predictionErrorMaxAngle < angleDiff){
						transform.rotation = component.ResultantQuaternion;
					}
					else{
						//use local rotation
					}
				}
				else if (useResultantRotation){
					transform.rotation = component.ResultantQuaternion;	
				}
				else{
					//use local rotation	
				}
			}
		}
		
		if(SFSNetworkManager.Mode.LOCAL == mode || SFSNetworkManager.Mode.HOSTREMOTE == mode){
			component.ResultantPosition = transform.position;
			component.ResultantQuaternion = transform.rotation;
			component.ResultantVelocity = GetComponent<CharacterController>().velocity;
		}
	}
	
	void OnGUI(){
		if(!showGUI) return;
		GUILayout.Label("target direction: " + component.MoveDirection);
		GUILayout.Label("movespeed (velocity mag): " + GetComponent<CharacterController>().velocity.magnitude);
		GUILayout.Label("movespeed (movespeed var): " + (component.MoveDirection.sqrMagnitude > 0f ? statusComponent.MoveSpeed : 0f));
	}

	public bool IsGrounded () {
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
}
