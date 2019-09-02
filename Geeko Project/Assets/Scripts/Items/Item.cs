using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : ScriptableObject
{
    public string m_ItemName = "New Item";
    public Sprite m_GameSprite;
    public Animation m_Animation;
    public AudioClip m_PickupSound;
    public BasicCollisionEvent m_OnPickup;

    protected GameObject m_ItemPrefab;
    protected ItemPrefabManager m_ItemManager;
    public abstract void Initialize();
    public abstract void PickupItem(GameObject target, GameObject src);
    public void DestroyItem() { Destroy(this.m_ItemPrefab); }
    public void SetPrefab(GameObject obj) { m_ItemPrefab = obj; }
}