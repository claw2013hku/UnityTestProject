using UnityEngine;
using System.Collections;

//Applys effects to status
//Fires events when attached effect, destroyed effect
[RequireComponent (typeof(ActorStatusComponent))]
public class ActorStatus : MonoBehaviour {
	public bool showInGUI;
	
	private ActorStatusComponent statusComp;
	
	//private ActorStatusComponent tempStatusComp = new ActorStatusComponent();
	private Hashtable statusEffects = new Hashtable();
	private ArrayList keysToRemove = new ArrayList();
	
	void Start(){
		statusComp = GetComponent<ActorStatusComponent>();
	}
	
	void Update () {
		//cache the actor status
		CacheStatus();
		
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
		
		//apply the actor status
		ApplyStatus();
	}
	
	void CacheStatus(){
		//tempStatusComp.Assign(statusComp);
		for(int i = 0; i < 4; i++){
			if(i % 2 == 0){
				statusComp.MaxHPModifiers[i] = 0;
				statusComp.HPModifiers[i] = 0;
				statusComp.MoveSpeedModifiers[i] = 0;
				statusComp.DamageModifiers[i] = 0;
				statusComp.AttackSpeedModifiers[i] = 0;
			}
			else{
				statusComp.MaxHPModifiers[i] = 1;
				statusComp.HPModifiers[i] = 1;
				statusComp.MoveSpeedModifiers[i] = 1;
				statusComp.DamageModifiers[i] = 1;
				statusComp.AttackSpeedModifiers[i] = 1;
			}
		}
		statusComp.SetMotionModifier(1, Vector3.zero);
	}
	
	void ApplyStatus(){
		statusComp.MaxHP = GetModifiedStatusf(statusComp, ActorStatusComponent.StatusType.MAXHP);
		statusComp.HP = GetModifiedStatusf(statusComp, ActorStatusComponent.StatusType.HP);
		statusComp.Damage = GetModifiedStatusf(statusComp, ActorStatusComponent.StatusType.DAMAGE);
		statusComp.MoveSpeed = GetModifiedStatusf(statusComp, ActorStatusComponent.StatusType.MOVESPEED);
		statusComp.AttackSpeed = GetModifiedStatusf(statusComp, ActorStatusComponent.StatusType.ATTACKSPEED);
		//statusComp.Assign(tempStatusComp);
	}
	
	private float GetModifiedStatusf(ActorStatusComponent comp, ActorStatusComponent.StatusType type){
		switch(type){
		case ActorStatusComponent.StatusType.MAXHP:
			return ((comp.BaseMaxHP + comp.MaxHPModifiers[0]) * comp.MaxHPModifiers[1] + comp.MaxHPModifiers[2]) * comp.MaxHPModifiers[3];
		case ActorStatusComponent.StatusType.HP:
			return ((comp.BaseHP + comp.HPModifiers[0]) * comp.HPModifiers[1] + comp.HPModifiers[2]) * comp.HPModifiers[3];
		case ActorStatusComponent.StatusType.DAMAGE:
			return ((comp.BaseDamage + comp.DamageModifiers[0]) * comp.DamageModifiers[1] + comp.DamageModifiers[2]) * comp.DamageModifiers[3];
		case ActorStatusComponent.StatusType.MOVESPEED:
			return ((comp.BaseMoveSpeed + comp.MoveSpeedModifiers[0]) * comp.MoveSpeedModifiers[1] + comp.MoveSpeedModifiers[2]) * comp.MoveSpeedModifiers[3];
		case ActorStatusComponent.StatusType.ATTACKSPEED:
			return ((comp.BaseAttackSpeed + comp.AttackSpeedModifiers[0]) * comp.AttackSpeedModifiers[1] + comp.AttackSpeedModifiers[2]) * comp.AttackSpeedModifiers[3];
		}
		return 0f;
	}
	
	public ActorStatusComponent ReadStatus{
		get{
			//use the resultant one last frame or the current temporary one
			return statusComp;	
		}
	}
	
	public ActorStatusComponent WriteStatus(){
		//use the resultant one last frame or the current temporary one
		return statusComp;
	}
	
	public Hashtable GetStatusEffects(){
		return statusEffects;
	}
	
	public void AttachStatusEffect(IActorStatusEffect effect){
		if(!statusEffects.ContainsKey((string)effect.GetName())){
			statusEffects.Add((string)effect.GetName(), new ArrayList());
		}
		if(statusEffects[(string)effect.GetName()] == null){
			statusEffects[(string)effect.GetName()] = new ArrayList();
		}
		((ArrayList)statusEffects[(string)effect.GetName()]).Add(effect);
		effect.OnAttach(this);
	}
	
	public void AttachStatusEffects(params IActorStatusEffect[] effects){
		for(int i = 0; i < effects.Length; i++){
			AttachStatusEffect(effects[i]);	
		}
	}
	
	public void OnGUI(){
		if(showInGUI){
			GUILayout.Label("Total HP: " + statusComp.MaxHP + " (" + statusComp.BaseMaxHP + ") ");
			GUILayout.Label("HP: " + statusComp.HP + " (" + statusComp.BaseHP + ") ");
			GUILayout.Label("Movement Speed: " + statusComp.MoveSpeed + " (" + statusComp.BaseMoveSpeed + ") ");
			GUILayout.Label("Attack Speed: " + statusComp.AttackSpeed + " (" + statusComp.BaseAttackSpeed + ") ");
		}
	}
}
