using System;
using UnityEngine;

[Serializable]
public class OnStartData
{
    public FollowRuleData m_FollowRuleData;
    public OnStartBehavior m_StartBehavior;
}

[Serializable]
public class FollowRuleData
{
    public FollowWho m_FollowRule = FollowWho.Player;
    public Vector3 m_FollowOffset;
    public float m_FollowSmoothDamp = 0.1f;
    public bool m_UseAimedTarget = false;
}

[System.Serializable]
public abstract class OnStartBehavior : ScriptableObject
{
    public abstract void OnStartEvent(GameObject src, OnStartData data);
}

[CreateAssetMenu(menuName = "SpellBehavior/Set Follow Rule")]
public class SetFollowRule : OnStartBehavior
{
    public override void OnStartEvent(GameObject src, OnStartData data)
    {
        src.GetComponent<SpellPrefabManager>().SetFollowerOwner(data.m_FollowRuleData.m_FollowRule);
        src.GetComponent<SpellPrefabManager>().m_FollowOffset = data.m_FollowRuleData.m_FollowOffset;
        src.GetComponent<SpellPrefabManager>().m_FollowSmoothDamp = data.m_FollowRuleData.m_FollowSmoothDamp;
        src.GetComponent<SpellPrefabManager>().SetUseAimedTarget(data.m_FollowRuleData.m_UseAimedTarget);
    }
}