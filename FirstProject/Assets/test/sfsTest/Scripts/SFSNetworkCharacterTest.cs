using UnityEngine;
using System.Collections;

public class SFSNetworkCharacterTest : MonoBehaviour {
	
	public double m_InterpolationBackTime = 0.1;
	public double m_ExtrapolationLimit = 0.5;
	
	protected Animator animator;
	public IHitBox slashHitbox;
	
	private string idleAnimationName = "Base Layer.Idle";
	private int idleAnimationNameHash;
	
	private string runAnimationName = "Base Layer.Run Blend Tree";
	private int runAnimationNameHash;
	
	private string slash1AnimationName = "Slash.Slash1";
	private int slash1AnimationNameHash;
	
	private string slash2AnimationName = "Slash.Slash2";
	private int slash2AnimationNameHash;
	
	private float[] runAnimationLookUpValues = {0.224f, 0.5f, 0.666f, 0.778f, 0.857f, 0.9165f, 0.963f, 1f};
	private float defaultRunAnimationVelocity = 5.299f;
	
	private Vector3 lastPosition = Vector3.zero;
	
	public float velocitySmooth = 0.8f;
	private float velocity = 0f;
	
	private bool running = false;
	
	void Start(){
		animator = GetComponent<Animator>();
		idleAnimationNameHash = Animator.StringToHash(idleAnimationName);
		runAnimationNameHash = Animator.StringToHash(runAnimationName);
		slash1AnimationNameHash = Animator.StringToHash(slash1AnimationName);
		slash2AnimationNameHash = Animator.StringToHash(slash2AnimationName);	
	}
	
	// We store twenty states with "playback" information
	NetworkTransform[] m_BufferedState = new NetworkTransform[20];
	// Keep track of what slots are used
	int m_TimestampCount;
	
	// We call it on remote player to start receiving his transform
	public void StartReceiving() {
		running = true;
	}
	
	public void ReceivedTransform(NetworkTransform ntransform) {
		if (!running) return;
			
		// Shift the buffer sideways, deleting state 20
		for (int i=m_BufferedState.Length-1;i>=1;i--)
		{
			m_BufferedState[i] = m_BufferedState[i-1];
		}
		
		// Record current state in slot 0
		m_BufferedState[0] = ntransform;
		
		// Update used slot count, however never exceed the buffer size
		// Slots aren't actually freed so this just makes sure the buffer is
		// filled up and that uninitalized slots aren't used.
		m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

		// Check if states are in order, if it is inconsistent you could reshuffel or 
		// drop the out-of-order state. Nothing is done here
		for (int i=0;i<m_TimestampCount-1;i++)
		{
			if (m_BufferedState[i].TimeStamp < m_BufferedState[i+1].TimeStamp)
				Debug.Log("State inconsistent");
		}
	}
	
	
	// We have a window of interpolationBackTime where we basically play 
	// By having interpolationBackTime the average ping, you will usually use interpolation.
	// And only if no more data arrives we will use extra polation
	void Update () {
		if (!running) return;
		if (m_TimestampCount == 0) return;
		// This is the target playback time of the rigid body
		double interpolationTime = (TimeManager.Instance.NetworkTime / 1000f) - m_InterpolationBackTime;
		
		// Use interpolation if the target playback time is present in the buffer
		if ((m_BufferedState[0].TimeStamp / 1000f) > interpolationTime)
		{
			//Debug.Log("OnInterpolate");
			// Go through buffer and find correct state to play back
			for (int i=0;i<m_TimestampCount;i++)
			{
				if (m_BufferedState[i].TimeStamp / 1000f <= interpolationTime || i == m_TimestampCount-1)
				{
					// The state one slot newer (<100ms) than the best playback state
					NetworkTransform rhs = m_BufferedState[Mathf.Max(i-1, 0)];
					// The best playback state (closest to 100 ms old (default time))
					NetworkTransform lhs = m_BufferedState[i];
					
					// Use the time between the two slots to determine if interpolation is necessary
					double length = rhs.TimeStamp / 1000f - lhs.TimeStamp / 1000f;
					float t = 0.0F;
					// As the time difference gets closer to 100 ms t gets closer to 1 in 
					// which case rhs is only used
					// Example:
					// Time is 10.000, so sampleTime is 9.900 
					// lhs.time is 9.910 rhs.time is 9.980 length is 0.070
					// t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
					if (length > 0.0001)
						t = (float)((interpolationTime - lhs.TimeStamp / 1000f) / length);
					
					// if t=0 => lhs is used directly
					transform.position = Vector3.Lerp(lhs.Position, rhs.Position, t);
					transform.rotation = Quaternion.Slerp(lhs.Rotation, rhs.Rotation, t);
					Debug.Log("inter transform: " + transform);
//					attacked = lhs.swinging;
					break;
				}
			}
		}
		// Use extrapolation
		else
		{
			NetworkTransform latest = m_BufferedState[0];
			
			float extrapolationLength = (float)(interpolationTime - latest.TimeStamp / 1000f);
			// Don't extrapolation for more than 500 ms, you would need to do that carefully
			if (extrapolationLength < m_ExtrapolationLimit)
			{
				//float axisLength = extrapolationLength * latest.angularVelocity.magnitude * Mathf.Rad2Deg;
				//Quaternion angularRotation = Quaternion.AngleAxis(axisLength, latest.angularVelocity);
				
				transform.position = latest.Position;// + latest.velocity * extrapolationLength;
				transform.rotation = latest.Rotation;// angularRotation * latest.rot;
				Debug.Log("extra transform: " + transform);
//					a
				//				attacked = latest.swinging;
				//rigidbody.angularVelocity = latest.angularVelocity;
			}
		}
		SynchVelocity();
		SynchAttack();
		lastPosition = transform.position;
	}
	
