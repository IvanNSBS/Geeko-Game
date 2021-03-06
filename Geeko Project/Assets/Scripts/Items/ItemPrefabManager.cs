﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemPrefabManager : MonoBehaviour
{
    [SerializeField] private GameplayStatics.CollisionEvent m_OnCollideEnter;   // called when a collision happens and the
                                                                // collider is not a trigger
    [SerializeField] private GameplayStatics.TriggerEvent m_OnTriggerTick;    // Called on a collision stay
    [SerializeField] private GameplayStatics.CollisionEvent m_OnCollisionTick;  // Called on a trigger stay
    [SerializeField] private GameplayStatics.TriggerEvent m_OnTriggerEnter;   // called when a collision happens and collider
                                                                // is a trigger
    [SerializeField] private Item m_ItemData;   // Item ScriptableObject data

    public void Start()
    {
        m_ItemData.SetPrefab(this.gameObject); //set the prefab
        m_ItemData.Initialize();
    }

    //basic functions to add Actions
    public void AddCollideEnter(UnityAction<Collision2D, GameObject> action)
    {
        if (m_OnCollideEnter == null)
            m_OnCollideEnter = new GameplayStatics.CollisionEvent();
        m_OnCollideEnter.AddListener(action);
    }
    public void AddTriggerEnter(UnityAction<Collider2D, GameObject> action)
    {
        if (m_OnTriggerEnter == null)
            m_OnTriggerEnter = new GameplayStatics.TriggerEvent();
        m_OnTriggerEnter.AddListener(action);
    }
    public void AddCollideTick(UnityAction<Collision2D, GameObject> action)
    {
        if (m_OnCollisionTick == null)
            m_OnCollisionTick = new GameplayStatics.CollisionEvent();
        m_OnCollisionTick.AddListener(action);
    }
    public void AddTriggerTick(UnityAction<Collider2D, GameObject> action)
    {
        if (m_OnTriggerTick == null)
            m_OnTriggerTick = new GameplayStatics.TriggerEvent();
        m_OnTriggerTick.AddListener(action);
    }

    //Call Unity Action functions when the correct event is triggered
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (m_OnTriggerEnter != null)
            m_OnTriggerEnter.Invoke(other, this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_OnCollideEnter != null)
            m_OnCollideEnter.Invoke(collision, this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (m_OnTriggerTick != null)
            m_OnTriggerTick.Invoke(collision, this.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (m_OnCollisionTick != null)
            m_OnCollisionTick.Invoke(collision, this.gameObject);
    }
}
