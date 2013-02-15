
using System;
using System.Collections;
using UnityEngine;

// Displaying enemy info like name and health
public class RemotePlayerInfo : MonoBehaviour
{
	public TextMesh name;
	public Color color;
	
	private Renderer[] renderers;
	
	void Awake() {
		renderers = this.GetComponentsInChildren<Renderer>();
		SetColor(color);
	}
	
	public void SetName(string name) {
		this.name.text = name;
	}
	
	public void SetColor(Color _color){
		color = _color;
		name.renderer.material.color = _color;
	}
	
	public void Hide() {
		foreach (Renderer rend in renderers) {
			rend.enabled = false;
		}
	}
	
	public void Show() {
		foreach (Renderer rend in renderers) {
			rend.enabled = true;
		}
	}
	
	void LateUpdate() {
		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back,
            Camera.main.transform.rotation * Vector3.up);
	}
	
}

