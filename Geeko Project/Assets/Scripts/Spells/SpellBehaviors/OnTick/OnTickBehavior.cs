using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class OnTickData
{
    public float m_Delay;
    public OnTickBehavior m_Behavior;
    public StatusTickData m_SCData;
    public TravelDistanceData m_TravelData;
}

[Serializable]
public class StatusTickData
{
    public float m_Value;
    public bool m_RemoveOnModified = false;
}

[Serializable]
public class TravelDistanceData 
{
    public float m_TravelDistance;
    public Spell m_SpellToCast;
}

[System.Serializable]
public abstract class OnTickBehavior : ScriptableObject
{
    public abstract void OnTickEvent(GameObject src, OnTickData data);
}
