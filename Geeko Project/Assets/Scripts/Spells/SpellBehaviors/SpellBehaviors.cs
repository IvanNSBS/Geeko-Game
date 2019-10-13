using System;
using UnityEngine;

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
public abstract class OnDestructionBehavior : ScriptableObject
{
    public abstract void OnDestructionEvent(OnDestructionData data);
}