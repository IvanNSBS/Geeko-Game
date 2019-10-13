using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SpellBehavior/On Start/AddSpeedMultiplier")]
public class AddSpeed : OnStartBehavior
{
    public override void OnStartEvent(GameObject src, OnStartData data)
    {
        var manager = src.GetComponent<SpellPrefabManager>();
        var owner = manager.GetOwner();

        manager.SetFollowerOwner(FollowWho.Player);
        manager.GetOwner().GetComponent<EffectManagerComponent>().AddToSpeedMult(data.m_General.m_Value);

        manager.AddOnDestruction(() => {
            owner.layer = LayerMask.NameToLayer("Player");
            manager.GetOwner().GetComponent<EffectManagerComponent>().AddToSpeedMult(-data.m_General.m_Value);
        });


    }
}