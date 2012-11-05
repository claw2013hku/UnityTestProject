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
	
	public float horizontalAimingSpeed = 270f;
	public float verticalAimingSpeed = 270f;
	public float maxVerticalAngle = 80f;
	public float minVerticalAngle = -80f;
	
	public float mouseSensitivity = 1f;
	
	public float horizontalSpeed = 1f;
	public float verticalSpeed = 1f;
	
	public float clampHeadPositionScreenSpace = 0.75f;
	
	private float angleH = 0;
	private float angleV = 0;
	private Transform cam;
	private float maxCamDist = 1;
	private LayerMask mask;
	private Vector3 smoothPlayerPos;
	
	private bool storeDist = false;
	private float storedDist = 0f;
	
	private ThirdPersonController2 controller;
	
	private bool saveDirection = false;
	private Quaternion savedDirection;
	private float savedDistanceForDirection;
	private Vector3 tempTarget;
	private Vector3 tempTargetOffset;
	private Vector3 expectedTarget;
	private Vector3 savedPlayerPos;
	private Vector3 lastCameraOffset;
	private Vector3 tempCameraPos;
	private Quaternion tempCameraRot;
	
	public Transform testObj;
	
	private Vector3 savedCameraOffset;
	private Quaternion savedRotation;
	// Use this for initialization
	void Start () {
		// Add player's own layer to mask
		mask = 1 << player.gameObject.layer;
		// Invert mask
		mask = ~mask;
		
		cam = transform;
		smoothPlayerPos = player.position;
		
		maxCamDist = 3;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Time.deltaTime == 0 || Time.timeScale == 0 || player == null) 
			return;
		controller = playerObj.GetComponent<ThirdPersonController2>();
		
		Vector3 resultCameraPos;
		Quaternion resultCameraRot;
		Transform resultCamera;
		if(saveDirection){
			resultCameraPos = tempCameraPos;
			resultCameraRot = tempCameraRot;
		}
		else{
			resultCameraPos = transform.position;
			resultCameraRot = transform.rotation;
		}
		
		Vector3 targetCenter;
		/*if(saveDirection){
			targetCenter = tempTarget;
		}
		else{
			targetCenter = player.position;	
		}*/
		targetCenter = player.position;	
		Vector3 offsetToCamera = targetCenter - resultCameraPos;
		Vector2 offsetToCamera2 = new Vector2(offsetToCamera.x, offsetToCamera.z);
		/*float horizontalAngleDiff = Vector3.Angle(
			new Vector3(offsetToCamera.x, 0, offsetToCamera.z),
			new Vector3(resultCamera.forward.x, 0, resultCamera.forward.z));*/
		float realDistance = offsetToCamera2.magnitude;
		
		//float targetDistance = realDistance == 0 ? 0 : realDistance * Mathf.Cos(Mathf.Deg2Rad * horizontalAngleDiff);
		float targetDistance;
		//if(saveDirection){
		//	targetDistance = savedDistanceForDirection;	
		//}
		//else{
		//	targetDistance = realDistance;
		//}
		targetDistance = realDistance;
		targetDistance = Mathf.Clamp(targetDistance, minDist, maxDist);
		
		Vector3 targetPosition = new Vector3(
			targetCenter.x - ((offsetToCamera2.normalized).x * targetDistance),
			resultCameraPos.y,
			targetCenter.z - ((offsetToCamera2.normalized).y * targetDistance));
		
		offsetToCamera = targetCenter - targetPosition;
		Quaternion direction = Quaternion.LookRotation(offsetToCamera, Vector3.up);
		
		
		float newTargetDistance;
		if(storeDist){
			newTargetDistance = storedDist;
		}
		else{
			newTargetDistance = Vector3.Distance(targetCenter, targetPosition);	
		}
		
		Quaternion newDirection;
		//if(saveDirection){
		//	direction = savedDirection;
			
		//}
//		else{
			float xRot = direction.eulerAngles.x > 180 ? direction.eulerAngles.x - 360 : direction.eulerAngles.x;
			xRot += Input.GetAxis("Mouse Y") * mouseSensitivity * verticalSpeed;
			xRot = Mathf.Clamp(xRot, minVerticalAngle, maxVerticalAngle);
			xRot = xRot < 0 ? 360 + xRot : xRot;
			float yRot = direction.eulerAngles.y;
			yRot += Input.GetAxis("Mouse X") * mouseSensitivity * horizontalSpeed;
			newDirection = 
				Quaternion.Euler(
					xRot, 
					yRot, 
					direction.eulerAngles.z);
