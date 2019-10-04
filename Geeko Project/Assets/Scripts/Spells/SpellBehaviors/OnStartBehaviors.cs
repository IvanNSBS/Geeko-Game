using System;
using UnityEngine;

[Serializable]
public class OnStartData
{
    public OnStartBehavior m_StartBehavior;
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

[System.Serializable]
public abstract class OnStartBehavior : ScriptableObject
{
    public abstract void OnStartEvent(GameObject src, OnStartData data);
}

[CreateAssetMenu(menuName = "SpellBehavior/On Start/Set Follow Rule")]
public class SetFollowRule : OnStartBehavior
{
    public override void OnStartEvent(GameObject src, OnStartData data)
    {
        var manager = src.GetComponent<SpellPrefabManager>();
        var owner = manager.GetOwner();
        if (data.m_FollowRuleData.m_CenterAroundCaster)
        {
            var circle = owner.GetComponent<Collider2D>();
            if(data.m_FollowRuleData.m_MoveImmediately)
                src.transform.position = data.m_FollowRuleData.m_FollowOffset + circle.bounds.center - owner.transform.position;
            manager.m_FollowOffset = data.m_FollowRuleData.m_FollowOffset + circle.bounds.center - owner.transform.position;
        }
        else
        {
            var circle = owner.GetComponent<Collider2D>();
            if (data.m_FollowRuleData.m_MoveImmediately)
                src.transform.position = data.m_FollowRuleData.m_FollowOffset + circle.bounds.center - owner.transform.position;
            manager.m_FollowOffset = data.m_FollowRuleData.m_FollowOffset;
        }

        manager.SetFollowerOwner(data.m_FollowRuleData.m_FollowRule);
        manager.m_FollowSmoothDamp = data.m_FollowRuleData.m_FollowSmoothDamp;
        manager.SetUseAimedTarget(data.m_FollowRuleData.m_UseAimedTarget);
    }
}