using UnityEngine;
using System.Collections;

public class NetworkTransformInterpolator : MonoBehaviour {
	public double m_InterpolationBackTime = 0.1;
	public double m_ExtrapolationLimit = 0.5;
	
	private Vector3 resultantPosition;
	public Vector3 ResultantPosition{ get {return resultantPosition;}}
	private Quaternion resultantRotation;
	public Quaternion ResultantRotation{ get{return resultantRotation;}}
	
	void Start(){
	}
	
	// We store twenty states with "playback" information
	NetworkTransform[] m_BufferedState = new NetworkTransform[20];
	// Keep track of what slots are used
	int m_TimestampCount;
	
	public void ReceivedTransform(NetworkTransform ntransform) {	
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
					resultantPosition = Vector3.Lerp(lhs.Position, rhs.Position, t);
					resultantRotation = Quaternion.Slerp(lhs.Rotation, rhs.Rotation, t);
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
				
				resultantPosition = latest.Position;// + latest.velocity * extrapolationLength;
				resultantRotation = latest.Rotation;// angularRotation * latest.rot;
			}
		}
	}
}