//		}
		//savedDirection = newDirection;
		Vector3 newTargetPos = targetCenter - newDirection * Vector3.forward * newTargetDistance;
		float temp_newTargetDistance = newTargetDistance;
		
		float newTargetHeight = Mathf.Sin(Mathf.Deg2Rad * newDirection.eulerAngles.x) * newTargetDistance;
		float realHeight = newTargetHeight;
		if(newTargetHeight > maxHeight){
			newTargetDistance *= maxHeight / newTargetHeight;
			realHeight = maxHeight;
		}
		
		// Make sure camera doesn't intersect geometry
		// Move camera towards closeOffset if ray back towards camera position intersects something 
		RaycastHit hit;
		float padding = 0.3f;
		if (Physics.Raycast(targetCenter, newDirection * Vector3.back, out hit, newTargetDistance + padding, mask)) {
			storeDist = true;
			storedDist = newTargetDistance;
			newTargetDistance = hit.distance - padding;
		}
		else{
			storeDist = false;
		}
	
		targetPosition = targetCenter - newDirection * Vector3.forward * newTargetDistance;
		offsetToCamera = targetCenter - targetPosition;
		
		Quaternion targetRotation = Quaternion.LookRotation(offsetToCamera, Vector3.up);
	
		if (controller.IsJumping()){
			Debug.Log("not grounded");
			targetRotation *= Quaternion.Inverse(Quaternion.Euler(direction.eulerAngles.x, 0, 0));
			targetRotation *= Quaternion.Euler(resultCameraRot.eulerAngles.x, 0, 0);
		}
		
		float lerpedXRot = Mathf.Lerp(
			resultCameraRot.eulerAngles.x > 180 ? resultCameraRot.eulerAngles.x - 360 : resultCameraRot.eulerAngles.x,
			targetRotation.eulerAngles.x > 180 ? targetRotation.eulerAngles.x - 360 : targetRotation.eulerAngles.x,
			smoothingTime);
		lerpedXRot = lerpedXRot < 0 ? 360 - lerpedXRot : lerpedXRot;
		Quaternion targetRotation2 = 
			Quaternion.Euler(
				lerpedXRot,
				targetRotation.eulerAngles.y,
				targetRotation.eulerAngles.z);
		resultCameraRot = targetRotation2;
		
		resultCameraPos = targetPosition;
		
		if(saveDirection){
			tempCameraPos = resultCameraPos;
			tempCameraRot = resultCameraRot;
			transform.position = Vector3.Slerp(transform.position, tempCameraPos, cameraMoveSmoothingTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, tempCameraRot, cameraRotSmoothingTime);
			Debug.Log("pos: " + transform.position);
			if(Vector3.Distance(transform.position, tempCameraPos) < 0.01f){
				Debug.Log("saved");
				saveDirection = false;	
			}
		}
		else{
			transform.position = resultCameraPos;
			transform.rotation = resultCameraRot;
		}
		/*if(saveDirection){
			tempTarget = player.position;// - tempTargetOffset;
			expectedTarget = Vector3.Slerp(expectedTarget, savedPlayerPos, cameraMoveSmoothingTime);
			tempTarget = Vector3.Slerp(tempTarget, player.position, cameraMoveSmoothingTime);
			//tempTargetOffset = Vector3.Slerp(tempTargetOffset, Vector3.zero, cameraMoveSmoothingTime);
			if(Vector3.Distance(tempTarget, player.position) < 0.0f){
				Debug.Log("saved");
				//saveDirection = false;
			}
			testObj.transform.position = tempTarget;
		}
		else{
			testObj.transform.position = player.position;
		}*/
	}
	
	void LoadDirection(){
		saveDirection = true;	
		savedDirection = transform.rotation;
		tempTarget = transform.position + transform.forward * savedDistanceForDirection;
		expectedTarget = player.position - transform.forward * savedDistanceForDirection;
		savedPlayerPos = player.position;
		tempTargetOffset = player.position - tempTarget;
		Debug.Log("tempTargetOffset: " + tempTargetOffset);
		tempCameraPos = player.position - transform.forward * savedDistanceForDirection;
		tempCameraRot = transform.rotation;
		Debug.Log("pos : " + transform.position + " temp : " + tempCameraPos);
	}
	
	void SaveDirection(){
		Vector3 offsetToCamera = player.position - transform.position;
		Vector2 offsetToCamera2 = new Vector2(offsetToCamera.x, offsetToCamera.z);
		savedDistanceForDirection = offsetToCamera2.magnitude;
		Debug.Log("saved dist: " + savedDistanceForDirection);
	}
}
