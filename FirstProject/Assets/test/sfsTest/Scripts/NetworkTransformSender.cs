//using UnityEngine;
//using System.Collections;
//using System;
//
//// Sends the transform of the local player to server
//public class NetworkTransformSender : MonoBehaviour {
//
//	// We will send transform each 0.1 second. To make transform synchronization smoother consider writing interpolation algorithm instead of making smaller period.
//	public static readonly float sendingPeriod = 0.1f; 
//	
//	private readonly float accuracy = 0.002f;
//	private float timeLastSending = 0.0f;
//
//	private bool send = false;
//	private NetworkTransform lastState;
//	
//	private Transform thisTransform;
//	
//	protected Animator animator;
//	
//	private string idleAnimationName = "Base Layer.Idle";
//	private int idleAnimationNameHash;
//	
//	private string runAnimationName = "Base Layer.Run Blend Tree";
//	private int runAnimationNameHash;
//	
//	private string slash1AnimationName = "Slash.Slash1";
//	private int slash1AnimationNameHash;
//	
//	private string slash2AnimationName = "Slash.Slash2";
//	private int slash2AnimationNameHash;
//	
//	
//	void Start() {
//		thisTransform = this.transform;
//		lastState = NetworkTransform.FromTransform(thisTransform);
//		animator = GetComponent<Animator>();
//		idleAnimationNameHash = Animator.StringToHash(idleAnimationName);
//		runAnimationNameHash = Animator.StringToHash(runAnimationName);
//		slash1AnimationNameHash = Animator.StringToHash(slash1AnimationName);
//		slash2AnimationNameHash = Animator.StringToHash(slash2AnimationName);
//	}
//		
//	// We call it on local player to start sending his transform
//	void StartSendTransform() {
//		send = true;
//	}
//	
//	void FixedUpdate() { 
//		if (send) {
//			SendTransform();
//		}
//	}
//	
//	void SendTransform() {
//		//if (lastState.IsDifferent(thisTransform, accuracy)) {
//			if (timeLastSending >= sendingPeriod) {
//				lastState = NetworkTransform.FromTransform(thisTransform);
//				SFSNetworkManager.Instance.SendTransform(lastState);
//				timeLastSending = 0;
//				return;
//			}
//		//}
//		timeLastSending += Time.deltaTime;
//		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
//		bool swinging = false;
//		if(stateInfo.nameHash == idleAnimationNameHash || stateInfo.nameHash == runAnimationNameHash){
//			if(ControlSchemeInterface.instance.GetAxis(ControlAxis.ATTACK1) > 0f){
//				SFSNetworkManager.Instance.SendReload();	
//			}
//		}
//	}
//}
