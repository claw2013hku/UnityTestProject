using UnityEngine;
using System.Collections;

public class NetSyncObj : MonoBehaviour {
	private int id;
	public int ID {get {return id;} set {id = value;}}
	
	public SFSNetworkManager.Mode mode = SFSNetworkManager.Mode.LOCAL;
}
