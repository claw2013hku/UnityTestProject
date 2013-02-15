using UnityEngine;
using System.Collections;

public interface Interpolatable<T> where T: new(){
	double GetTimeStamp();
	void SetTimeStamp(double time);
	void Assign(T t);
}
