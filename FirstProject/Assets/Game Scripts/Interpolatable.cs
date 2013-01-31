using UnityEngine;
using System.Collections;

public interface Interpolatable<T>{
	T Interpolate(T rhs, double time);
	T Extrapolate(double time);
	double GetTimeStamp();
}
