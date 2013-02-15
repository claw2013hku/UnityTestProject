using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

[RequireComponent (typeof(CharAnimEffComp))]

public class CharAnimRecp : MonoBehaviour {
	public SFSNetworkManager.Mode mode = SFSNetworkManager.Mode.LOCAL;
	
	//local
	private Animator animator;
	
	//remote
	public float interpolatorBackTime = 0.1f;
	public float interpolatorExTime = 0.5f;
	public bool useInterpolation = true;
	public bool useExtrapolation = true;
	
	private Interpolator<CharAnimEffComp.NetworkResultantState> resultantInterpolator;
	
	private CharAnimEffComp component;
	
	void Start () {
		animator = GetComponent<Animator>();
		component = GetComponent<CharAnimEffComp>();
		resultantInterpolator = new Interpolator<CharAnimEffComp.NetworkResultantState>(
			useInterpolation, 
			interpolatorBackTime, 
			CharAnimEffComp.NetworkResultantState.Interpolate, 
			useExtrapolation, 
			interpolatorExTime, 
			CharAnimEffComp.NetworkResultantState.Extrapolate);
	}
	
	void Update () {
		if(SFSNetworkManager.Mode.LOCAL == mode || SFSNetworkManager.Mode.PREDICT == mode){
			component.Swing = ControlSchemeInterface.instance.GetAxis(ControlAxis.ATTACK1);
		}
		
		if(SFSNetworkManager.Mode.REMOTE == mode || SFSNetworkManager.Mode.PREDICT == mode){
			resultantInterpolator.Update(TimeManager.Instance.NetworkTime / 1000f, null);
			component.StateInfoNameHash =  resultantInterpolator.ResultantItem.nameHash;
			component.state = resultantInterpolator.ResultantItem.state;
		}
		
		if(SFSNetworkManager.Mode.HOSTREMOTE == mode){
			
		}
	}
	
	public void ReceiveState(ISFSObject obj){
		ReceiveState(CharAnimEffComp.NetworkResultantState.FromSFSObject(obj));
	}
	
	public void ReceiveState(CharAnimEffComp.NetworkResultantState state){
		if(SFSNetworkManager.Mode.REMOTE == mode || SFSNetworkManager.Mode.PREDICT == mode){
			resultantInterpolator.ReceivedItem(state);	
		}
		else{
//			Debug.LogError("wrong mode");	
		}
	}
}
