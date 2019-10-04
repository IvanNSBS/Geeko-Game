using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class OnTickData
{
    public float m_stuff;
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
public abstract class OnTickBehavior : ScriptableObject
{
    public abstract void OnTickEvent(GameObject src, OnTickData data);
}

[System.Serializable]
public abstract class OnDestructionBehavior : ScriptableObject
{
    public abstract void OnDestructionEvent(OnDestructionData data);
}