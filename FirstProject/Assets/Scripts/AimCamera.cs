using UnityEngine;
using System.Collections;

public class AimCamera : MonoBehaviour {
	public Transform player;
	
	public float smoothingTime = 0.5f;
	
	public Vector3 pivotOffset = new Vector3(1.3f, 0.4f,  0.0f);
	public Vector3 camOffset   = new Vector3(0.0f, 0.7f, -2.4f);
	public Vector3 closeOffset = new Vector3(0.35f, 1.7f, 0.0f);
	
	public float horizontalAimingSpeed = 270f;
	public float verticalAimingSpeed = 270f;
	public float maxVerticalAngle = 80f;
	public float minVerticalAngle = -80f;
	
	public float mouseSensitivity = 0.1f;
	
	[HideInInspector]
	public Vector3 position;
	[HideInInspector]
	public Quaternion rotation;
	
	private float angleH = 0;
	private float angleV = 0;
	private float maxCamDist = 1;
	private LayerMask mask;
	private Vector3 smoothPlayerPos;
	
	public Texture reticle;
	
	// Use this for initialization
	void Start () {
		// Add player's own layer to mask
		mask = 1 << player.gameObject.layer;
		// Invert mask
		mask = ~mask;
		
		position = transform.position;
		rotation = transform.rotation;
		smoothPlayerPos = player.position;
		
		maxCamDist = 3;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		/*if (Time.deltaTime == 0 || Time.timeScale == 0 || player == null) 
			return;*/
		
		angleH += Mathf.Clamp(ControlSchemeInterface.instance.GetAxis(ControlAxis.CAMERA_SCROLL_X), -1, 1) * horizontalAimingSpeed * Time.deltaTime;
		angleV += Mathf.Clamp(ControlSchemeInterface.instance.GetAxis(ControlAxis.CAMERA_SCROLL_Y), -1, 1) * verticalAimingSpeed * Time.deltaTime;
		// limit vertical angle
		angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);
		
		Apply();
	}
	
	void OnGUI () {
		if (Time.time != 0 && Time.timeScale != 0)
			GUI.DrawTexture(new Rect(Screen.width/2-(reticle.width*0.5f), Screen.height/2-(reticle.height*0.5f), reticle.width, reticle.height), reticle);
	}
	
	public void SetCurrentAngle(){
		angleH = transform.rotation.eulerAngles.y;
		angleV = transform.rotation.eulerAngles.x > 180 ? transform.rotation.eulerAngles.x - 360 : transform.rotation.eulerAngles.x;
		angleV = -angleV;
	}
	
	public void Apply(){
		// Set aim rotation
		Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
		Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
		//cam.rotation = Quaternion.Lerp(cam.rotation, aimRotation, 1);
		rotation = aimRotation;
		// Find far and close position for the camera
		smoothPlayerPos = Vector3.Slerp(smoothPlayerPos, player.position, smoothingTime * Time.deltaTime);
		smoothPlayerPos.x = player.position.x;
		smoothPlayerPos.z = player.position.z;
		Vector3 farCamPoint = smoothPlayerPos + camYRotation * pivotOffset + aimRotation * camOffset;
		Vector3 closeCamPoint = player.position + camYRotation * closeOffset;
		float farDist = Vector3.Distance(farCamPoint, closeCamPoint);
		
		// Smoothly increase maxCamDist up to the distance of farDist
		maxCamDist = Mathf.Lerp(maxCamDist, farDist, 5 * Time.deltaTime);
		
		// Make sure camera doesn't intersect geometry
		// Move camera towards closeOffset if ray back towards camera position intersects something 
		RaycastHit hit;
		Vector3 closeToFarDir = (farCamPoint - closeCamPoint) / farDist;
		float padding = 0.3f;
		if (Physics.Raycast(closeCamPoint, closeToFarDir, out hit, maxCamDist + padding, mask)) {
			maxCamDist = hit.distance - padding;
			Debug.Log("hit");
		}
		
		position = closeCamPoint + closeToFarDir * maxCamDist;
	}
}
