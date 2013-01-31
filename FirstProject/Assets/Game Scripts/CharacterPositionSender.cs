using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterPositionEffectorComponent))]

public class CharacterPositionSender : MonoBehaviour {
	private CharacterPositionEffectorComponent component;
	
	public SFSNetworkManager.Mode mode = SFSNetworkManager.Mode.LOCAL;
	
	// We will send transform each 0.1 second. To make transform synchronization smoother consider writing interpolation algorithm instead of making smaller period.
	public static readonly float sendingPeriod = 0.1f; 
	
	private readonly float accuracy = 0.002f;
	private float timeLastSending = 0.0f;

	private CharacterPositionEffectorComponent.NetworkResultant lastResultState = new CharacterPositionEffectorComponent.NetworkResultant();
	private CharacterPositionEffectorComponent.NetworkMoveDirection lastMoveState = new CharacterPositionEffectorComponent.NetworkMoveDirection();	
	
	void Start() {
		component = GetComponent<CharacterPositionEffectorComponent>();
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
		if (lastResultState.IsDifferent(component, accuracy)) {
			if (timeLastSending >= sendingPeriod) {
				lastResultState = CharacterPositionEffectorComponent.NetworkResultant.FromComponent(component);
				//Debug.Log ("Sending Resultant: " + lastResultState.position + ", " + lastResultState.rotation);
				SFSNetworkManager.Instance.SendCharacterPositionResultant(lastResultState);
				timeLastSending = 0;
				return;
			}
		}
		timeLastSending += Time.deltaTime;
	}
	
	void SendMovementDirection(){
		if (lastMoveState.IsDifferent(component, accuracy)) {
			if (timeLastSending >= sendingPeriod) {
				lastMoveState = CharacterPositionEffectorComponent.NetworkMoveDirection.FromComponent(component);
				//SFSNetworkManager.Instance.SendCharacterPositionMovement(lastMoveState);
				Debug.Log ("Sending Movement: " + lastMoveState.moveDirection);
				timeLastSending = 0;
				return;
			}
		}
		timeLastSending += Time.deltaTime;	
	}
}
