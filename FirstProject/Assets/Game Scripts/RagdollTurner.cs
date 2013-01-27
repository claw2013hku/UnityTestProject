using UnityEngine;
using System.Collections;

public class RagdollTurner : MonoBehaviour {
	private ArrayList velocities;
	private ArrayList positions;
	private Rigidbody[] ragdollBodies;
	private ArrayList localPositions;
	private ArrayList localRotations;
	
	private Animator charAnimator;
	private CharacterController charController;
	
	public bool turnRagdoll = false;
	private bool finishedTurningRagdoll = false;
	
	private bool hasExplosionForce = false;
	private float explosionForce = 0f;
	private Vector3 explosionPosition;
	private float explosionRadius;
	
	public bool testMode = false;
	public float time = 0f;
	private float timer = 0f;
	
	public float restartTime = 3f;
	private float restartTimer = 0f;
	private bool pendingRestart = false;
	
	public bool testExplosion = false;
	public float testExplosionForce = 0f;
	public Transform testExplosionPos;
	public float testExplosionRadius;
	
	// Use this for initialization
	void Start () {
		timer = 0f;
		
		ragdollBodies = GetComponentsInChildren<Rigidbody>();
		velocities = new ArrayList(ragdollBodies.Length);
		positions = new ArrayList(ragdollBodies.Length);
		localPositions = new ArrayList(ragdollBodies.Length);
		localRotations = new ArrayList(ragdollBodies.Length);
		for(int i = 0; i < ragdollBodies.Length; i++){
			velocities.Add(new Vector3());
			positions.Add (new Vector3());
			localPositions.Add(new Vector3());
			localRotations.Add(new Quaternion());
			Vector3 newPosition = ragdollBodies[i].transform.position;
			velocities[i] = 0;
			positions[i] = newPosition;
			localPositions[i] = ragdollBodies[i].transform.localPosition;
			localRotations[i] = ragdollBodies[i].transform.localRotation;
		}
		
		charAnimator = GetComponent<Animator>();
		charController = GetComponent<CharacterController>();
	}
	
	void FixedUpdate(){
		if(!turnRagdoll){
			for(int i = 0; i < ragdollBodies.Length; i++){
				Vector3 newPosition = ragdollBodies[i].transform.position;
				velocities[i] = (newPosition - (Vector3)positions[i]) / Time.deltaTime;
				positions[i] = newPosition;
			}
		}
	}
	
	// Update is called once per frame
	void Test () {
		timer += Time.deltaTime;
		if(timer > time && !pendingRestart){
			charAnimator.enabled = false;
			charController.enabled = false;
			for(int i = 0; i < ragdollBodies.Length; i++){
				((Rigidbody)(ragdollBodies[i])).isKinematic = false;
				((Rigidbody)(ragdollBodies[i])).velocity = (Vector3)velocities[i];
				((Rigidbody)(ragdollBodies[i])).WakeUp();
			}
			if(testExplosion){
				foreach(Rigidbody body in ragdollBodies){
					body.AddExplosionForce(testExplosionForce, testExplosionPos.position, testExplosionRadius, 0, ForceMode.Impulse);	
				}
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
	
	void Update(){
		if(testMode){
			Test ();
			return;
		}
		
		if(!finishedTurningRagdoll){
			if(turnRagdoll){
				charAnimator.enabled = false;
				charController.enabled = false;
				for(int i = 0; i < ragdollBodies.Length; i++){
					((Rigidbody)(ragdollBodies[i])).isKinematic = false;
					((Rigidbody)(ragdollBodies[i])).velocity = (Vector3)velocities[i];
					((Rigidbody)(ragdollBodies[i])).WakeUp();
				}
				if(hasExplosionForce){
					foreach(Rigidbody body in ragdollBodies){
						body.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, 0, ForceMode.Impulse);	
					}
				}
				finishedTurningRagdoll = true;
			}
		}
	}
	
	public void TurnRagdoll(){
		turnRagdoll = true;
		//hasExplosionForce = false;
	}
	
	public void TurnRagdoll(Vector3 _explosionPosition, float _explosionForce, float _explosionRadius){
		turnRagdoll = true;
		hasExplosionForce = true;
		explosionForce = _explosionForce;
		explosionPosition = _explosionPosition;
		explosionRadius = _explosionRadius;
	}
	
	public void ReadyExplosion(Vector3 _explosionPosition, float _explosionForce, float _explosionRadius){
		hasExplosionForce = true;
		explosionForce = _explosionForce;
		explosionPosition = _explosionPosition;
		explosionRadius = _explosionRadius;
	}
	
	public void UnreadyExplosion(){
		hasExplosionForce = false;
	}
	
	void OnGUI(){
		if(testMode){
			if(pendingRestart){
				GUILayout.Label("Time to restart : " + (restartTime - restartTimer));	
			}
			else{
				GUILayout.Label("Time to live : " + (time - timer));		
			}
		}
	}
	
	public void UnturnRagdoll(){
		turnRagdoll = false;
		charAnimator.enabled = true;
		charController.enabled = true;
		for(int i = 0; i < ragdollBodies.Length; i++){
			((Rigidbody)(ragdollBodies[i])).isKinematic = true;
			((Rigidbody)(ragdollBodies[i])).transform.localPosition = (Vector3)localPositions[i];
			((Rigidbody)(ragdollBodies[i])).transform.localRotation = (Quaternion)localRotations[i];
		}
		finishedTurningRagdoll = false;
	}
}
