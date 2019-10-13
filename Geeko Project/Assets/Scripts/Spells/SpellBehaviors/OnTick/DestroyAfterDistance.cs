using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

[CreateAssetMenu(menuName = "SpellBehavior/On Tick/Destroy After Distance")]
public class DestroyAfterDistance : OnTickBehavior
{
    public override void OnTickEvent(GameObject src, OnTickData data)
    {
        GameObject owner = src.GetComponent<SpellPrefabManager>().GetOwner();
        if (!owner)
            return;

        if ((src.transform.position - owner.transform.position).magnitude > data.m_TravelData.m_TravelDistance)
                Destroy(src);
    }
}
