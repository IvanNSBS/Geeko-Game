using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemPrefabManager : MonoBehaviour
{
    [SerializeField] private BasicCollisionEvent m_OnCollideEnter;   // called when a collision happens and the
                                                                // collider is not a trigger
    [SerializeField] private BasicCollisionEvent m_OnTriggerTick;    // Called on a collision stay
    [SerializeField] private BasicCollisionEvent m_OnCollisionTick;  // Called on a trigger stay
    [SerializeField] private BasicCollisionEvent m_OnTriggerEnter;   // called when a collision happens and collider
                                                                // is a trigger
    [SerializeField] private Item m_ItemData;   // Item ScriptableObject data

    public void Start()
    {
        m_ItemData.SetPrefab(this.gameObject); //set the prefab
        m_ItemData.Initialize();
    }

    //basic functions to add Actions
    public void AddCollideEnter(UnityAction<GameObject, GameObject> action)
    {
        if (m_OnCollideEnter == null)
            m_OnCollideEnter = new BasicCollisionEvent();
        m_OnCollideEnter.AddListener(action);
    }
    public void AddTriggerEnter(UnityAction<GameObject, GameObject> action)
    {
        if (m_OnTriggerEnter == null)
            m_OnTriggerEnter = new BasicCollisionEvent();
        m_OnTriggerEnter.AddListener(action);
    }
    public void AddCollideTick(UnityAction<GameObject, GameObject> action)
    {
        if (m_OnCollisionTick == null)
            m_OnCollisionTick = new BasicCollisionEvent();
        m_OnCollisionTick.AddListener(action);
    }
    public void AddTriggerTick(UnityAction<GameObject, GameObject> action)
    {
        if (m_OnTriggerTick == null)
            m_OnTriggerTick = new BasicCollisionEvent();
        m_OnTriggerTick.AddListener(action);
    }

    //Call Unity Action functions when the correct event is triggered
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (m_OnTriggerEnter != null)
            m_OnTriggerEnter.Invoke(other.gameObject, this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_OnCollideEnter != null)
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
