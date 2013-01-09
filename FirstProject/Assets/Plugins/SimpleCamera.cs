using UnityEngine;
using System.Collections;

public class SimpleCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 position = Camera.main.transform.position;
		position += Camera.main.transform.right * ControlSchemeInterface.instance.GetAxis(ControlAxis.MOVE_X) * 0.5f;
		position += Camera.main.transform.forward * ControlSchemeInterface.instance.GetAxis(ControlAxis.MOVE_Y) * 0.5f;
		position += Camera.main.transform.up * ((Input.GetKey(KeyCode.E) ? 1f : 0f) + (Input.GetKey(KeyCode.Q) ? -1f : 0f)) * 0.5f;
		Camera.main.transform.localPosition = position;
		Vector3 rotation = Camera.main.transform.rotation.eulerAngles;
		rotation.y += ControlSchemeInterface.instance.GetAxis(ControlAxis.CAMERA_SCROLL_X);
		rotation.x -= ControlSchemeInterface.instance.GetAxis(ControlAxis.CAMERA_SCROLL_Y);
		Camera.main.transform.rotation = Quaternion.Euler(rotation);
	}
}
