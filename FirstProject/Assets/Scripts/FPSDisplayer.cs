using UnityEngine;
using System.Collections;
[RequireComponent (typeof (GUIText))]

public class FPSDisplayer : MonoBehaviour {
	public WindowPad windowPad;
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
			if(windowPad != null){
				guiText.text += " finger delta : " + windowPad.GetAnyDeltaPositions();
			}
			frames = 0;
			timer = 0;
		}
	}
}
