using UnityEngine;
using System.Collections;
[RequireComponent (typeof (GUIText))]

public class FPSDisplayer : MonoBehaviour {
	float frames = 0;
	float timer = 0;
	// Use this for initialization
	void Start () {
		guiText.text = "FPS: --";
	}
	
	// Update is called once per frame
	void Update () {
		frames++;
		timer += Time.deltaTime;
		if(timer >= 2f){
			float fps = Mathf.Round(frames / timer);
			guiText.text = "FPS: " + fps;
			frames = 0;
			timer = 0;
		}
	}
}
