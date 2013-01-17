using UnityEngine;
using System.Collections;

public class TurnRagdoll : MonoBehaviour {
	public CharacterController character;
	public Collider[] ragdollParts;
	public float time = 0f;
	private float timer = 0f;
	private Vector3[] velocities;
	private Vector3[] positions;
	
	public float restartTime = 3f;
	private float restartTimer = 0f;
	private bool pendingRestart = false;
	// Use this for initialization
	void Start () {
		timer = 0f;
		velocities = new Vector3[ragdollParts.Length];
		positions = new Vector3[ragdollParts.Length];
	}
	
	void FixedUpdate(){
		for(int i = 0; i < ragdollParts.Length; i++){
			Vector3 newPosition = ragdollParts[i].transform.position;
			velocities[i] = (newPosition - positions[i]) / Time.deltaTime;
			positions[i] = newPosition;
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(timer > time && !pendingRestart){
			//Destroy((CharacterController)character);
			Destroy(GetComponent<Animator>());
			GetComponent<CharacterController>().enabled = false;
			Destroy(GetComponent<CharacterController>());
			
			for(int i = 0; i < ragdollParts.Length; i++){
				Collider part = ragdollParts[i];
				part.enabled = true;
				part.rigidbody.isKinematic = false;
				part.rigidbody.velocity = velocities[i];
				part.rigidbody.WakeUp();
			}
			pendingRestart = true;
		}
		
		if(pendingRestart){
			restartTimer += Time.deltaTime;
			if(restartTimer > restartTime){
				Application.LoadLevel(0);	
			}
		}
		else{
			
		}
	}
	
	void OnGUI(){
		if(pendingRestart){
			GUILayout.Label("Time to restart : " + (restartTime - restartTimer));	
		}
		else{
			GUILayout.Label("Time to live : " + (time - timer));		
		}
	}
}
