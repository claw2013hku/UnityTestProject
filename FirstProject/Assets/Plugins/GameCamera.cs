using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {
	private float targetHeight;
	
	public Transform testObj;
	
	public Transform player;
	public GameObject playerObj;
	
	public float verticalAngleSmoothingTime = 0.9f;
//	public float cameraMoveSmoothingTime = 1f;
//	public float cameraRotSmoothingTime = 1f;
	
	public float maxHeight = 5f;
	public float minHeight = 0f;
	public float maxDist = 4f;
	public float minDist = 1f;
	
	public float horizontalAimingSpeed = 1f;
	public float verticalAimingSpeed = 1f;
	public float maxVerticalAngle = 80f;
	public float minVerticalAngle = -80f;
	
	public float mouseSensitivity = 1f;
	
	[HideInInspector]
	public Vector3 position;
	[HideInInspector]
	public Quaternion rotation;
	
	private float targetDistance;

	private LayerMask mask;
	
	private bool storeDist = false;
	private float storedDist = 0f;
	
	private ThirdPersonTouchController controller;
	
	public bool dontClampDist = false;
	public bool dontClampXRot = false;
	public bool dontClampHeight = false;
	
	public float raycastPadding = 10f;
	// Use this for initialization
	void Start () {
		// Add player's own layer to mask
		mask = 1 << player.gameObject.layer;
		mask |= ( 1 << LayerMask.NameToLayer("Ignore Raycast"));
		mask |= ( 1 << LayerMask.NameToLayer("PlayerRagdoll"));
		// Invert mask
		mask = ~mask;
		position = transform.position;
		rotation = transform.rotation;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Time.deltaTime == 0 || Time.timeScale == 0 || player == null) 
			return;
		
		controller = playerObj.GetComponent<ThirdPersonTouchController>();
		
		Vector3	targetCenter = player.position;	
		
		//Determine and clamp camera distance from player
		if(storeDist){
			targetDistance = storedDist;
		}
		else{
			targetDistance = Vector3.Distance(targetCenter, position);
		}
		
		Vector3 targetPosition;
		
		//Determine camera rotation from input
		Quaternion direction = Quaternion.LookRotation(position - targetCenter, Vector3.up);
		float xRot = direction.eulerAngles.x > 180 ? direction.eulerAngles.x - 360 : direction.eulerAngles.x;
		xRot += -(ControlSchemeInterface.instance.GetAxis(ControlAxis.CAMERA_SCROLL_Y)) * mouseSensitivity * verticalAimingSpeed;
		if(!dontClampXRot){
			xRot = Mathf.Clamp(xRot, minVerticalAngle, maxVerticalAngle);
		}
		xRot = xRot < 0 ? 360 + xRot : xRot;
		
		float yRot = direction.eulerAngles.y;
		yRot += (ControlSchemeInterface.instance.GetAxis(ControlAxis.CAMERA_SCROLL_X)) * mouseSensitivity * horizontalAimingSpeed;
		Quaternion newDirection = 
			Quaternion.Euler(
				xRot, 
				yRot, 
				direction.eulerAngles.z);
		targetPosition = targetCenter + newDirection * Vector3.forward * targetDistance;
		
		if(!dontClampHeight){
			bool heightClamped = true;
			//Clamp camera height relative to player
			float newTargetHeight = targetPosition.y - targetCenter.y;
			targetHeight = newTargetHeight;
			if(newTargetHeight > maxHeight){
				//targetDistance *= maxHeight / newTargetHeight;
				targetPosition.y = targetCenter.y + maxHeight;
			}
			else if (newTargetHeight < minHeight){
				//targetDistance *= minHeight / newTargetHeight;
				targetPosition.y = targetCenter.y + minHeight;
			}
			else{
				heightClamped = false;
			}
			
			if(heightClamped){
				//targetPosition = targetCenter + newDirection * Vector3.forward * targetDistance;
			}
		}
		
		if(!dontClampDist){
			bool XZDistClamped = true;
			
			Vector2 targetCenter2D = new Vector2(targetCenter.x, targetCenter.z);
			Vector2 targetPosition2D = new Vector2(targetPosition.x, targetPosition.z);
			Vector2 direction2D = targetPosition2D - targetCenter2D;
			float distanceXZ = direction2D.magnitude;
			direction2D.Normalize();
			if(distanceXZ > maxDist){
				targetPosition2D = targetCenter2D + direction2D * maxDist;
			}
			else if (distanceXZ < minDist){
				targetPosition2D = targetCenter2D + direction2D * minDist;
			}
			else{
				XZDistClamped = false;	
			}
			
			if(XZDistClamped){
				targetPosition = new Vector3(targetPosition2D.x, targetPosition.y, targetPosition2D.y);
			}
		}
		
		// Make sure camera doesn't intersect geometry
		// Move camera towards closeOffset if ray back towards camera position intersects something 
		RaycastHit hit;
		Vector3 rayDirection = targetPosition - targetCenter;
		float rayDistance = rayDirection.magnitude;
		rayDirection.Normalize();
		if (Physics.Raycast(targetCenter, rayDirection , out hit, rayDistance, mask)) {
			if(!storeDist){
				storedDist = hit.distance;
				storeDist = true;
			}
			targetPosition = targetCenter + rayDirection * (hit.distance + raycastPadding);
		}
		else{
			storeDist = false;
		}
		
		Vector3 cameraDirection = targetCenter - targetPosition;
		Quaternion targetRotation = Quaternion.LookRotation(cameraDirection, Vector3.up);
	
		//Freeze vertical rotation when jumping
		if (controller != null && controller.IsJumping()){
			Debug.Log("not grounded");
			targetRotation *= Quaternion.Inverse(Quaternion.Euler(direction.eulerAngles.x, 0, 0));
			targetRotation *= Quaternion.Euler(rotation.eulerAngles.x, 0, 0);
		}
		
		float lerpedXRot = Mathf.LerpAngle(
			rotation.eulerAngles.x,
			targetRotation.eulerAngles.x,
			verticalAngleSmoothingTime * Time.deltaTime);
		targetRotation = 
			Quaternion.Euler(
				lerpedXRot,
				targetRotation.eulerAngles.y,
				targetRotation.eulerAngles.z);
		
		rotation = targetRotation;
//		position = Vector3.Slerp(position, targetPosition, cameraMoveSmoothingTime);
		position = targetPosition;
	}
	
	public void ResetDirection(Quaternion direction){
		position = player.position - direction * Vector3.forward * (targetDistance == 0f ? 0.5f : targetDistance);
		rotation = direction;
	}
	
	public void Reset(Vector3 _position, Quaternion _rotation, bool _dontClamp = false){
		position = _position;
		rotation = _rotation;
		dontClampDist = _dontClamp;
		dontClampXRot = _dontClamp;
		dontClampHeight = _dontClamp;
	}
}
