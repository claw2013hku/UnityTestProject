using UnityEngine;
using System.Collections;

public class SpeedPlatformTest : MonoBehaviour {
	public float plusModifier1 = 0f;
	public float mulModifier2 = 1f;
	public float plusModifier3 = 0f;
	public float mulModifier4 = 1f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider col){
		ActorStatus status = col.GetComponent<ActorStatus>();
		if(status != null){
			SpeedModifierTest statusfx = (SpeedModifierTest) status.gameObject.AddComponent("SpeedModifierTest");
			statusfx.area = collider;
			statusfx.plusModifier1 = plusModifier1;
			statusfx.mulModifier2 = mulModifier2;
			statusfx.plusModifier3 = plusModifier3;
			statusfx.mulModifier4 = mulModifier4;	
			status.AttachStatusEffect(statusfx);
		}
	}
}
