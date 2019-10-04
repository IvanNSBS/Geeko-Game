using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class OnCollisionData
{
    public float m_Damage = 10.0f;
    public GameplayStatics.DamageType m_DamageType = GameplayStatics.DamageType.Normal;
    public Spell m_SpellToCast;
    public OnHitBehavior m_HitBehavior;
}

[Serializable]
public class OnTickData
{
    public float m_stuff;
}

[Serializable]
public class OnStartData
{
    public FollowWho m_FollowRule = FollowWho.Player;
    public Vector3 m_FollowOffset;
    public float m_FollowSmoothDamp = 0.1f;
    public bool m_UseAimedTarget = false;
    public OnStartBehavior m_StartBehavior;
}

[Serializable]
public class OnDestructionData
{
}

[System.Serializable]
public abstract class OnStayBehavior : ScriptableObject
{
    public abstract void OnTriggerStayEvent(Collider2D collision, GameObject src, OnCollisionData data);
    public abstract void OnCollisionStayEvent(Collision2D collision, GameObject src, OnCollisionData data);
}

[System.Serializable]
public abstract class OnHitBehavior : ScriptableObject
{
    public abstract void OnTriggerHitEvent(Collider2D collision, GameObject src, OnCollisionData data);
    public abstract void OnCollisionHitEvent(Collision2D collision, GameObject src, OnCollisionData data);
}

[System.Serializable]
public abstract class OnTickBehavior : ScriptableObject
{
    public abstract void OnTickEvent(GameObject src, OnTickData data);
}

[System.Serializable]
public abstract class OnStartBehavior : ScriptableObject
{
    public abstract void OnStartEvent(GameObject src, OnStartData data);
}

[System.Serializable]
public abstract class OnDestructionBehavior : ScriptableObject
{
    public abstract void OnDestructionEvent(OnDestructionData data);
}




[CreateAssetMenu(menuName = "SpellBehavior/Set Follow Rule")]
public class SetFollowRule : OnStartBehavior
{
    public override void OnStartEvent(GameObject src, OnStartData data)
    {
        src.GetComponent<SpellPrefabManager>().SetFollowerOwner(data.m_FollowRule);
        src.GetComponent<SpellPrefabManager>().m_FollowOffset = data.m_FollowOffset;
        src.GetComponent<SpellPrefabManager>().m_FollowSmoothDamp = data.m_FollowSmoothDamp;
        src.GetComponent<SpellPrefabManager>().SetUseAimedTarget(data.m_UseAimedTarget);
    }
}



[CreateAssetMenu (menuName = "SpellBehavior/Apply Damage")]
public class ApplyDamage : OnHitBehavior
{
    public override void OnCollisionHitEvent(Collision2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();

        SpellUtilities.DamageOnCollide(collision.gameObject, s_manager, data.m_Damage, SpellUtilities.invalid2, data.m_DamageType);
        if (target_obj != s_manager.GetOwner() && GameplayStatics.ObjHasTag(target_obj, SpellUtilities.entities))
            GameObject.Destroy(src);
    }

    public override void OnTriggerHitEvent(Collider2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();

        SpellUtilities.DamageOnCollide(collision.gameObject, s_manager, data.m_Damage, SpellUtilities.invalid2, data.m_DamageType);
        if (target_obj != s_manager.GetOwner() && GameplayStatics.ObjHasTag(target_obj, SpellUtilities.entities))
            GameObject.Destroy(src);
    }
}