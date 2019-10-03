using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class BehaviorData
{
    public float m_Strength = 10;
    public SpellBehavior behavior;
}

[Serializable]
public class OnCollisionData
{
    public float m_Damage = 10.0f;
    public GameplayStatics.DamageType m_DamageType = GameplayStatics.DamageType.Normal;
    public Spell m_SpellToCast;
    public OnHitBehavior m_HitBehavior;
}

[System.Serializable]
public abstract class SpellBehavior : ScriptableObject
{
    public abstract void Invoke(GameObject target, GameObject source, BehaviorData data);
}

[CreateAssetMenu (menuName = "SpellBehavior/ModifySpeed")]
    public class ModifySpeed : SpellBehavior
    {
    public override void Invoke(GameObject target, GameObject source, BehaviorData data)
    {
        target.GetComponent<EffectManagerComponent>().AddToSpeedMult(data.m_Strength);
    }
}





[System.Serializable]
public abstract class OnHitBehavior : ScriptableObject
{
    public abstract void OnTriggerHit(Collider2D collision, GameObject src, OnCollisionData data);
    public abstract void OnCollisionHit(Collision2D collision, GameObject src, OnCollisionData data);
}

[CreateAssetMenu (menuName = "SpellBehavior/Apply Damage")]
public class ApplyDamage : OnHitBehavior
{
    public override void OnCollisionHit(Collision2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();

        SpellUtilities.DamageOnCollide(collision.gameObject, s_manager, data.m_Damage, SpellUtilities.invalid2, data.m_DamageType);
        if (target_obj != s_manager.GetOwner() && GameplayStatics.ObjHasTag(target_obj, SpellUtilities.entities))
            GameObject.Destroy(src);
    }

    public override void OnTriggerHit(Collider2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();

        SpellUtilities.DamageOnCollide(collision.gameObject, s_manager, data.m_Damage, SpellUtilities.invalid2, data.m_DamageType);
        if (target_obj != s_manager.GetOwner() && GameplayStatics.ObjHasTag(target_obj, SpellUtilities.entities))
            GameObject.Destroy(src);
    }
}