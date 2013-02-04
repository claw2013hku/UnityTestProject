using UnityEngine;
using System.Collections;

public class MovesControllerTest : MonoBehaviour {
	
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
	
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		idleAnimationNameHash = Animator.StringToHash(idleAnimationName);
		runAnimationNameHash = Animator.StringToHash(runAnimationName);
		slash1AnimationNameHash = Animator.StringToHash(slash1AnimationName);
		slash2AnimationNameHash = Animator.StringToHash(slash2AnimationName);
	}
	
	// Update is called once per frame
	void Update () {
		if (animator && animator.enabled)
		{
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			bool swinging = false;
			if(stateInfo.nameHash == idleAnimationNameHash || stateInfo.nameHash == runAnimationNameHash){
//				if(ControlSchemeInterface.instance.GetAxis(ControlAxis.ATTACK1) > 0f){
//					animator.SetBool("Slash", true);
//					animator.SetInteger("SlashVariant", UnityEngine.Random.Range(1, 3));
//				}
				DeactivateHitBoxes();
			}
			else if(stateInfo.nameHash == slash1AnimationNameHash || stateInfo.nameHash == slash2AnimationNameHash){
//				swinging = true;
				if(animator.GetFloat("SlashHit") > 0f){
					if(!slashHitbox.activated){
						Debug.Log ("Activate hitbox");
						slashHitbox.Activate(true);	
					}
				}
				else{
					if(slashHitbox.activated){
						slashHitbox.Activate(false);	
					}
				}
//				if(ControlSchemeInterface.instance.GetAxis(ControlAxis.ATTACK1) > 0f && stateInfo.normalizedTime > 0.6f){
//					animator.SetBool("Slash", true);
//					animator.SetInteger("SlashVariant", UnityEngine.Random.Range(1, 3));
//				}
			}
			
//			AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);
//			if(nextStateInfo.nameHash == idleAnimationNameHash || nextStateInfo.nameHash == runAnimationNameHash){
//
//			}
//			else if(nextStateInfo.nameHash == slash1AnimationNameHash || nextStateInfo.nameHash == slash2AnimationNameHash){
//				animator.SetBool("Slash", false);
//				animator.SetInteger("SlashVariant", 0);
//			}
//			if(swinging){
//				Debug.Log ("swinging:" + swinging);
//			}
		}
	}
	
	void DeactivateHitBoxes(){
		if(slashHitbox.activated){
			slashHitbox.Activate(false);	
		}
	}
}
