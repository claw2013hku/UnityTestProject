using UnityEngine;
using System.Collections;

public class ActorStatus : MonoBehaviour {
	public enum ActorType {Test};
	
	public ActorType actorType;
	public float baseMaxHP;
	public float baseHP;
	public float baseMoveSpeed;
	public float baseDamage;
	public float baseAttackSpeed;
	
	public float[] MaxHPModifiers = {0, 1, 0, 1};
	public float[] HPModifiers = {0, 1, 0, 1};
	public float[] MoveSpeedModifiers = {0, 1, 0, 1};
	public float[] DamageModifiers = {0, 1, 0, 1};
	public float[] AttackSpeedModifiers = {0, 1, 0, 1};
	
	//motion = modifiedMoveSpeed? * modifier1 + modifier2
	public float motionModifier1m;
	public Vector3 motionModifier2p;
	
	public enum StatusType {TYPE, MAXHP, HP, MOVESPEED, DAMAGE, ATTACKSPEED}
	//private ArrayList statusEffects;
	private Hashtable statusEffects = new Hashtable();
	
	public bool showInGUI;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	private ArrayList keysToRemove = new ArrayList();
	void Update () {
		//reset modifiers
		ResetModifiers();
		//set modifiers by status effects and call detach on finished status effects
		foreach(DictionaryEntry effectEntry in statusEffects){
			ArrayList effects = (ArrayList) effectEntry.Value;
			foreach(IActorStatusEffect effect in effects){
				if(effect.IsDead()){
					effect.OnDetach(this);	
				}
				else{
					effect.OnApply(this);
				}
			}
		}
		
		foreach(DictionaryEntry effectEntry in statusEffects){
			ArrayList effects = (ArrayList) effectEntry.Value;
			//remove finished status effects
			for(int i = effects.Count - 1; i > -1; i--){
				IActorStatusEffect effect = (IActorStatusEffect) effects[i];
				if(effect.IsDead()){
					Destroy(effect);
					effects.RemoveAt(i);
					if(effects.Count == 0){
				   		keysToRemove.Add((string)effectEntry.Key);
					}
				}
			}	
		}
		 
		foreach(string key in keysToRemove){
			statusEffects.Remove(key);	
		}
		keysToRemove.Clear();
	}
	
	private void ResetModifiers(){
		for(int i = 0; i < 4; i++){
			if(i % 2 == 0){
				MaxHPModifiers[i] = 0;
				HPModifiers[i] = 0;
				MoveSpeedModifiers[i] = 0;
				DamageModifiers[i] = 0;
				AttackSpeedModifiers[i] = 0;
			}
			else{
				MaxHPModifiers[i] = 1;
				HPModifiers[i] = 1;
				MoveSpeedModifiers[i] = 1;
				DamageModifiers[i] = 1;
				AttackSpeedModifiers[i] = 1;
			}
		}
		motionModifier1m = 1;
		motionModifier2p = Vector3.zero;
	}
	
	public Hashtable GetStatusEffects(){
		return statusEffects;
	}
	
	public GameObject GetActor(){
		return gameObject;	
	}
	
	public void AttachStatusEffect(IActorStatusEffect effect){
		if(!statusEffects.ContainsKey((string)effect.GetName())){
//			Debug.Log ("no Key");
			statusEffects.Add((string)effect.GetName(), new ArrayList());
		}
		if(statusEffects[(string)effect.GetName()] == null){
			statusEffects[(string)effect.GetName()] = new ArrayList();
		}
		((ArrayList)statusEffects[(string)effect.GetName()]).Add(effect);
//		Debug.Log ("adding effect: " + ((ArrayList)statusEffects[(string)effect.GetName()]).Count);
		effect.OnAttach(this);
//		Debug.Log ("Attached fx, count: " + statusEffects.Count);
	}
	
	public void AttachStatusEffects(params IActorStatusEffect[] effects){
		for(int i = 0; i < effects.Length; i++){
			AttachStatusEffect(effects[i]);	
		}
	}
	
	public float GetModifiedStatusf(StatusType type){
		switch(type){
		case StatusType.MAXHP:
			return ((baseMaxHP + MaxHPModifiers[0]) * MaxHPModifiers[1] + MaxHPModifiers[2]) * MaxHPModifiers[3];
			break;
		case StatusType.HP:
			return ((baseHP + HPModifiers[0]) * HPModifiers[1] + HPModifiers[2]) * HPModifiers[3];
			break;
		case StatusType.DAMAGE:
			return ((baseDamage + DamageModifiers[0]) * DamageModifiers[1] + DamageModifiers[2]) * DamageModifiers[3];
			break;
		case StatusType.MOVESPEED:
			return ((baseMoveSpeed + MoveSpeedModifiers[0]) * MoveSpeedModifiers[1] + MoveSpeedModifiers[2]) * MoveSpeedModifiers[3];
			break;
		case StatusType.ATTACKSPEED:
			return ((baseAttackSpeed + AttackSpeedModifiers[0]) * AttackSpeedModifiers[1] + AttackSpeedModifiers[2]) * AttackSpeedModifiers[3];
			break;
		}
		return 0f;
	}
	
	public float[] GetModifiers(StatusType type){
		switch(type){
		case StatusType.MAXHP:
			return MaxHPModifiers;
			break;
		case StatusType.HP:
			return HPModifiers;
			break;
		case StatusType.DAMAGE:
			return DamageModifiers;
			break;
		case StatusType.MOVESPEED:
			return MoveSpeedModifiers;
			break;
		case StatusType.ATTACKSPEED:
			return AttackSpeedModifiers;
			break;
		}
		return null;
	}
	
	public ActorType GetActorType(){
		return actorType;
	}

	public void OnGUI(){
		if(showInGUI){
			GUILayout.Label("Total HP: " + GetModifiedStatusf(StatusType.MAXHP) + " (" + baseMaxHP + ") ");
			GUILayout.Label("HP: " + GetModifiedStatusf(StatusType.HP) + " (" + baseHP + ") ");
			GUILayout.Label("Movement Speed: " + GetModifiedStatusf(StatusType.MOVESPEED) + " (" + baseMoveSpeed + ") ");
			GUILayout.Label("Attack Speed: " + GetModifiedStatusf(StatusType.ATTACKSPEED) + " (" + baseAttackSpeed + ") ");
		}
	}
}
