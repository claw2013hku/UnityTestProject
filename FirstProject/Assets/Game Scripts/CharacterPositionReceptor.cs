using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterPositionEffectorComponent))]

public class CharacterPositionReceptor : MonoBehaviour {
	public SFSNetworkManager.Mode mode = SFSNetworkManager.Mode.LOCAL;
	
	private Interpolator<CharacterPositionEffectorComponent.NetworkMoveDirection> moveDirInterpolator = new Interpolator<CharacterPositionEffectorComponent.NetworkMoveDirection>();
	private Interpolator<CharacterPositionEffectorComponent.NetworkResultant> resultantInterpolator = new Interpolator<CharacterPositionEffectorComponent.NetworkResultant>();
	
	private CharacterPositionEffectorComponent component;
	
	void Start () {
		component = GetComponent<CharacterPositionEffectorComponent>();	
	}
	
	void Update () {
		if(SFSNetworkManager.Mode.LOCAL == mode || SFSNetworkManager.Mode.PREDICT == mode){
			Transform cameraTransform = Camera.main.transform;
			// Forward vector relative to the camera along the x-z plane	
			Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
			forward.y = 0f;
			forward = forward.normalized;
			
			// Right vector relative to the camera
			// Always orthogonal to the forward vector
			Vector3 right = new Vector3(forward.z, 0, -forward.x);
			
	  		float h = ControlSchemeInterface.instance.GetAxis(ControlAxis.MOVE_X);
	    	float v = ControlSchemeInterface.instance.GetAxis(ControlAxis.MOVE_Y);
			
			Vector3 targetDirection = h * right + v * forward;	
			component.MoveDirection = targetDirection;
		}
		
		if(SFSNetworkManager.Mode.REMOTE == mode || SFSNetworkManager.Mode.PREDICT == mode){
			resultantInterpolator.Update(TimeManager.Instance.NetworkTime / 1000);
			CharacterPositionEffectorComponent.NetworkResultant result = resultantInterpolator.ResultantItem;
			component.ResultantPosition = result.position;
			component.ResultantQuaternion = result.rotation;
		}
		
		if(SFSNetworkManager.Mode.HOSTREMOTE == mode){
			moveDirInterpolator.Update(TimeManager.Instance.NetworkTime / 1000);
			CharacterPositionEffectorComponent.NetworkMoveDirection result = moveDirInterpolator.ResultantItem;
			component.MoveDirection = result.moveDirection;
		}
	}
	
	public void ReceiveResultant(CharacterPositionEffectorComponent.NetworkResultant ntr){
		if(SFSNetworkManager.Mode.REMOTE == mode || SFSNetworkManager.Mode.PREDICT == mode){
			resultantInterpolator.ReceivedItem(ntr);
		}
		else{
			Debug.LogError("wrong mode");	
		}
	}

	public void ReceiveMoveDirection(CharacterPositionEffectorComponent.NetworkMoveDirection dir){
		if(SFSNetworkManager.Mode.HOSTREMOTE == mode){
			moveDirInterpolator.ReceivedItem(dir);
		}
		else{
			Debug.LogError("wrong mode");	
		}
	}
}
