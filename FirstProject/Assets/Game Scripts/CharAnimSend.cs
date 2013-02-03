using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharAnimEffComp))]

public class CharAnimSend : MonoBehaviour {
	private CharAnimEffComp component;
	
	public SFSNetworkManager.Mode mode = SFSNetworkManager.Mode.LOCAL;
	
	// We will send transform each 0.1 second. To make transform synchronization smoother consider writing interpolation algorithm instead of making smaller period.
	public static readonly float sendingPeriod = 0.5f;
	
	private readonly float accuracy = 0.002f;
	private float timeLastSendingState = 0.0f;
	private bool pendingSend = false;
	
	private CharAnimEffComp.NetworkResultantState lastState = new CharAnimEffComp.NetworkResultantState();
	
	// Use this for initialization
	void Start () {
		component = GetComponent<CharAnimEffComp>();
		component.HasChangedAnimState += ChangedState;
		//component.HasChangedAnimHash += ChangedHash;
	}

	void FixedUpdate () {
		if(SFSNetworkManager.Mode.LOCAL == mode || SFSNetworkManager.Mode.HOSTREMOTE == mode){
			SendState();
		}
		else if (SFSNetworkManager.Mode.PREDICT == mode){
			
		}
	}
	
	void SendState(){
		//if(lastState.nameHash != component.StateInfoNameHash){
			if (timeLastSendingState >= sendingPeriod || pendingSend) {
				if(pendingSend){
					//Debug.Log ("pend send");
					pendingSend = false;
				}
				CharAnimEffComp.NetworkResultantState.FromComponent(component, ref lastState);
				SFSNetworkManager.Instance.SendAnimCompState(lastState);
				timeLastSendingState = 0;
				return;
			}
		//}
		timeLastSendingState += Time.deltaTime;
	}
		
	private void ChangedHash(int oldVal, int newVal){
		if(mode == SFSNetworkManager.Mode.LOCAL || mode == SFSNetworkManager.Mode.HOSTREMOTE){
			pendingSend = true;	
		}
	}
		
	private void ChangedState(CharAnimEffComp.AnimState oldState, CharAnimEffComp.AnimState newState){
		if(mode == SFSNetworkManager.Mode.LOCAL || mode == SFSNetworkManager.Mode.HOSTREMOTE){
//			Debug.Log ("pend send: " + oldState.Slash + ", " + oldState.SlashVariant + ", " + newState.Slash + ", " + newState.SlashVariant);
			pendingSend = true;	
		}
	}
}
