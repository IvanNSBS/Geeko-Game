using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class BehaviorData
{
    public float m_Strength = 10;
}

[System.Serializable]
public abstract class SpellBehavior : ScriptableObject
{
    public abstract void Invoke(GameObject target, GameObject source);
}

[System.Serializable]
public abstract class OnHitBehavior : SpellBehavior
{
    public abstract void OnHit(Collider2D target, GameObject source);
}
[System.Serializable]
public class ApplyDamage : OnHitBehavior
{
    public override void Invoke(GameObject target, GameObject source)
    {
        throw new System.NotImplementedException();
    }

    public override void OnHit(Collider2D target, GameObject source)
    {
        GameplayStatics.ApplyDamage(target.gameObject, 1000.0f);
    }
}

