using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {
	public Transform player;
	public GameObject playerObj;
	
	public float smoothingTime = 0.9f;
	public float cameraMoveSmoothingTime = 1f;
	public float cameraRotSmoothingTime = 1f;
	
	public float maxHeight = 5f;
	public float minHeight = 0f;
	public float maxDist = 4f;
	public float minDist = 1f;
	
	public float horizontalAimingSpeed = 1f;
	public float verticalAimingSpeed = 1f;
	public float maxVerticalAngle = 80f;
	public float minVerticalAngle = -80f;
	
	public float mouseSensitivity = 1f;
	
	public float clampHeadPositionScreenSpace = 0.75f;
	
	[HideInInspector]
	public Vector3 position;
	[HideInInspector]
	public Quaternion rotation;
	
	private float targetDistance;
		
	private float maxCamDist = 1;
	private LayerMask mask;
	
	private bool storeDist = false;
	private float storedDist = 0f;
	
	private ThirdPersonTouchController controller;
	
	// Use this for initialization
	void Start () {
		// Add player's own layer to mask
		mask = 1 << player.gameObject.layer;
		// Invert mask
		mask = ~mask;
		position = transform.position;
		rotation = transform.rotation;
		maxCamDist = 3;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Time.deltaTime == 0 || Time.timeScale == 0 || player == null) 
			return;
		
		controller = playerObj.GetComponent<ThirdPersonTouchController>();
		
		Vector3	targetCenter = player.position;	
		Vector3 offsetToCamera = targetCenter - position;
		
		
		//Determine and clamp camera distance from player
		if(storeDist){
			targetDistance = storedDist;
		}
		else{
			Vector2 offsetToCamera2 = new Vector2(offsetToCamera.x, offsetToCamera.z);
			float realDistance2 = offsetToCamera2.magnitude;
			//float targetDistance = realDistance == 0 ? 0 : realDistance * Mathf.Cos(Mathf.Deg2Rad * horizontalAngleDiff);
		
			float targetDistance2 = realDistance2;
			targetDistance2 = Mathf.Clamp(targetDistance2, minDist, maxDist);
		
			Vector3 targetPos = new Vector3(
			targetCenter.x - ((offsetToCamera2.normalized).x * targetDistance2),
			position.y,
			targetCenter.z - ((offsetToCamera2.normalized).y * targetDistance2));
		
			offsetToCamera = targetCenter - targetPos;
			targetDistance = Vector3.Distance(targetCenter, targetPos);	
		}
		
		//Determine camera rotation from input
		Quaternion direction = Quaternion.LookRotation(offsetToCamera, Vector3.up);
		Quaternion newDirection;
		float xRot = direction.eulerAngles.x > 180 ? direction.eulerAngles.x - 360 : direction.eulerAngles.x;
		xRot += -ControlSchemeInterface.instance.GetAxis(ControlAxis.CAMERA_SCROLL_Y) * mouseSensitivity * verticalAimingSpeed;
		xRot = Mathf.Clamp(xRot, minVerticalAngle, maxVerticalAngle);
		xRot = xRot < 0 ? 360 + xRot : xRot;
		float yRot = direction.eulerAngles.y;
//		Debug.Log ("xRot: " + xRot);
		yRot += ControlSchemeInterface.instance.GetAxis(ControlAxis.CAMERA_SCROLL_X) * mouseSensitivity * horizontalAimingSpeed;
		newDirection = 
			Quaternion.Euler(
				xRot, 
				yRot, 
				direction.eulerAngles.z);
		
		//Clamp camera height relative to player
		float newTargetHeight = Mathf.Sin(Mathf.Deg2Rad * newDirection.eulerAngles.x) * targetDistance;
		if(newTargetHeight > maxHeight){
			targetDistance *= maxHeight / newTargetHeight;
		}
		else if (newTargetHeight < minHeight){
			targetDistance *= minHeight / newTargetHeight;
		}
		
		// Make sure camera doesn't intersect geometry
		// Move camera towards closeOffset if ray back towards camera position intersects something 
		RaycastHit hit;
		float padding = 0.3f;
		if (Physics.Raycast(targetCenter, newDirection * Vector3.back, out hit, targetDistance + padding, mask)) {
			storeDist = true;
			storedDist = targetDistance;
			targetDistance = hit.distance - padding;
		}
		else{
			storeDist = false;
		}
	
		Vector3 targetPosition = targetCenter - newDirection * Vector3.forward * targetDistance;
		offsetToCamera = targetCenter - targetPosition;
		
		Quaternion targetRotation = Quaternion.LookRotation(offsetToCamera, Vector3.up);
	
		//Freeze vertical rotation when jumping
		if (controller.IsJumping()){
			Debug.Log("not grounded");
			targetRotation *= Quaternion.Inverse(Quaternion.Euler(direction.eulerAngles.x, 0, 0));
			targetRotation *= Quaternion.Euler(rotation.eulerAngles.x, 0, 0);
		}
		
		float lerpedXRot = Mathf.LerpAngle(
			rotation.eulerAngles.x,// > 180 ? resultCameraRot.eulerAngles.x - 360 : resultCameraRot.eulerAngles.x,
			targetRotation.eulerAngles.x,// > 180 ? targetRotation.eulerAngles.x - 360 : targetRotation.eulerAngles.x,
			smoothingTime * Time.deltaTime);
		targetRotation = 
			Quaternion.Euler(
				lerpedXRot,
				targetRotation.eulerAngles.y,
				targetRotation.eulerAngles.z);
//		Debug.Log("lerpedXRot: " + lerpedXRot);
		rotation = targetRotation;
		position = targetPosition;
	}
	
	public void ResetDirection(Quaternion direction){
		position = player.position - direction * Vector3.forward * (targetDistance == 0f ? 0.5f : targetDistance);
		rotation = direction;
	}
}
