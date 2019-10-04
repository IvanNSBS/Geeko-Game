using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class OnCollisionData
{
    public OnHitBehavior m_HitBehavior;
    public ApplyDamageData m_ApplyDamage;
    public DestroySelfData m_DestroySelfData;
}

[Serializable]
public class ApplyDamageData
{
    public float m_Damage = 10.0f;
    public GameplayStatics.DamageType m_DamageType = GameplayStatics.DamageType.Normal;
    public List<string> m_ValidHitActors = SpellUtilities.invalid2;
}

[Serializable]
public class DestroySelfData
{
    public List<string> m_ValidHitActors = SpellUtilities.entities;
}

[System.Serializable]
public abstract class OnHitBehavior : ScriptableObject
{
    public abstract void OnTriggerHitEvent(Collider2D collision, GameObject src, OnCollisionData data);
    public abstract void OnCollisionHitEvent(Collision2D collision, GameObject src, OnCollisionData data);
}

[CreateAssetMenu (menuName = "SpellBehavior/On Hit/Apply Damage")]
public class ApplyDamage : OnHitBehavior
{
    public override void OnCollisionHitEvent(Collision2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();

        SpellUtilities.DamageOnCollide(
            collision.gameObject, s_manager, data.m_ApplyDamage.m_Damage, data.m_ApplyDamage.m_ValidHitActors, data.m_ApplyDamage.m_DamageType);
    }

    public override void OnTriggerHitEvent(Collider2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();

        SpellUtilities.DamageOnCollide(
            collision.gameObject, s_manager, data.m_ApplyDamage.m_Damage, data.m_ApplyDamage.m_ValidHitActors, data.m_ApplyDamage.m_DamageType);
    }
}

[CreateAssetMenu(menuName = "SpellBehavior/On Hit/Destroy Self")]
public class DestroySelfEvent : OnHitBehavior
{
    public override void OnCollisionHitEvent(Collision2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();
        if (target_obj != s_manager.GetOwner() && GameplayStatics.ObjHasTag(target_obj, data.m_DestroySelfData.m_ValidHitActors))
            GameObject.Destroy(src);
    }

    public override void OnTriggerHitEvent(Collider2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();
        if (target_obj != s_manager.GetOwner() && GameplayStatics.ObjHasTag(target_obj, data.m_DestroySelfData.m_ValidHitActors))
            GameObject.Destroy(src);
    }
}