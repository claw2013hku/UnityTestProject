using UnityEngine;
using System;
using System.Collections;

public class Interpolator<T> where T: Interpolatable<T>, new() {
	private double m_InterpolationBackTime = 0.1;
	private double m_ExtrapolationLimit = 0.5;
	private bool useInterpolation = true;
	private bool useExtrapolation = true;
	
	public delegate void InterpolatorDelegate(int index, T[] buffer, int size, double time, ref T result);
	public delegate void ExtrapolatorDelegate(int index, T[] buffer, int size, float length, ref T result);
	private InterpolatorDelegate Interpolate;
	private ExtrapolatorDelegate Extrapolate;
	
	private T tempInterItem = new T();
	private T tempExtraItem = new T();
	private T resultantItem = new T();
	public T ResultantItem{ get {return resultantItem;}}
	
	T[] m_BufferedState = new T[20];
	// Keep track of what slots are used
	int m_TimestampCount;
	
	public Interpolator(bool useInter, double interBackTime, InterpolatorDelegate interD, bool useExtra, double extraLimit, ExtrapolatorDelegate extraD){
		useInterpolation = useInter;
		m_InterpolationBackTime = interBackTime;
		Interpolate = interD;
		
		useExtrapolation = useExtra;
		m_ExtrapolationLimit = extraLimit;
		Extrapolate = extraD;
	}
	
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
	public void Update (double currentTime, T approxCurrentState) {
		if (m_TimestampCount == 0) return;
		// This is the target playback time of the rigid body
		double interpolationTime = currentTime - m_InterpolationBackTime;
		
		// Use interpolation if the target playback time is present in the buffer
		bool usedInter = false;
		if (useInterpolation && Interpolate != null && (m_BufferedState[0].GetTimeStamp() / 1000f) > interpolationTime)
		{	
			// Go through buffer and find correct state to play back
			for (int i=0;i<m_TimestampCount;i++)
			{
				if (m_BufferedState[i].GetTimeStamp() / 1000f <= interpolationTime || i == m_TimestampCount-1)
				{
					Interpolate(Mathf.Max(i-1, 0), m_BufferedState, m_TimestampCount, interpolationTime, ref tempInterItem);
					usedInter = true;
					break;
				}
			}
		}
		
		// Use extrapolation
		bool usedExtra = false;
		if(!usedInter && useExtrapolation && Extrapolate != null)
		{
			int latestOldIndex = 0;
			for (int i=0;i<m_TimestampCount - 1; latestOldIndex++, i++)
			{
				if (m_BufferedState[i].GetTimeStamp() / 1000f < interpolationTime)
				{
					break;
				}
			}
			
			float extrapolationLength = Convert.ToSingle(interpolationTime - m_BufferedState[latestOldIndex].GetTimeStamp() / 1000.0f);
			// Don't extrapolation for more than 500 ms, you would need to do that carefully
			if (extrapolationLength < m_ExtrapolationLimit && m_TimestampCount > latestOldIndex + 1 && useExtrapolation)
			{
				Extrapolate(latestOldIndex, m_BufferedState, m_TimestampCount, extrapolationLength, ref tempExtraItem);
				usedExtra = true;
			}
		}
		
		if (usedExtra){
			resultantItem.Assign(tempExtraItem);
		}
		else if (usedInter){
			resultantItem.Assign(tempInterItem);
		}
		else{
			resultantItem.Assign(m_BufferedState[0]);
		}
	}
}
