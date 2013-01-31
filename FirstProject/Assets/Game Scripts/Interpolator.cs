using UnityEngine;
using System.Collections;

public class Interpolator<T> where T: Interpolatable<T> {
	public double m_InterpolationBackTime = 0.1;
	public double m_ExtrapolationLimit = 0.5;
	
	private T resultantItem;
	public T ResultantItem{ get {return resultantItem;}}
	
	T[] m_BufferedState = new T[20];
	// Keep track of what slots are used
	int m_TimestampCount;
	
	public void ReceivedItem(T item) {	
		// Shift the buffer sideways, deleting state 20
		for (int i=m_BufferedState.Length-1;i>=1;i--)
		{
			m_BufferedState[i] = m_BufferedState[i-1];
		}
		
		// Record current state in slot 0
		m_BufferedState[0] = item;
		
		// Update used slot count, however never exceed the buffer size
		// Slots aren't actually freed so this just makes sure the buffer is
		// filled up and that uninitalized slots aren't used.
		m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

		// Check if states are in order, if it is inconsistent you could reshuffel or 
		// drop the out-of-order state. Nothing is done here
		for (int i=0;i<m_TimestampCount-1;i++)
		{
			if (m_BufferedState[i].GetTimeStamp() < m_BufferedState[i+1].GetTimeStamp())
				Debug.Log("State inconsistent");
		}
	}
	
	
	// We have a window of interpolationBackTime where we basically play 
	// By having interpolationBackTime the average ping, you will usually use interpolation.
	// And only if no more data arrives we will use extra polation
	public void Update (double currentTime) {
		if (m_TimestampCount == 0) return;
		// This is the target playback time of the rigid body
		double interpolationTime = currentTime - m_InterpolationBackTime;
		
		// Use interpolation if the target playback time is present in the buffer
		if ((m_BufferedState[0].GetTimeStamp() / 1000f) > interpolationTime)
		{
			//Debug.Log("OnInterpolate");
			// Go through buffer and find correct state to play back
			for (int i=0;i<m_TimestampCount;i++)
			{
				if (m_BufferedState[i].GetTimeStamp() / 1000f <= interpolationTime || i == m_TimestampCount-1)
				{
					// The state one slot newer (<100ms) than the best playback state
					T rhs = m_BufferedState[Mathf.Max(i-1, 0)];
					// The best playback state (closest to 100 ms old (default time))
					T lhs = m_BufferedState[i];
					resultantItem = lhs.Interpolate(rhs, interpolationTime);
					break;
				}
			}
		}
		// Use extrapolation
		else
		{
			T latest = m_BufferedState[0];
			
			float extrapolationLength = (float)(interpolationTime - latest.GetTimeStamp() / 1000f);
			// Don't extrapolation for more than 500 ms, you would need to do that carefully
			if (extrapolationLength < m_ExtrapolationLimit)
			{
				latest.Extrapolate(extrapolationLength);
			}
		}
	}
}
