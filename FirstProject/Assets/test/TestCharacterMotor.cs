using UnityEngine;
using System.Collections;

public class TestCharacterMotor: MonoBehaviour {
	public bool controllable = false;
	public float runSpeedModifier = 0f;
	
	private Animator animator;
	private string runAnimationName = "Base Layer.Run Blend Tree";
	private int runAnimationNameHash;
	private string idleAnimationName = "Base Layer.Idle";
	private int idleAnimationNameHash;
	
	private CharacterController charController;
	private ActorStatus status;
	
//	public bool ApplyGravity = true; 
//	public bool useGravity;
	
	private Vector3 targetDirection;
	
	private float[] runAnimationLookUpValues = {0.224f, 0.5f, 0.666f, 0.778f, 0.857f, 0.9165f, 0.963f, 1f};
	private float defaultRunAnimationVelocity = 5.299f;
	
	private CollisionFlags collisionFlags;
	
	public bool showGUI = true;
	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>();
		runAnimationNameHash = Animator.StringToHash(runAnimationName);
		idleAnimationNameHash = Animator.StringToHash(idleAnimationName);
		
		charController = GetComponent<CharacterController>();
		status = GetComponent<ActorStatus>();
		
		if(animator.layerCount >= 2)
			animator.SetLayerWeight(1, 1);
	}
		
	// Update is called once per frame
	void Update () 
	{
		if(!charController.enabled){
			return;
		}
		
		//also act as a run status effector
		if(ControlSchemeInterface.instance.GetAxis(ControlAxis.RUN) > 0f){
			status.GetModifiers(ActorStatus.StatusType.MOVESPEED)[0] += runSpeedModifier;	
		}
		
		Vector3 motion = Vector3.zero;
		
		if(controllable){
			Transform cameraTransform = Camera.main.transform;
			// Forward vector relative to the camera along the x-z plane	
			Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
			forward.y = 0f;
			forward = forward.normalized;
			
			// Right vector relative to the camera
			// Always orthogonal to the forward vector
			Vector3 right = new Vector3(forward.z, 0, -forward.x);
			
	  		float h = ControlSchemeInterface.instance.GetAxis(ControlAxis.MOVE_X);
	    	float v = ControlSchemeInterface.instance.GetAxis(ControlAxis.MOVE_Y);
			
			targetDirection = h * right + v * forward;	
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			animator.SetBool("Run", false);
			if(IsGrounded()){
				if(targetDirection.sqrMagnitude != 0 && (stateInfo.nameHash == idleAnimationNameHash || stateInfo.nameHash == runAnimationNameHash)){
					animator.SetBool("Run", true);
					motion += targetDirection.normalized * status.GetModifiedStatusf(ActorStatus.StatusType.MOVESPEED) * Time.deltaTime;
					transform.rotation = Quaternion.LookRotation(targetDirection);
				}
			}
		}
		
		//Debug.Log("MOtion0: " + motion);
		motion *= status.motionModifier1m;
		//Debug.Log("MOtion1: " + motion);
		motion += status.motionModifier2p;
		motion += new Vector3(0, -10, 0) * Time.deltaTime;
		//Debug.Log("MOtion2: " + motion);
//		if(useGravity){
//			if(!GetComponent<CharacterController>().isGrounded){
//				motion += new Vector3(0, -10, 0) * Time.deltaTime;
//			}
//		}
		collisionFlags = charController.Move(motion);
		//Debug.Log("IsGrounded: " + IsGrounded());
		
		if(animator){
			animator.SetFloat("Blended Speed", getRunAnimationSpeedValue(GetComponent<CharacterController>().velocity.magnitude));
			animator.SetFloat("Speed", GetComponent<CharacterController>().velocity.magnitude);
		}
		
        //animator.SetFloat("Direction", h, DirectionDampTime, Time.deltaTime);			  
	}
	
	void OnGUI(){
		if(!showGUI) return;
		GUILayout.Label("target direction: " + targetDirection);
		GUILayout.Label("movespeed (velocity mag): " + GetComponent<CharacterController>().velocity.magnitude);
		GUILayout.Label("movespeed (movespeed var): " + (targetDirection.sqrMagnitude > 0f ? status.GetModifiedStatusf(ActorStatus.StatusType.MOVESPEED) : 0f));
	}
	
	float getRunAnimationSpeedValue(float velocity){
//		if(velocity < defaultRunAnimationVelocity * 0.5f){
//			return 0;	
//		}
//		else if (velocity > defaultRunAnimationVelocity * 2f){
//			return 1;	
//		}
//		else{
//			float factor = velocity / (defaultRunAnimationVelocity * 2f);
//			return runAnimationLookUpValues[(int)(factor * 10 + 0.5f) - 3];
//		}
		
		float factor = velocity / (defaultRunAnimationVelocity * 2f);
		int index = (int)(factor * 10 + 0.5f) - 3;
		return runAnimationLookUpValues[Mathf.Clamp(index, 0, 7)];
	}
	
	public bool IsGrounded () {
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
}
