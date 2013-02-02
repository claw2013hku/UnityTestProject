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
	public Vector3 resultantVelocity;
	public Vector3 ResultantVelocity {set {resultantVelocity = value;} get {return resultantVelocity;}}
		
	//delegates
	public delegate void MoveDirectionChange(Vector3 oldVal, Vector3 newVal);
	public event MoveDirectionChange HasChangedMoveDirection;
	
	//Public methods
	public static void ToSFSObject(NetworkResultant result, ISFSObject data){
		ISFSObject tr = new SFSObject();
		tr.PutDouble("x", Convert.ToDouble(result.position.x));
		tr.PutDouble("y", Convert.ToDouble(result.position.y));
		tr.PutDouble("z", Convert.ToDouble(result.position.z));
		tr.PutDouble("vx", Convert.ToDouble(result.velocity.x));
		tr.PutDouble("vy", Convert.ToDouble(result.velocity.y));
		tr.PutDouble("vz", Convert.ToDouble(result.velocity.z));
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
		
		public void SetTimeStamp(double time){
			timeStamp = time;	
		}
		
		public void Assign(NetworkMoveDirection t){
			moveDirection = t.moveDirection;
			timeStamp = t.timeStamp;
		}
		
		public static void Interpolate(int index, NetworkMoveDirection[] buffer, int size, double interpolationTime, ref NetworkMoveDirection result){
			NetworkMoveDirection rhs = buffer[index];
			if(index + 1 >= size){
				return;
			}
			NetworkMoveDirection lhs = buffer[index + 1];
			
			double length = (rhs.timeStamp - lhs.timeStamp) / 1000f;
			float t = 0.0f;
			if (length > 0.0001) {
				t = (float)((interpolationTime - lhs.timeStamp) / length);
			}		
			result.moveDirection = Vector3.Lerp(lhs.moveDirection, rhs.moveDirection, t);
		}
	
		public static void Extrapolate(int index, NetworkMoveDirection[] buffer, int size, float extrapolationLength, ref NetworkMoveDirection result){
			if(extrapolationLength <= 0.1f){
				result.moveDirection = buffer[index].moveDirection;
			}
			else{
				result.moveDirection = Vector3.zero;			
			}
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
		public Vector3 position = new Vector3(148, 31, 230);
		public Quaternion rotation;
		public Vector3 velocity;
		public double timeStamp;
		
		public double GetTimeStamp (){
			return timeStamp;
		}
		
		public void SetTimeStamp(double time){
			timeStamp = time;	
		}
		
		public void Assign(NetworkResultant t){
			Assign (this, t);	
		}
		
		public static void Assign(NetworkResultant lhs, NetworkResultant rhs){
			lhs.position = rhs.position;
			lhs.velocity = rhs.velocity;
			lhs.rotation = rhs.rotation;
			lhs.timeStamp = rhs.timeStamp;
		}

		public static void Interpolate(int index, NetworkResultant[] buffer, int size, double interpolationTime, ref NetworkResultant result){
			NetworkResultant rhs = buffer[index];
			if(index + 1 >= size){
				Assign(result, rhs);
				return;
			}
			
			NetworkResultant lhs = buffer[index + 1];
			
			// Use the time between the two slots to determine if interpolation is necessary
			float length = (float)(rhs.timeStamp - lhs.timeStamp) / 1000f;

			Vector3 backwardsAcceleration = (lhs.velocity - rhs.velocity) * (1f / length);
			
			float backwardsTime = (float)(rhs.timeStamp / 1000 - interpolationTime);
			// s = ut + 0.5at^2
			Vector3 backwardsDistance = rhs.velocity * backwardsTime + 0.5f * backwardsAcceleration * backwardsTime * backwardsTime;
			// compensation between normal interpolation and velocity interpolation
			if(backwardsDistance.sqrMagnitude > (rhs.position - lhs.position).sqrMagnitude){
				backwardsDistance *= ((rhs.position - lhs.position).magnitude / backwardsDistance.magnitude);
			}
			result.position = rhs.position - backwardsDistance;
			
			float t = 0.0F;
			if (length > 0.0001f) {
				t = (float)((interpolationTime - lhs.timeStamp) / length);
			}
	
			result.rotation = Quaternion.Lerp(lhs.rotation, rhs.rotation, t);
		}
		
		public static void Extrapolate(int index, NetworkResultant[] buffer, int size, float extrapolationLength, ref NetworkResultant result){
			result.rotation = buffer[index].rotation;
			result.position = buffer[index].position + buffer[index].velocity * extrapolationLength;
		}
		
		public bool IsDifferent(CharacterPositionEffectorComponent result, float accuracy) {
			float posDif = Vector3.Distance(this.position, result.ResultantPosition);
			float angDif = Vector3.Distance(this.rotation.eulerAngles, result.ResultantQuaternion.eulerAngles);
			
			return (posDif>accuracy || angDif > accuracy);
		}
		
		public static NetworkResultant FromComponent(CharacterPositionEffectorComponent comp){
			NetworkResultant result = new NetworkResultant();
			result.position = comp.ResultantPosition;
			result.velocity = comp.ResultantVelocity;
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
		
		float vx = Convert.ToSingle(transformData.GetDouble("vx"));
		float vy = Convert.ToSingle(transformData.GetDouble("vy"));
		float vz = Convert.ToSingle(transformData.GetDouble("vz"));
		
		float rx = Convert.ToSingle(transformData.GetDouble("rx"));
		float ry = Convert.ToSingle(transformData.GetDouble("ry"));
		float rz = Convert.ToSingle(transformData.GetDouble("rz"));	
		
		trans.position = new Vector3(x, y, z);
		trans.velocity = new Vector3(vx, vy, vz);
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
