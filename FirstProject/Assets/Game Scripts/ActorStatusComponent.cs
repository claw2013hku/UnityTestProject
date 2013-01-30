using UnityEngine;
using System.Collections;

//Stores base stats, modifiers, modified stats
//Fires (resulting) base stats changed event, modifiers changed event, modified stats changed event
public class ActorStatusComponent : MonoBehaviour {
	//Base stats
	public enum ActorType {Test};
	private ActorType type;
	public ActorType Type {set {if (value != type){HasChangedType(type, value); type = value;}} get {return type;}}
	public enum StatusType {TYPE, MAXHP, HP, MOVESPEED, DAMAGE, ATTACKSPEED}
	public float baseMaxHP = 1f; 
	public float BaseMaxHP {set {if(value != baseMaxHP){if(HasChangedStatus != null) HasChangedStatus(true, StatusType.MAXHP, baseMaxHP, value); baseMaxHP = value;}} get {return baseMaxHP;}}
	public float baseHP = 1f;
	public float BaseHP {set {if(value != baseHP){if(HasChangedStatus != null) HasChangedStatus(true, StatusType.HP, baseHP, value); baseHP = value;}} get {return baseHP;}}
	public float baseMoveSpeed = 1f;
	public float BaseMoveSpeed {set {if(value != baseMoveSpeed){if(HasChangedStatus != null) HasChangedStatus(true, StatusType.MOVESPEED, baseMoveSpeed, value); baseMoveSpeed = value;}} get {return baseMoveSpeed;}}
	public float baseDamage = 1f;
	public float BaseDamage {set {if(value != baseDamage){if(HasChangedStatus != null) HasChangedStatus(true, StatusType.DAMAGE, baseDamage, value); baseDamage = value;}} get {return baseDamage;}}
	public float baseAttackSpeed = 1f;
	public float BaseAttackSpeed {set {if(value != baseAttackSpeed){if(HasChangedStatus != null) HasChangedStatus(true, StatusType.ATTACKSPEED, baseAttackSpeed, value); baseAttackSpeed = value;}} get {return baseAttackSpeed;}}
	
	//Modifiers
	private float[] maxHPModifiers = {0, 1, 0, 1};
	public float[] MaxHPModifiers {set {SetModifiers(StatusType.MAXHP, maxHPModifiers, value);} get{return maxHPModifiers;}}
	private float[] hpModifiers = {0, 1, 0, 1};
	public float[] HPModifiers {set {SetModifiers(StatusType.HP, hpModifiers, value);} get{return hpModifiers;}}
	private float[] moveSpeedModifiers = {0, 1, 0, 1};
	public float[] MoveSpeedModifiers {set {SetModifiers(StatusType.MOVESPEED, moveSpeedModifiers, value);} get{return moveSpeedModifiers;}}
	private float[] damageModifiers = {0, 1, 0, 1};
	public float[] DamageModifiers {set {SetModifiers(StatusType.DAMAGE, damageModifiers, value);} get{return damageModifiers;}}
	private float[] attackSpeedModifiers = {0, 1, 0, 1};
	public float[] AttackSpeedModifiers {set {SetModifiers(StatusType.ATTACKSPEED, attackSpeedModifiers, value);} get{return attackSpeedModifiers;}}
	//motion = modifiedMoveSpeed? * modifier1 + modifier2
	private float motionModifier1m = 1f;
	public float MotionModifier1m {get {return motionModifier1m;}}
	private Vector3 motionModifier2p = Vector3.zero;
	public Vector3 MotionModifier2p {get {return motionModifier2p;}}
	
	//Current stats
	private float maxHP = 1f;
	public float MaxHP {set {if(value != MaxHP){if(HasChangedStatus != null) HasChangedStatus(false, StatusType.MAXHP, maxHP, value); maxHP = value;}} get {return maxHP;}}
	private float hp = 1f;
	public float HP {set {if(value != hp){if(HasChangedStatus != null) HasChangedStatus(false, StatusType.HP, hp, value); hp = value;}} get {return hp;}}
	private float moveSpeed = 1f;
	public float MoveSpeed {set {if(value != moveSpeed){if(HasChangedStatus != null) HasChangedStatus(false, StatusType.MOVESPEED, moveSpeed, value); moveSpeed = value;}} get {return moveSpeed;}}
	private float damage = 1f;
	public float Damage {set {if(value != damage){if(HasChangedStatus != null) HasChangedStatus(false, StatusType.DAMAGE, damage, value); damage = value;}} get {return damage;}}
	private float attackSpeed = 1f;		
	public float AttackSpeed {set {if(value != attackSpeed){if(HasChangedStatus != null) HasChangedStatus(false, StatusType.ATTACKSPEED, attackSpeed, value); attackSpeed = value;}} get {return attackSpeed;}}
	
	//delegates
	public delegate void ActorTypeChange(ActorType oldVal, ActorType newVal);
	public event ActorTypeChange HasChangedType;
	public delegate void ActorStatusChange(bool isBase, StatusType type, float oldVal, float newVal);
	public event ActorStatusChange HasChangedStatus;
	public delegate void ActorStatusModifierChange(StatusType type, float[] oldVal, float[] newVal);
	public event ActorStatusModifierChange HasChangedModifier;
	public delegate void ActorStatusMotionModifierChange(float oldVal1, Vector3 oldVal2, float newVal1, Vector3 newVal2);
	public event ActorStatusMotionModifierChange HasChangedMotionModifier;
	
	//Public methods
	public void SetMotionModifier(float first, Vector3 second){
		bool changedValue = false;
		if(motionModifier1m != first || motionModifier2p != second){
			if(HasChangedMotionModifier != null)
				HasChangedMotionModifier(motionModifier1m, motionModifier2p, first, second);	
		}
		motionModifier1m = first;
		motionModifier2p = second;
	}
	
	public void Assign(ActorStatusComponent values){
		Type = values.Type;
		BaseMaxHP = values.BaseMaxHP;
		BaseHP = values.BaseHP;
		BaseMoveSpeed = values.BaseMoveSpeed;
		BaseAttackSpeed = values.BaseAttackSpeed;
		BaseDamage = values.BaseDamage;
		
		MaxHPModifiers = values.MaxHPModifiers;
		HPModifiers = values.HPModifiers;
		MoveSpeedModifiers = values.MoveSpeedModifiers;
		AttackSpeedModifiers = values.AttackSpeedModifiers;
		DamageModifiers = values.DamageModifiers;
		
		MaxHP = values.MaxHP;
		HP = values.HP;
		MoveSpeed = values.MoveSpeed;
		AttackSpeed = values.AttackSpeed;
		Damage = values.Damage;
	}
	
	//Helper methods
	private void SetModifiers(StatusType type, float[] oldVal, float[] newVal){
		if(oldVal.Length != newVal.Length){
			Debug.LogError("length mismatch when assigning modifier");
		}
		
		bool changedValue = false;
		for(int i = 0; i < oldVal.Length; i++){
			if(oldVal[i] != newVal[i]){
				changedValue = true;
			}
			oldVal[i] = newVal[i];
		}
		if(changedValue){
			if(HasChangedModifier != null)
				HasChangedModifier(type, oldVal, newVal);	
		}
	}
}
