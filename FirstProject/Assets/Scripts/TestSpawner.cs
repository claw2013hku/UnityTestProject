using UnityEngine;
using System.Collections;


public class TestSpawner : MonoBehaviour {
	public Transform testObject;

	// Use this for initialization
	void Start () {
		Instantiate(testObject, transform.position, transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
