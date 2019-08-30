using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Items/HealthPotion")]
public class HealthPotion : Item
{
    public float m_HealAmount = 5.0f;

    public override void Initialize() {
        m_ItemManager = m_ItemPrefab.GetComponent<ItemPrefabManager>();
        m_ItemManager.AddTriggerEnter(PickupItem);

        //set item visual stuff;
    }
    public override void PickupItem(GameObject obj, GameObject src)
    {
        if (obj.CompareTag("Player"))
        {
            StatusComponent comp = obj.GetComponent<StatusComponent>();
            comp.Heal(m_HealAmount);

            if (m_OnPickup != null)
                m_OnPickup.Invoke(obj, obj);

            DestroyItem();
        }
    }
}
