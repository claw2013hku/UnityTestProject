using UnityEngine;
using System.Collections;

public class NetworkAnimation: MonoBehaviour {
	
	public double m_InterpolationBackTime = 0.1;
	public double m_ExtrapolationLimit = 0.5;
	
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public AnimationClip runAnimation;
	public AnimationClip jumpPoseAnimation;
	
	public float walkMaxAnimationSpeed = 0.75f;
	public float trotMaxAnimationSpeed = 1.0f;
	public float runMaxAnimationSpeed = 1.0f;
	public float jumpAnimationSpeed = 1.15f;
	public float landAnimationSpeed = 1.0f;
	
	private Animation _animation;
	private ThirdPersonTouchController.CharacterState _characterState;
	private float _velocityMagnitude;
	
	// The speed when walking
	public float walkSpeed = 2.0f;
	// after trotAfterSeconds of walking we trot with trotSpeed
	public float trotSpeed = 4.0f;
	// when pressing "Fire3" button (cmd) we start running
	public float runSpeed = 6.0f;
	
	public float inAirControlAcceleration = 3.0f;
	
	// How high do we jump when pressing jump and letting go immediately
	public float jumpHeight = 0.5f;
	
	// The gravity for the character
	public float gravity = 20.0f;
	// The gravity in controlled descent mode
	public float speedSmoothing = 10.0f;
	public float rotateSpeed = 500.0f;
	public float trotAfterSeconds = 3.0f;
	
	public bool canJump = true;
	
	private float jumpRepeatTime = 0.05f;
	private float jumpTimeout = 0.15f;
	private float groundedTimeout = 0.25f;
	
	// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
	private float lockCameraTimer = 0.0f;
	
	// The current move direction in x-z
	private Vector3 moveDirection = Vector3.zero;
	// The current vertical speed
	private float verticalSpeed = 0.0f;
	// The current x-z move speed
	private float moveSpeed = 0.0f;
	
	// The last collision flags returned from controller.Move
	private CollisionFlags collisionFlags; 
	
	// Are we jumping? (Initiated with jump button and not grounded yet)
	private bool jumping = false;
	private bool jumpingReachedApex = false;
	
	// Are we moving backwards (This locks the camera to not do a 180 degree spin)
	private bool movingBack = false;
	// Is the user pressing any keys?
	private bool isMoving = false;
	// When did the user start walking (Used for going into trot after a while)
	private float walkTimeStart = 0.0f;
	// Last time the jump button was clicked down
	private float lastJumpButtonTime = -10.0f;
	// Last time we performed a jump
	private float lastJumpTime = -1.0f;
	
	
	// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
	private float lastJumpStartHeight = 0.0f;
	
	
	private Vector3 inAirVelocity = Vector3.zero;
	
	private float lastGroundedTime = 0.0f;
	
	internal struct  State
	{
		internal double timestamp;
		internal ThirdPersonTouchController.CharacterState state;
		internal float velocityMagnitude;
	}
	
	// We store twenty states with "playback" information
	State[] m_BufferedState = new State[20];
	// Keep track of what slots are used
	int m_TimestampCount;
	
