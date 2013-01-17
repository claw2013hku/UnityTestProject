using UnityEngine;
using System.Collections;

public class touchExplosionTest : MonoBehaviour {
	public float restartTime = 3f;
	private float restartTimer = 0f;
	private bool restart = false;
	
	public float explosionForce;
	public float explosionRadius;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		if(Input.GetButton("Fire1")){
//			Ray ray = Camera.main.camera.ScreenPointToRay(Input.mousePosition);
//			Debug.DrawRay(ray.origin, ray.direction * 10, Color.green);
//		}
		if(restart){
			restartTimer += Time.deltaTime;
			if(restartTimer > restartTime){
				Application.LoadLevel(0);	
			}
		}
		else{
			if(Input.GetButtonDown("Fire1")){
				Ray ray = Camera.main.camera.ScreenPointToRay(Input.mousePosition);
				Debug.DrawRay(ray.origin, ray.direction * 10, Color.green);
				LayerMask mask = 1 << LayerMask.NameToLayer("PlayerRagdoll");
				RaycastHit hit;
				if (Physics.Raycast(ray.origin, ray.direction , out hit, 100f, mask)) {
					RagdollTurner turner = GetComponent<RagdollTurner>();
					turner.TurnRagdoll(hit.point, explosionForce, explosionRadius);
					restart = true;
				}
			}
		}
	}
	
	void OnGUI(){
		if(restart){
			GUILayout.Label("Time to restart : " + (restartTime - restartTimer));	
		}
	}
}
