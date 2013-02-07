using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

[RequireComponent (typeof(CharPosEffComp))]

public class CharPosSend : MonoBehaviour {
	private CharPosEffComp component;
	
	public SFSNetworkManager.Mode mode = SFSNetworkManager.Mode.LOCAL;
	
	// We will send transform each 0.1 second. To make transform synchronization smoother consider writing interpolation algorithm instead of making smaller period.
	public static readonly float sendingPeriod = 0.1f;
	
	private readonly float accuracy = 0.002f;
	private float timeLastSendingPos = 0.0f;
	private float timeLastSendingMove = 0.0f;

	private CharPosEffComp.NetworkResultant lastResultState = new CharPosEffComp.NetworkResultant();
	private CharPosEffComp.NetworkMoveDirection lastMoveState = new CharPosEffComp.NetworkMoveDirection();	
	
	void Start() {
		component = GetComponent<CharPosEffComp>();
	}
	
	void FixedUpdate() { 
		if(SFSNetworkManager.Mode.LOCAL == mode || SFSNetworkManager.Mode.HOSTREMOTE == mode){
			SendResultant();
		}
		else if (SFSNetworkManager.Mode.PREDICT == mode){
			SendMovementDirection();
		}
	}
	
	void SendResultant() {
		//if (lastResultState.IsDifferent(component, accuracy)) {
			if (timeLastSendingPos >= sendingPeriod) {
				lastResultState = CharPosEffComp.NetworkResultant.FromComponent(component);
				ISFSObject data = new SFSObject();
				CharPosEffComp.ToSFSObject(lastResultState, data);
				SFSNetworkManager.Instance.SendNetObjSync(data);
				timeLastSendingPos = 0;
				return;
			}
		//}
		timeLastSendingPos += Time.deltaTime;
	}
	
	void SendMovementDirection(){
		if (lastMoveState.IsDifferent(component, accuracy)) {
			if (timeLastSendingMove >= sendingPeriod) {
				lastMoveState = CharPosEffComp.NetworkMoveDirection.FromComponent(component);
				ISFSObject data = new SFSObject();
				CharPosEffComp.ToSFSObject(lastMoveState, data);
				SFSNetworkManager.Instance.SendNetObjSync(data);
				timeLastSendingMove = 0;
				return;
			}
		}
		timeLastSendingMove += Time.deltaTime;	
	}
}