	void Awake(){
		moveDirection = transform.TransformDirection(Vector3.forward);
	
		_animation = GetComponent<Animation>();
		if(!_animation)
			Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
		
		/*
	public var idleAnimation : AnimationClip;
	public var walkAnimation : AnimationClip;
	public var runAnimation : AnimationClip;
	public var jumpPoseAnimation : AnimationClip;	
		*/
		if(!idleAnimation) {
			_animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}
		if(!walkAnimation) {
			_animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}
		if(!runAnimation) {
			_animation = null;
			Debug.Log("No run animation found. Turning off animations.");
		}
		if(!jumpPoseAnimation && canJump) {
			_animation = null;
			Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		// Send data to server
		if (stream.isWriting)
		{
			ThirdPersonTouchController.CharacterState state = GetComponent<ThirdPersonTouchController>().GetState();
			int serializedState = (int)state;
			
			float velocityMagnitude = GetComponent<ThirdPersonTouchController>().GetVelocityMagnitude();
			//Debug.Log("serialize, characterState : " + serializedState + ", vm : " + velocityMagnitude);
			
			stream.Serialize(ref serializedState);
			stream.Serialize(ref velocityMagnitude);
		}
		// Read data from remote client
		else
		{
			ThirdPersonTouchController.CharacterState _state = ThirdPersonTouchController.CharacterState.Idle;
			int serializedState = 0;
			float velocityMagnitude = 0f;
			
			stream.Serialize(ref serializedState);
			stream.Serialize(ref velocityMagnitude);
			_state = (ThirdPersonTouchController.CharacterState)serializedState;
			
			// Shift the buffer sideways, deleting state 20
			for (int i=m_BufferedState.Length-1;i>=1;i--)
			{
				m_BufferedState[i] = m_BufferedState[i-1];
			}
			
			// Record current state in slot 0
			State state;
			state.timestamp = info.timestamp;
			state.state = _state;
			state.velocityMagnitude = velocityMagnitude;
			m_BufferedState[0] = state;
			
			//Debug.Log("deserialize, characterState : " + serializedState + ", vm : " + velocityMagnitude);
			// Update used slot count, however never exceed the buffer size
			// Slots aren't actually freed so this just makes sure the buffer is
			// filled up and that uninitalized slots aren't used.
			m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

			// Check if states are in order, if it is inconsistent you could reshuffel or 
			// drop the out-of-order state. Nothing is done here
			for (int i=0;i<m_TimestampCount-1;i++)
			{
				if (m_BufferedState[i].timestamp < m_BufferedState[i+1].timestamp)
					Debug.Log("State inconsistent");
			}	
		}
	}
	
	// We have a window of interpolationBackTime where we basically play 
	// By having interpolationBackTime the average ping, you will usually use interpolation.
	// And only if no more data arrives we will use extra polation
	void Update () {
		// This is the target playback time of the rigid body
		double interpolationTime = Network.time - m_InterpolationBackTime;
		
		// Use interpolation if the target playback time is present in the buffer
		if (m_BufferedState[0].timestamp > interpolationTime)
		{
			// Go through buffer and find correct state to play back
			for (int i=0;i<m_TimestampCount;i++)
			{
				if (m_BufferedState[i].timestamp <= interpolationTime || i == m_TimestampCount-1)
				{
					// The state one slot newer (<100ms) than the best playback state
					State rhs = m_BufferedState[Mathf.Max(i-1, 0)];
					// The best playback state (closest to 100 ms old (default time))
					State lhs = m_BufferedState[i];
					
					// Use the time between the two slots to determine if interpolation is necessary
					double length = rhs.timestamp - lhs.timestamp;
					float t = 0.0F;
					// As the time difference gets closer to 100 ms t gets closer to 1 in 
					// which case rhs is only used
					// Example:
					// Time is 10.000, so sampleTime is 9.900 
					// lhs.time is 9.910 rhs.time is 9.980 length is 0.070
					// t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
					if (length > 0.0001)
						t = (float)((interpolationTime - lhs.timestamp) / length);
					
					// if t=0 => lhs is used directly
					//transform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
					//transform.localRotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);
					_characterState = rhs.state;
					_velocityMagnitude = Mathf.Lerp (lhs.velocityMagnitude, rhs.velocityMagnitude, t);
					//Debug.Log("interpolation result, characterState : " + _characterState + ", lhs : " + lhs.velocityMagnitude + ", rhs : " + rhs.velocityMagnitude);
					//Debug.Log("interpolation result, characterState : " + _characterState + ", vm : " + _velocityMagnitude);
			
					return;
				}
			}
		}
		// Use extrapolation
		else
		{
			State latest = m_BufferedState[0];
			
			float extrapolationLength = (float)(interpolationTime - latest.timestamp);
			// Don't extrapolation for more than 500 ms, you would need to do that carefully
			if (extrapolationLength < m_ExtrapolationLimit)
			{
				//float axisLength = extrapolationLength * latest.angularVelocity.magnitude * Mathf.Rad2Deg;
				//Quaternion angularRotation = Quaternion.AngleAxis(axisLength, latest.angularVelocity);
				
				_characterState = latest.state;// + latest.velocity * extrapolationLength;
				_velocityMagnitude = latest.velocityMagnitude;
				//transform.rotation = latest.rot;// angularRotation * latest.rot;
				//rigidbody.angularVelocity = latest.angularVelocity;
				Debug.Log("extrapolation result, characterState : " + _characterState + ", vm : " + _velocityMagnitude);
			}
		}
		Animate();
	}
	
	void Animate(){
		//Debug.Log("animation from network, _animation : " + (_animation != null) + " state : " + _characterState + " vm : " + _velocityMagnitude);
		// ANIMATION sector
		if(_animation) {
			if(_characterState == ThirdPersonTouchController.CharacterState.Jumping) 
			{
				if(!jumpingReachedApex) {
					_animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
					_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name);
				} else {
					_animation[jumpPoseAnimation.name].speed = -landAnimationSpeed;
					_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name);				
				}
			} 
			else 
			{
				if(_velocityMagnitude * _velocityMagnitude < 0.1f) {
					_animation.CrossFade(idleAnimation.name);
				}
				else 
				{
					if(_characterState == ThirdPersonTouchController.CharacterState.Running) {
						_animation[runAnimation.name].speed = Mathf.Clamp(_velocityMagnitude, 0.0f, runMaxAnimationSpeed);
						_animation.CrossFade(runAnimation.name);	
					}
					else if(_characterState == ThirdPersonTouchController.CharacterState.Trotting) {
						_animation[walkAnimation.name].speed = Mathf.Clamp(_velocityMagnitude, 0.0f, trotMaxAnimationSpeed);
						_animation.CrossFade(walkAnimation.name);	
					}
					else if(_characterState == ThirdPersonTouchController.CharacterState.Walking) {
						_animation[walkAnimation.name].speed = Mathf.Clamp(_velocityMagnitude, 0.0f, walkMaxAnimationSpeed);
						_animation.CrossFade(walkAnimation.name);	
					}
					
				}
			}
		}
		// ANIMATION sector
	}
}
