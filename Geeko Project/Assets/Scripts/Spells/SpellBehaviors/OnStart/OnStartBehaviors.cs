using System;
using UnityEngine;

[Serializable]
public class OnStartData
{
    public OnStartBehavior m_StartBehavior;
    public GeneralData m_General;
    public StatusComponentData m_StatusComponentData;
    public FollowRuleData m_FollowRuleData;
}

[Serializable]
public class FollowRuleData
{
    public FollowWho m_FollowRule = FollowWho.Player;
    public Vector3 m_FollowOffset;
    public float m_FollowSmoothDamp = 0.1f;
    public bool m_CenterAroundCaster = false;
    public bool m_UseAimedTarget = false;
    public bool m_MoveImmediately = false;
}

[Serializable]
public class StatusComponentData
{
    public float m_Health = 20.0f;
    public float m_IFrameTime = 0.0f;
    public GameplayStatics.DamageType m_OverrideDmgType = GameplayStatics.DamageType.Null;
}

[Serializable]
public class GeneralData 
{
    public float m_Value;
}


[System.Serializable]
public abstract class OnStartBehavior : ScriptableObject
{
    public abstract void OnStartEvent(GameObject src, OnStartData data);
}