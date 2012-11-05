using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;
	
	public Transform target;
	public Transform camera;
	
	float rotationY = 0F;

	void Update ()
	{
		if (axes == RotationAxes.MouseXAndY)
		{
			/*float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);*/
			//transform.RotateAround(target.transform.position, Vector3.up, Input.GetAxis("Mouse X") * sensitivityX);
			//transform.RotateAround(target.transform.position, camera.transform.right, Input.GetAxis("Mouse Y") * sensitivityY);
			
			//CharacterController controller = GetComponent<CharacterController>();
			//controller.Move(camera.right * -Input.GetAxis("Mouse X") * sensitivityX);
			//var newRotationY = rotationY + Input.GetAxis("Mouse Y") * sensitivityY;
			//var yAngle = Input.GetAxis("Mouse Y") * sensitivityY;
			//Debug.Log(Input.GetAxis("Mouse Y"));
			//if(!(yAngle > 0 && 60 < transform.localEulerAngles.x && transform.localEulerAngles.x < 180) &&
			//	!(yAngle < 0 && 180 < transform.localEulerAngles.x && transform.localEulerAngles.x < 358))
			//{
				//rotationY = newRotationY;
				//transform.RotateAround(target.transform.position, target.transform.right, 1f);//Input.GetAxis("Mouse Y") * sensitivityY);
				//controller.Move(camera.up * -Input.GetAxis("Mouse Y") * sensitivityY);
			//}
			//camera.rigidbody.velocity = Vector3.zero;
		}
		else if (axes == RotationAxes.MouseX)
		{
			//transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
			transform.RotateAround(target.transform.position, Vector3.up, Input.GetAxis("Mouse X") * sensitivityX);
			//Input.GetAxis("Mouse X") * sensitivityX
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
}