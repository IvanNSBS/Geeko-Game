using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SpellBehavior/On Start/Add Status Component")]
public class AddStatusComponent : OnStartBehavior
{
    public override void OnStartEvent(GameObject src, OnStartData data)
    {
        StatusComponent status = src.AddComponent<StatusComponent>() as StatusComponent;
        status.SetMaxHealth(data.m_StatusComponentData.m_Health);
        status.Heal(data.m_StatusComponentData.m_Health, GameplayStatics.DamageType.Null);
        status.m_CanUseIFrames = data.m_StatusComponentData.m_Health > 0;
        status.m_IFrameTime = data.m_StatusComponentData.m_IFrameTime;
        status.m_DamagePopupOverride = data.m_StatusComponentData.m_OverrideDmgType;
        status.AddOnDeath(() => Destroy(src));
    }
}