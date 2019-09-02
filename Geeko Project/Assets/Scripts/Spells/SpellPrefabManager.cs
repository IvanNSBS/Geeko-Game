﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// This script is used to manage the Main Spell Prefab
// collision and other useful spell behavior

// TODO: Move those collision to a GameplayStatics class
[System.Serializable]
public class BasicCollisionEvent : UnityEvent<GameObject, GameObject> { }
public class CollisionEvent : UnityEvent<Collision2D, GameObject> { }
public class TriggerEvent: UnityEvent<Collider2D, GameObject> { }
public class SpellEvent : UnityEvent<GameObject> { }
public class SpellPrefabManager : MonoBehaviour
{
    [SerializeField] private BasicCollisionEvent m_OnCollideEnter;   // called when a collision happens and the
                                                                        // collider is not a trigger
    [SerializeField] private BasicCollisionEvent m_OnTriggerTick;    // Called on a collision stay
    [SerializeField] private BasicCollisionEvent m_OnCollisionTick;  // Called on a trigger stay
    [SerializeField] private BasicCollisionEvent m_OnTriggerEnter;   // called when a collision happens and collider
                                                                // is a trigger
    [SerializeField] private SpellEvent m_OnUpdate;
    // private List<GameObject> m_CollidingActors = new List<GameObject>();
    private GameObject m_Owner;
    [HideInInspector] public float m_TimeToLive;

    public void SetOwner(GameObject n_owner) { m_Owner = n_owner; }
    public GameObject GetOwner() { return m_Owner; }

    public void AddCollideEnter( UnityAction<GameObject, GameObject> action) {
        if (m_OnCollideEnter == null)
            m_OnCollideEnter = new BasicCollisionEvent();
        m_OnCollideEnter.AddListener(action);
    }
    public void AddTriggerEnter( UnityAction<GameObject, GameObject> action) {
        if (m_OnTriggerEnter == null)
            m_OnTriggerEnter = new BasicCollisionEvent();
        m_OnTriggerEnter.AddListener(action);
    }
    public void AddCollideTick( UnityAction<GameObject, GameObject> action) {
        if (m_OnCollisionTick == null)
            m_OnCollisionTick = new BasicCollisionEvent();
        m_OnCollisionTick.AddListener(action);
    }
    public void AddTriggerTick( UnityAction<GameObject, GameObject> action) {
        if (m_OnTriggerTick == null)
            m_OnTriggerTick = new BasicCollisionEvent();
        m_OnTriggerTick.AddListener(action);
    }
    public void AddOnUpdate( UnityAction<GameObject> action )
    {
        if (m_OnUpdate == null)
            m_OnUpdate = new SpellEvent();
        m_OnUpdate.AddListener(action);
    }

    public void Update()
    {
        if (m_OnUpdate != null)
            m_OnUpdate.Invoke(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(m_OnTriggerEnter != null)
            m_OnTriggerEnter.Invoke(other.gameObject, this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(m_OnCollideEnter != null)
            m_OnCollideEnter.Invoke(collision.gameObject, this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (m_OnTriggerTick != null)
            m_OnTriggerTick.Invoke(collision.gameObject, this.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (m_OnCollisionTick != null)
            m_OnCollisionTick.Invoke(collision.gameObject, this.gameObject);
    }
}