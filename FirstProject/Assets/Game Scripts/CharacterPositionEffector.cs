using UnityEngine;
using System.Collections;
[RequireComponent (typeof(CharacterPositionEffectorComponent))]
[RequireComponent (typeof(CharacterStateEffectorComponent))]
[RequireComponent (typeof(ActorStatusComponent))]

public class CharacterPositionEffector: MonoBehaviour {
	private CharacterPositionEffectorComponent component;
	private CharacterStateEffectorComponent stateComponent;
	private ActorStatusComponent statusComponent;
	private CharacterController charController;
	private Vector3 gravitationalVelocity;
	public Vector3 gravitationalAcceleration = new Vector3(0, -10, 0);
	
	//local logic
	private CollisionFlags collisionFlags;
	
	//remote logic
	public float positionSmoothingLag = 0.1f;
	private Vector3 positionSmoothingVelocity;
	public float rotationSmoothingLag = 0.1f;
	private float rotationSmoothingVelocity;
	
	public bool showGUI = true;
	public SFSNetworkManager.Mode mode = SFSNetworkManager.Mode.LOCAL;
	
	// Use this for initialization
	void Start () 
	{
		component = GetComponent<CharacterPositionEffectorComponent>();
		stateComponent = GetComponent<CharacterStateEffectorComponent>();
		statusComponent = GetComponent<ActorStatusComponent>();
		charController = GetComponent<CharacterController>();
	}
		
	// Update is called once per frame
	void Update () 
	{
		//local logic
		if(SFSNetworkManager.Mode.LOCAL == mode || SFSNetworkManager.Mode.HOSTREMOTE == mode || SFSNetworkManager.Mode.PREDICT == mode){
			if(!charController.enabled){
				return;
			}	
			Vector3 motion = Vector3.zero;
			
			if(IsGrounded()){	
				if(component.MoveDirection.sqrMagnitude != 0){// && 
					//(stateComponent.CurrentAnimationState == stateComponent.IdleAnimationNameHash 
					//|| stateComponent.CurrentAnimationState == stateComponent.RunAnimationNameHash)){
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
			//motion += new Vector3(0, -10, 0) * Time.deltaTime;
		
			collisionFlags = charController.Move(motion);
		}
		
		//remote logic
		
		if(SFSNetworkManager.Mode.REMOTE == mode || SFSNetworkManager.Mode.PREDICT == mode){
			if(SFSNetworkManager.Mode.REMOTE == mode){
				transform.position = component.ResultantPosition;
				transform.rotation = component.ResultantQuaternion;
			}
			else{
				transform.position = Vector3.SmoothDamp(transform.position, component.ResultantPosition, ref positionSmoothingVelocity, positionSmoothingLag);				
			}
		}
		
		if(SFSNetworkManager.Mode.LOCAL == mode || SFSNetworkManager.Mode.HOSTREMOTE == mode){
			component.ResultantPosition = transform.position;
			component.ResultantQuaternion = transform.rotation;
		}
	}
	
	void OnGUI(){
		if(!showGUI) return;
		GUILayout.Label("target direction: " + component.MoveDirection);
		GUILayout.Label("movespeed (velocity mag): " + GetComponent<CharacterController>().velocity.magnitude);
		GUILayout.Label("movespeed (movespeed var): " + (component.MoveDirection.sqrMagnitude > 0f ? statusComponent.MoveSpeed : 0f));
	}

	private bool IsGrounded () {
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
}
