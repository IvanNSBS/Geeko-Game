using System;
using UnityEngine;

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
            if (data.m_FollowRuleData.m_MoveImmediately)
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