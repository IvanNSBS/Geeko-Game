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
        if (obj.CompareTag("Player")) // Check if it was the player that tried to pic the item
        {
            //get player status component
            StatusComponent comp = obj.GetComponent<StatusComponent>();
            comp.Heal(m_HealAmount); // heal the player
            Debug.Log("Current Health = " + comp.GetCurrentHealth());
            if (m_OnPickup != null)
                m_OnPickup.Invoke(obj, obj); // call auxiliary m_OnPickup function if there's one

            DestroyItem(); // destroy the potion, since it was used
        }
    }
}
