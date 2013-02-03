using UnityEngine;
using System.Collections;

public class CharAnimEff: MonoBehaviour {
	public SFSNetworkManager.Mode mode = SFSNetworkManager.Mode.LOCAL;
	
	[HideInInspector]
	public int RunAnimationNameHash;
	[HideInInspector]
	public int IdleAnimationNameHash;
	[HideInInspector]
	public int Slash1AnimationNameHash;
	[HideInInspector]
	public int Slash2AnimationNameHash;
		
	private Animator animator;
	private CharAnimEffComp component;
	private CharacterPositionEffectorComponent posComponent;
	private CharacterPositionEffector posEffector;
	
	private static float[] runAnimationLookUpValues = {0.224f, 0.5f, 0.666f, 0.778f, 0.857f, 0.9165f, 0.963f, 1f};
	private static float defaultRunAnimationVelocity = 5.299f;
	
	void Start () {
		RunAnimationNameHash = Animator.StringToHash(CharAnimEffComp.RunAnimationName);
		IdleAnimationNameHash = Animator.StringToHash(CharAnimEffComp.IdleAnimationName);
		Slash1AnimationNameHash = Animator.StringToHash(CharAnimEffComp.Slash1AnimationName);
		Slash2AnimationNameHash = Animator.StringToHash(CharAnimEffComp.Slash2AnimationName);
		
		animator = GetComponent<Animator>();
		component = GetComponent<CharAnimEffComp>();
		posComponent = GetComponent<CharacterPositionEffectorComponent>();
		posEffector = GetComponent<CharacterPositionEffector>();
	}
		
	void Update () 
	{
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);
			
		//Run
		animator.SetBool("Run", false);
			//local
		if(mode == SFSNetworkManager.Mode.LOCAL || mode == SFSNetworkManager.Mode.PREDICT || mode == SFSNetworkManager.Mode.HOSTREMOTE){
			if(posComponent.ResultantVelocity.sqrMagnitude != 0 && 
				posEffector.IsGrounded() &&
				(stateInfo.nameHash == IdleAnimationNameHash || 
					stateInfo.nameHash == RunAnimationNameHash)){
				animator.SetBool("Run", true);
			}
		}
			//remote
		if(mode == SFSNetworkManager.Mode.REMOTE){
			if(posComponent.ResultantVelocity.sqrMagnitude != 0){ 
				animator.SetBool("Run", true);
			}
		}
		
		animator.SetFloat("Blended Speed", getRunAnimationSpeedValue(posComponent.ResultantVelocity.magnitude));
		animator.SetFloat("Speed", posComponent.ResultantVelocity.magnitude);
		
		//Attack
			//local
		if(mode == SFSNetworkManager.Mode.LOCAL || mode == SFSNetworkManager.Mode.PREDICT || mode == SFSNetworkManager.Mode.HOSTREMOTE){
			if((stateInfo.nameHash == IdleAnimationNameHash || stateInfo.nameHash == RunAnimationNameHash) &&
				(nextStateInfo.nameHash != Slash1AnimationNameHash && nextStateInfo.nameHash != Slash2AnimationNameHash)){
				if(component.Swing > 0f){
					animator.SetBool("Slash", true);
					animator.SetInteger("SlashVariant", UnityEngine.Random.Range(1, 3));
				}
			}
//			else if(stateInfo.nameHash == Slash1AnimationNameHash || stateInfo.nameHash == Slash2AnimationNameHash){
//				if(component.Swing > 0f && stateInfo.normalizedTime > 0.9f){
//					animator.SetBool("Slash", true);
//					animator.SetInteger("SlashVariant", UnityEngine.Random.Range(1, 3));
//				}
//			}		
		}
			//remote
		if(mode == SFSNetworkManager.Mode.REMOTE){
			if(component.StateInfoNameHash == Slash1AnimationNameHash && stateInfo.nameHash != Slash1AnimationNameHash){
				animator.SetBool("Slash", true);
				animator.SetInteger("SlashVariant", 1);	
			}
			else if (component.StateInfoNameHash == Slash2AnimationNameHash && stateInfo.nameHash != Slash2AnimationNameHash){
				animator.SetBool("Slash", true);
				animator.SetInteger("SlashVariant", 2);	
			}
			else if (component.StateInfoNameHash == IdleAnimationNameHash && stateInfo.nameHash != IdleAnimationNameHash){
				animator.SetBool("Slash", false);
				animator.SetInteger("SlashVariant", 0);
			}
			else if (component.StateInfoNameHash == RunAnimationNameHash && stateInfo.nameHash != RunAnimationNameHash){
				animator.SetBool("Slash", false);
				animator.SetInteger("SlashVariant", 0);
			}
//			Debug.Log ("Set Slash: " + component.state.Slash);
			animator.SetBool("Slash", component.state.Slash);
			animator.SetInteger("SlashVariant", component.state.SlashVariant);
		}
		
		//Sync animation variables
		if(mode == SFSNetworkManager.Mode.LOCAL || mode == SFSNetworkManager.Mode.HOSTREMOTE){
			CharAnimEffComp.AnimState state;
			state.Slash = animator.GetBool("Slash");
			state.SlashVariant = animator.GetInteger("SlashVariant");
			component.State = state;
			component.StateInfoNameHash = animator.GetCurrentAnimatorStateInfo(0).nameHash;	
		}
		
		if(mode == SFSNetworkManager.Mode.LOCAL || mode == SFSNetworkManager.Mode.PREDICT || mode == SFSNetworkManager.Mode.HOSTREMOTE){
			if(nextStateInfo.nameHash == IdleAnimationNameHash || nextStateInfo.nameHash == RunAnimationNameHash){
	
			}
			else if(nextStateInfo.nameHash == Slash1AnimationNameHash || nextStateInfo.nameHash == Slash2AnimationNameHash){
				animator.SetBool("Slash", false);
				animator.SetInteger("SlashVariant", 0);
			}	
		}		
	}
	
	private static float getRunAnimationSpeedValue(float velocity){	
		float factor = velocity / (defaultRunAnimationVelocity * 2f);
		int index = (int)(factor * 10 + 0.5f) - 3;
		return runAnimationLookUpValues[Mathf.Clamp(index, 0, 7)];
	}
}
