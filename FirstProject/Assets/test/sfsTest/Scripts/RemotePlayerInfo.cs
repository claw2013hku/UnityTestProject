
using System;
using System.Collections;
using UnityEngine;

// Displaying enemy info like name and health
public class RemotePlayerInfo : MonoBehaviour
{
	public TextMesh name;
	public Color color;
	public Color team1Color;
	public Color team2Color;
	
	private Renderer[] renderers;
	
	void Awake() {
		renderers = GetComponentsInChildren<Renderer>();
		transform.root.gameObject.GetComponent<ActorStatusComponent>().HasChangedStatus += HandleHasChangedStatus;
		SetColor(color);
	}

	void HandleHasChangedStatus (bool isBase, ActorStatusComponent.StatusType type, float oldVal, float newVal){
		if(type == ActorStatusComponent.StatusType.TEAM){
			SetColor((int)newVal);	
		}
	}
	
	public void SetName(string name) {
		this.name.text = name;
	}
	
	public void SetColor(int i){
		if(i == 1){
			SetColor(team1Color);	
		}
		else if (i == 2){
			SetColor(team2Color);	
		}
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

