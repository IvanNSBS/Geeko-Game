using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

[CreateAssetMenu(menuName = "SpellBehavior/On Tick/ApplyHealing")]
public class ApplyHealing : OnTickBehavior
{
    public override void OnTickEvent(GameObject src, OnTickData data)
    {
        src.GetComponent<SpellPrefabManager>().SetFollowerOwner(FollowWho.Player);
        src.GetComponent<SpellPrefabManager>().m_FollowOffset = new Vector3(0, 0, -5);
        var status = src.GetComponent<SpellPrefabManager>().GetOwner().GetComponent<StatusComponent>();
        status.Heal(data.m_SCData.m_Value);

    }
}
