using UnityEngine;
using System;
using System.Collections;
using Sfs2X.Entities.Data;

//Stores base stats, modifiers, modified stats
//Fires (resulting) base stats changed event, modifiers changed event, modified stats changed event
public class CharacterPositionEffectorComponent : MonoBehaviour {	
	//Base stats
	public Vector3 moveDirection;
	public Vector3 MoveDirection {set {if(value != moveDirection){ if(HasChangedMoveDirection != null){HasChangedMoveDirection(moveDirection, value);} moveDirection = value;}} get {return moveDirection;}} 
	
	public Vector3 resultantPosition;
	public Vector3 ResultantPosition {set {resultantPosition = value;} get {return resultantPosition;}}
	public Quaternion resultantQuaternion;
	public Quaternion ResultantQuaternion {set {resultantQuaternion = value;} get {return resultantQuaternion;}}
	
	//delegates
	public delegate void MoveDirectionChange(Vector3 oldVal, Vector3 newVal);
	public event MoveDirectionChange HasChangedMoveDirection;
	
	//Public methods
	
	// Stores the transform values to SFSObject to send them to server
	public static void ToSFSObject(CharacterPositionEffectorComponent component, bool isDirection, ISFSObject data) {
		ISFSObject tr = new SFSObject();
		if(isDirection){
			tr.PutDouble("x", Convert.ToDouble(component.MoveDirection.x));	
			tr.PutDouble("y", Convert.ToDouble(component.MoveDirection.y));
			tr.PutDouble("z", Convert.ToDouble(component.MoveDirection.z));
			
			tr.PutLong("t", Convert.ToInt64(0));
			
			data.PutSFSObject("character_position_movement", tr);
		}
		else{
			tr.PutDouble("x", Convert.ToDouble(component.ResultantPosition.x));
			tr.PutDouble("y", Convert.ToDouble(component.ResultantPosition.y));
			tr.PutDouble("z", Convert.ToDouble(component.ResultantPosition.z));
			
			tr.PutDouble("rx", Convert.ToDouble(component.ResultantQuaternion.eulerAngles.x));
			tr.PutDouble("ry", Convert.ToDouble(component.ResultantQuaternion.eulerAngles.y));
			tr.PutDouble("rz", Convert.ToDouble(component.ResultantQuaternion.eulerAngles.z));
			
			tr.PutLong("t", Convert.ToInt64(0));
				
			data.PutSFSObject("character_position_resultant", tr);
		}	
	}
	
	public static void ToSFSObject(NetworkResultant result, ISFSObject data){
		ISFSObject tr = new SFSObject();
		tr.PutDouble("x", Convert.ToDouble(result.position.x));
		tr.PutDouble("y", Convert.ToDouble(result.position.y));
		tr.PutDouble("z", Convert.ToDouble(result.position.z));
		
		tr.PutDouble("rx", Convert.ToDouble(result.rotation.eulerAngles.x));
		tr.PutDouble("ry", Convert.ToDouble(result.rotation.eulerAngles.y));
		tr.PutDouble("rz", Convert.ToDouble(result.rotation.eulerAngles.z));
		
		tr.PutLong("t", Convert.ToInt64(0));
			
		data.PutSFSObject("character_position_resultant", tr);	
	}
	
	public static void ToSFSObject(NetworkMoveDirection result, ISFSObject data){
		ISFSObject tr = new SFSObject();
		tr.PutDouble("x", Convert.ToDouble(result.moveDirection.x));
		tr.PutDouble("y", Convert.ToDouble(result.moveDirection.y));
		tr.PutDouble("z", Convert.ToDouble(result.moveDirection.z));
		
		tr.PutLong("t", Convert.ToInt64(0));
			
		data.PutSFSObject("character_position_movement", tr);	
	}

	//Network counterparts
	public class NetworkMoveDirection : Interpolatable<NetworkMoveDirection>{
		public Vector3 moveDirection;
		public double timeStamp;
		
		public double GetTimeStamp (){
			return timeStamp;
		}
		
		public NetworkMoveDirection Interpolate(NetworkMoveDirection rhs, double time){
			return this;	
		}
		
		public NetworkMoveDirection Extrapolate(double time){
			return this;	
		}
		
		public bool IsDifferent(CharacterPositionEffectorComponent comp, float accuracy) {
			float posDif = Vector3.Distance(this.moveDirection, comp.MoveDirection);
			return (posDif>accuracy);
		}
		
		public static NetworkMoveDirection FromComponent(CharacterPositionEffectorComponent comp){
			NetworkMoveDirection d = new NetworkMoveDirection();
			d.moveDirection = comp.MoveDirection;
			return d;
		}
	}
	
	public class NetworkResultant : Interpolatable<NetworkResultant>{
		public Vector3 position;
		public Quaternion rotation;
		public double timeStamp;
		
		public double GetTimeStamp (){
			return timeStamp;
		}
		
		public NetworkResultant Interpolate(NetworkResultant rhs, double time){
			return this;	
		}
		
		public NetworkResultant Extrapolate(double time){
			return this;	
		}
		
		public bool IsDifferent(CharacterPositionEffectorComponent result, float accuracy) {
			float posDif = Vector3.Distance(this.position, result.ResultantPosition);
			float angDif = Vector3.Distance(this.rotation.eulerAngles, result.ResultantQuaternion.eulerAngles);
			
			return (posDif>accuracy || angDif > accuracy);
		}
		
		public static NetworkResultant FromComponent(CharacterPositionEffectorComponent comp){
			NetworkResultant result = new NetworkResultant();
			result.position = comp.ResultantPosition;
			result.rotation = comp.ResultantQuaternion;
			return result;
		}
	}

	public static NetworkMoveDirection MoveDirFromSFSObject(ISFSObject data){
		NetworkMoveDirection md = new NetworkMoveDirection();
		ISFSObject transformData = data.GetSFSObject("character_position_movement");
		
		float x = Convert.ToSingle(transformData.GetDouble("x"));
		float y = Convert.ToSingle(transformData.GetDouble("y"));
		float z = Convert.ToSingle(transformData.GetDouble("z"));
		
		md.moveDirection = new Vector3(x, y, z);
				
		if (transformData.ContainsKey("t")) {
			md.timeStamp = Convert.ToDouble(transformData.GetLong("t"));
		}
		else {
			md.timeStamp = 0;
		}
		return md;
	}
	
	public static NetworkResultant ResultantFromSFSObject(ISFSObject data){
		NetworkResultant trans = new NetworkResultant();
		ISFSObject transformData = data.GetSFSObject("character_position_resultant");
		
		float x = Convert.ToSingle(transformData.GetDouble("x"));
		float y = Convert.ToSingle(transformData.GetDouble("y"));
		float z = Convert.ToSingle(transformData.GetDouble("z"));
		
		float rx = Convert.ToSingle(transformData.GetDouble("rx"));
		float ry = Convert.ToSingle(transformData.GetDouble("ry"));
		float rz = Convert.ToSingle(transformData.GetDouble("rz"));
					
		trans.position = new Vector3(x, y, z);
		trans.rotation = Quaternion.Euler(rx, ry, rz);
				
		if (transformData.ContainsKey("t")) {
			trans.timeStamp = Convert.ToDouble(transformData.GetLong("t"));
		}
		else {
			trans.timeStamp = 0;
		}
		return trans;
	}
	
}
