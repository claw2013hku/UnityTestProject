using UnityEngine;
using System.Collections;

//Stores base stats, modifiers, modified stats
//Fires (resulting) base stats changed event, modifiers changed event, modified stats changed event
public class CharacterStateEffectorComponent : MonoBehaviour {
	//Base stats
	public int currentAnimationState;
	public int CurrentAnimationState {
		set {
			if(currentAnimationState != value){
				HasChangedCurrentAnimation(currentAnimationState, value);
			}
			currentAnimationState = value;
		}
		get {return currentAnimationState;}}
	
	public int nextAnimationState;	
	public int NextAnimationState {
		set {
			if(nextAnimationState != value){
				HasChangedNextAnimation(nextAnimationState, value);
			}
			nextAnimationState = value;
		}
		get {return nextAnimationState;}}
	
	//delegates
	public delegate void AnimationStateChange(int oldVal, int newVal);
	public event AnimationStateChange HasChangedCurrentAnimation;
	public event AnimationStateChange HasChangedNextAnimation;
	
	//Helper stats
	private string runAnimationName = "Base Layer.Run Blend Tree";
	public int RunAnimationNameHash;
	private string idleAnimationName = "Base Layer.Idle";
	public int IdleAnimationNameHash;
	
	//Public methods
	
	//Helper methods
	void Start(){
		RunAnimationNameHash = Animator.StringToHash(runAnimationName);
		IdleAnimationNameHash = Animator.StringToHash(idleAnimationName);
	}	
}
