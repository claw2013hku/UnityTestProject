using UnityEngine;
using System.Collections;

public class runTest: MonoBehaviour {


	protected Animator animator;
	public float DirectionDampTime = .25f;
	public bool ApplyGravity = true; 
	
	private CharacterController charController;
	
	public float LowestMoveSpeed = 1f;
	public float HighestMoveSpeed = 1f;
	private float currentMoveSpeed = 1f;
	public float refreshTime = 1f;
	private float refreshTimer = 0f;
	
	public bool useGravity;
	
	private Vector3 targetDirection;
	
	private float[] runAnimationLookUpValues = {0.224f, 0.5f, 0.666f, 0.778f, 0.857f, 0.9165f, 0.963f, 1f};
	private float defaultRunAnimationVelocity = 5.299f;
	
	public bool showGUI = true;
	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>();
		charController = GetComponent<CharacterController>();
		
		if(animator.layerCount >= 2)
			animator.SetLayerWeight(1, 1);
		
		currentMoveSpeed = UnityEngine.Random.Range(LowestMoveSpeed, HighestMoveSpeed);
	}
		
	// Update is called once per frame
	void Update () 
	{
		if(!charController.enabled){
			return;
		}
		
		refreshTimer += Time.deltaTime;
		if(refreshTimer > refreshTime){
			refreshTimer = 0f;	
			currentMoveSpeed = UnityEngine.Random.Range(LowestMoveSpeed, HighestMoveSpeed);
		}
	
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
		Vector3 motion = targetDirection.normalized * currentMoveSpeed * Time.deltaTime;
		if(useGravity){
			if(!GetComponent<CharacterController>().isGrounded){
				motion += new Vector3(0, -10, 0) * Time.deltaTime;
			}
		}
		GetComponent<CharacterController>().Move(motion);
		
		if(targetDirection.sqrMagnitude != 0f){
			transform.rotation = Quaternion.LookRotation(targetDirection);
		}
		
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
		GUILayout.Label("movespeed (movespeed var): " + (targetDirection.sqrMagnitude > 0f ? currentMoveSpeed : 0f));
		GUILayout.Label("Time before change speed: " + (refreshTime - refreshTimer));
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
}
