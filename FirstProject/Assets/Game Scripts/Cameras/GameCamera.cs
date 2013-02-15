using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {
	public Transform focusTransform;
	
	//used for when jumping
	public bool freezeVerticalRotation = false;
	public bool freezeVerticalPosition = false;
	
	//boundaries
	public float maxHeight = 5f;
	public float minHeight = 0f;
	public bool dontClampHeight = false;
	
	public float maxDist = 4f;
	public float minDist = 1f;
	public bool dontClampDist = false;
	
	public float maxVerticalAngle = 80f;
	public float minVerticalAngle = -80f;
	public bool dontClampVertAng = false;
	
	//speed
	public float horizontalAimingSpeed = 1f;
	public float verticalAimingSpeed = 1f;
	
	//smoothing
	public bool smoothHeight = true;
	private float smoothHeightVelocity = 0f;
	public float smoothHeightLag = 0.3f;
	public bool smooth2dDistance = true;
	private float smooth2dDistanceVelocity = 0f;
	public float smooth2dDistanceLag = 0.3f;
	
	public bool[] smoothRotation = {true, true};
	public float[] smoothRotationLag = {0.1f, 0.1f};
	public float[] smoothRotationMaxSpeed = {150f, 150f};
	private float[] smoothRotationVelocity = {0f, 0f};
	
	//position for this camera
	[HideInInspector]
	public Vector3 position;
	[HideInInspector]
	public Quaternion rotation;
	
	//for preserving height distance
	private float lastHeight;
	//for raycasting against geometry
	private LayerMask mask;
	public float raycastPadding = 10f;
	
	//for preserving distance after colliding geometry
	private bool collidedGeometry = false;
	private float storedDist = 0f;
	
	void Start () {
		//Initialize mask to not collide with some layers
		mask = 1 << LayerMask.NameToLayer("Player");
		mask |= ( 1 << LayerMask.NameToLayer("Ignore Raycast"));
		mask |= ( 1 << LayerMask.NameToLayer("PlayerRagdoll"));
		mask = ~mask;
		position = transform.position;
		rotation = transform.rotation;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Time.deltaTime == 0 || Time.timeScale == 0 || focusTransform == null) 
		{
			return;
		}
		
		Vector3	targetCenter = focusTransform.position;	
		
		//Determine and clamp camera distance from player
		float targetDistance;
		if(collidedGeometry){
			targetDistance = storedDist;
		}
		else{
			targetDistance = Vector3.Distance(targetCenter, position);
		}
		
		//Determine resultant position for camera
		Vector3 targetPosition = position;
		
		//Preserve height bt focus and camera
		if(!freezeVerticalPosition){
			targetPosition.y = targetCenter.y + lastHeight;
		}
		
		//Have the camera look at the focus to get the initial x & y rotation in this frame
		Quaternion direction = Quaternion.LookRotation(targetPosition - targetCenter, Vector3.up);
		
		//extract and change by input the x rotation
		float xRot = direction.eulerAngles.x > 180 ? direction.eulerAngles.x - 360 : direction.eulerAngles.x;
		xRot += -(ControlSchemeInterface.instance.GetAxis(ControlAxis.CAMERA_SCROLL_Y)) * verticalAimingSpeed;
		if(!dontClampVertAng){
			if(!freezeVerticalRotation){
				xRot = Mathf.Clamp(xRot, minVerticalAngle, maxVerticalAngle);
			}
		}
		xRot = xRot < 0 ? 360 + xRot : xRot;
		
		//extract and change by input the y rotation
		float yRot = direction.eulerAngles.y;
		yRot += (ControlSchemeInterface.instance.GetAxis(ControlAxis.CAMERA_SCROLL_X)) * horizontalAimingSpeed;
		
		//rotation the camera around the new x and y angles
		Quaternion newDirection = 
			Quaternion.Euler(
				xRot, 
				yRot, 
				direction.eulerAngles.z);
		targetPosition = targetCenter + newDirection * Vector3.forward * targetDistance;
		
		//Clamp height of camera relative to player
		if(!dontClampHeight){
			float newTargetHeight = targetPosition.y - targetCenter.y;
			float tempTargetHeight = Mathf.Clamp(newTargetHeight, minHeight, maxHeight);
//			if(newTargetHeight > maxHeight){
//				targetPosition.y = targetCenter.y + maxHeight;
//			}
//			else if (newTargetHeight < minHeight){
//				targetPosition.y = targetCenter.y + minHeight;
//			}
			if(tempTargetHeight != newTargetHeight){
				if(smoothHeight){
					targetPosition.y = targetCenter.y + Mathf.SmoothDamp(newTargetHeight, tempTargetHeight, ref smoothHeightVelocity, smoothHeightLag);		
				}
				else{
					targetPosition.y = targetCenter.y + tempTargetHeight;	
				}
			}
		}
		
		//Clamp the 2D distance relative to player in a way height is not affected
		if(!dontClampDist){
			bool XZDistClamped = true;
			
			//determine the distance in 2D
			Vector2 targetCenter2D = new Vector2(targetCenter.x, targetCenter.z);
			Vector2 targetPosition2D = new Vector2(targetPosition.x, targetPosition.z);
			Vector2 direction2D = targetPosition2D - targetCenter2D;
			float distanceXZ = direction2D.magnitude;
			direction2D.Normalize();
			if(distanceXZ > maxDist){
				targetPosition2D = targetCenter2D + direction2D * (smooth2dDistance ? Mathf.SmoothDamp(distanceXZ, maxDist, ref smooth2dDistanceVelocity, smooth2dDistanceLag) : maxDist);
			}
			else if (distanceXZ < minDist){
				//induce control errors
//				if(smooth2dDistance){
//					float smoothedDist = Mathf.SmoothDamp(distanceXZ, minDist, ref smooth2dDistanceVelocity, smooth2dDistanceLag);
//					smoothedDist = Mathf.Clamp(smoothedDist, 0.5f, smoothedDist);
//					targetPosition2D = targetCenter2D + direction2D * smoothedDist;
				//}
				//else{
					targetPosition2D = targetCenter2D + direction2D * minDist;		
				//}
			}
			else{
				XZDistClamped = false;	
			}
			
			//apply the changed position from 2D back to 3D
			if(XZDistClamped){
				targetPosition = new Vector3(targetPosition2D.x, targetPosition.y, targetPosition2D.y);
			}
		}
		
		// Make sure camera doesn't intersect geometry
		// Move camera towards player if ray back towards camera position intersects something 
		RaycastHit hit;
		Vector3 rayDirection = targetPosition - targetCenter;
		float rayDistance = rayDirection.magnitude;
		rayDirection.Normalize();
		if (Physics.Raycast(targetCenter, rayDirection , out hit, rayDistance, mask)) {
			//intersection begins, save the distance for restoring the uncollided distance afterwards
			if(!collidedGeometry){
				storedDist = hit.distance;
				collidedGeometry = true;
			}
			targetPosition = targetCenter + rayDirection * (hit.distance - raycastPadding);
		}
		else{
			collidedGeometry = false;
		}
		
		position = targetPosition;
		
		//apply angular smoothing
		Vector3 cameraDirection = targetCenter - position;
		Quaternion targetRotation = Quaternion.LookRotation(cameraDirection, Vector3.up);
		if(smoothRotation[0] || smoothRotation[1]){
			rotation = Quaternion.Euler(
			smoothRotation[0] ? Mathf.SmoothDampAngle(rotation.eulerAngles.x, targetRotation.eulerAngles.x, ref smoothRotationVelocity[0], smoothRotationLag[0], smoothRotationMaxSpeed[0]) : targetRotation.eulerAngles.x,
			smoothRotation[1] ? Mathf.SmoothDampAngle(rotation.eulerAngles.y, targetRotation.eulerAngles.y, ref smoothRotationVelocity[1], smoothRotationLag[1], smoothRotationMaxSpeed[1]) : targetRotation.eulerAngles.y,
			targetRotation.eulerAngles.z);	
		}
		else{
			rotation = targetRotation;	
		}
		
		lastHeight = position.y - targetCenter.y;
	}
	
	public void ResetDirection(Quaternion direction){
		if(focusTransform == null) return;
		float distance = Vector3.Distance(position, focusTransform.position);
		position = focusTransform.position - direction * Vector3.forward * (distance == 0f ? 0.5f : distance);
		rotation = direction;		
	}
	
	public void Reset(Vector3 _position, Quaternion _rotation, bool _dontClamp = false){
		position = _position;
		rotation = _rotation;
		dontClampDist = _dontClamp;
		dontClampVertAng = _dontClamp;
		dontClampHeight = _dontClamp;
	}
}
