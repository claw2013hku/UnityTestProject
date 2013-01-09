using UnityEngine;
using System.Collections;

public class projectileDemoSwitch : MonoBehaviour {
	public GameObject shootCamera;
	public GameObject playCamera;
	public Transform player;
	private bool shooting = true;
	// Use this for initialization
	void Start () {
		shootCamera.SetActive(true);
		playCamera.SetActive(false);
		player.GetComponent<IdleRunJump>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			shooting = !shooting;
			if(shooting){
				shootCamera.SetActive(true);
				playCamera.SetActive(false);
				player.GetComponent<IdleRunJump>().enabled = false;
			}
			else{
				shootCamera.SetActive(false);
				playCamera.SetActive(true);
				player.GetComponent<IdleRunJump>().enabled = true;
			}
		}
	}
}
