using UnityEngine;
using System;
using System.Collections;
using Sfs2X.Entities.Data;

public class CharAnimEffComp : MonoBehaviour {
	public static string RunAnimationName = "Base Layer.Run Blend Tree";
	public static string IdleAnimationName = "Base Layer.Idle";
	public static string Slash1AnimationName = "Slash.Slash1";	
	public static string Slash2AnimationName = "Slash.Slash2";
	
	public int stateInfoNameHash;
	public int StateInfoNameHash{
		get{return stateInfoNameHash;}
		set{
			if(stateInfoNameHash != value){
				if(HasChangedAnimHash != null){
					HasChangedAnimHash(stateInfoNameHash, value);	
				}
				stateInfoNameHash = value;	
			}
		}
	}
	
	public struct AnimState{
		public bool Slash;
		public int SlashVariant;
	}
	
	public AnimState state;
	public AnimState State{
		get {return state;}
		set{
			if(state.Slash != value.Slash || state.SlashVariant != value.SlashVariant){
				if(HasChangedAnimState != null){
					HasChangedAnimState(state, value);
				}
				state = value;
			}
		}
	}
	
	public float swing;
	public float Swing{
		get {return swing;}
		set{
			if(swing != value){
				if(HasSwung != null){
					HasSwung(swing, value);	
				}
				swing = value;
			}
		}
	}
	
	//delegates
	public delegate void HashChange(int oldVal, int newVal);
	public event HashChange HasChangedAnimHash;
	public delegate void StateChange(AnimState oldVal, AnimState newVal);
	public event StateChange HasChangedAnimState;
	public delegate void SwingChange(float oldVal, float newVal);
	public event SwingChange HasSwung;
			
	public class NetworkResultantState : Interpolatable<NetworkResultantState>{
		public int nameHash;
		public AnimState state;
		public double timeStamp;
		
		public double GetTimeStamp (){
			return timeStamp;
		}
		
		public void SetTimeStamp(double time){
			timeStamp = time;	
		}
		
		public void Assign(NetworkResultantState t){
			nameHash = t.nameHash;
			state = t.state;
			timeStamp = t.timeStamp;
		}
		
		public static void Interpolate(int index, NetworkResultantState[] buffer, int size, double interpolationTime, ref NetworkResultantState result){
			result.Assign(buffer[size > index + 1 ? index + 1 : index]);
			//Debug.Log ("inter time: " + interpolationTime + ", stamp time: " + );
			//Debug.Log ("index: " + index + ", inter result: " + buffer[size > index + 1 ? index + 1 : index].state.Slash);
		}
	
		public static void Extrapolate(int index, NetworkResultantState[] buffer, int size, float extrapolationLength, ref NetworkResultantState result){
			result.Assign(buffer[index]);
		}
		
		public static void FromComponent(CharAnimEffComp comp, ref NetworkResultantState result){
			result.nameHash = comp.StateInfoNameHash;
			result.state = comp.State;
		}
		
		public void ToSFSObject(ISFSObject data){
			ISFSObject tr = new SFSObject();
			tr.PutInt("nameHash", nameHash);
			
			tr.PutBool("Slash", state.Slash);
			tr.PutInt("SlashVariant", state.SlashVariant);
			tr.PutLong("t", Convert.ToInt64(0));
				
			data.PutSFSObject("charAnimCompState", tr);	
		}
		
		public static NetworkResultantState FromSFSObject(ISFSObject data){
			NetworkResultantState md = new NetworkResultantState();
			ISFSObject animObj = data.GetSFSObject("charAnimCompState");
			
			int nameHash = animObj.GetInt("nameHash");
			md.nameHash = nameHash;
			
			bool slash = animObj.GetBool("Slash");
			md.state.Slash = slash;
			
			int slashV = animObj.GetInt("SlashVariant");
			md.state.SlashVariant = slashV;
			
			if (animObj.ContainsKey("t")) {
				md.timeStamp = Convert.ToDouble(animObj.GetLong("t"));
			}
			else {
				md.timeStamp = 0;
			}
			return md;
		}
	}
}