	public bool attackPending = false;
	void SynchAttack(){
		if (animator && animator.enabled)
		{
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

			if(stateInfo.nameHash == idleAnimationNameHash || stateInfo.nameHash == runAnimationNameHash){
				if(attackPending){
					Debug.Log ("start swinging");
					animator.SetBool("Slash", true);
					animator.SetInteger("SlashVariant", UnityEngine.Random.Range(1, 3));
				}
				DeactivateHitBoxes();
			}
			else if(stateInfo.nameHash == slash1AnimationNameHash || stateInfo.nameHash == slash2AnimationNameHash){
				if(animator.GetFloat("SlashHit") > 0f){
					if(!slashHitbox.activated){
						slashHitbox.Activate(true);	
					}
				}
				else{
					if(slashHitbox.activated){
						slashHitbox.Activate(false);	
					}
				}
				if(attackPending && stateInfo.normalizedTime > 0.6f){
					animator.SetBool("Slash", true);
					animator.SetInteger("SlashVariant", UnityEngine.Random.Range(1, 3));
				}
			}
			
			AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);
			if(nextStateInfo.nameHash == idleAnimationNameHash || nextStateInfo.nameHash == runAnimationNameHash){

			}
			else if(nextStateInfo.nameHash == slash1AnimationNameHash || nextStateInfo.nameHash == slash2AnimationNameHash){
				animator.SetBool("Slash", false);
				animator.SetInteger("SlashVariant", 0);
				attackPending = false;
			}
		}
	}
	
	void SynchVelocity(){
		if(animator){
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			animator.SetBool("Run", false);
			
			velocity = Mathf.Lerp(velocity, (transform.position - lastPosition).magnitude / Time.deltaTime, velocitySmooth);
			if(velocity > 0f){
				Debug.Log ("Controll velocity:" + velocity + "; distance: " + (transform.position - lastPosition).magnitude + "; Time step: " + Time.deltaTime);
				if((stateInfo.nameHash == idleAnimationNameHash || stateInfo.nameHash == runAnimationNameHash)){
					animator.SetBool("Run", true);
				}
			}
			animator.SetFloat("Blended Speed", getRunAnimationSpeedValue(velocity));
			animator.SetFloat("Speed", velocity);
		}
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
	
	void DeactivateHitBoxes(){
		if(slashHitbox.activated){
			slashHitbox.Activate(false);	
		}
	}
}