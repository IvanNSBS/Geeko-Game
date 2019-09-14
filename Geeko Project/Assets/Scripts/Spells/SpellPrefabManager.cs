using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// This script is used to manage the Main Spell Prefab
// collision and other useful spell behavior

public class SpellPrefabManager : MonoBehaviour
{
    [SerializeField] private GameplayStatics.CollisionEvent m_OnCollideEnter;   // called when a collision happens and the
                                                                                // collider is not a trigger
    [SerializeField] private GameplayStatics.TriggerEvent m_OnTriggerTick;      // Called on a collision stay
    [SerializeField] private GameplayStatics.CollisionEvent m_OnCollisionTick;  // Called on a trigger stay
    [SerializeField] private GameplayStatics.TriggerEvent m_OnTriggerEnter;     // called when a collision happens and collider
                                                                                // is a trigger
    [SerializeField] private GameplayStatics.SpellEvent m_OnUpdate;
    [SerializeField] private UnityEvent m_OnDestruction;
    // private List<GameObject> m_CollidingActors = new List<GameObject>();
    private GameObject m_Owner;
    [HideInInspector] public float m_TimeToLive;

    public void SetOwner(GameObject n_owner) { m_Owner = n_owner; }
    public GameObject GetOwner() { return m_Owner; }

    public void AddCollideEnter( UnityAction<Collision2D, GameObject> action) {
        if (m_OnCollideEnter == null)
            m_OnCollideEnter = new GameplayStatics.CollisionEvent();
        m_OnCollideEnter.AddListener(action);
    }
    public void AddTriggerEnter( UnityAction<Collider2D, GameObject> action) {
        if (m_OnTriggerEnter == null)
            m_OnTriggerEnter = new GameplayStatics.TriggerEvent();
        m_OnTriggerEnter.AddListener(action);
    }
    public void AddCollideTick( UnityAction<Collision2D, GameObject> action) {
        if (m_OnCollisionTick == null)
            m_OnCollisionTick = new GameplayStatics.CollisionEvent();
        m_OnCollisionTick.AddListener(action);
    }
    public void AddTriggerTick( UnityAction<Collider2D, GameObject> action) {
        if (m_OnTriggerTick == null)
            m_OnTriggerTick = new GameplayStatics.TriggerEvent();
        m_OnTriggerTick.AddListener(action);
    }
    public void AddOnUpdate( UnityAction<GameObject> action )
    {
        if (m_OnUpdate == null)
            m_OnUpdate = new GameplayStatics.SpellEvent();
        m_OnUpdate.AddListener(action);
    }
    public void AddOnDestruction(UnityAction action)
    {
        if (m_OnDestruction == null)
            m_OnDestruction = new UnityEvent();
        m_OnDestruction.AddListener(action);
    }

    public void OnDestroy() { if (m_OnDestruction != null) m_OnDestruction.Invoke(); }

    public void Update()
    {
        if (m_OnUpdate != null)
            m_OnUpdate.Invoke(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(m_OnTriggerEnter != null)
            m_OnTriggerEnter.Invoke(other, this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(m_OnCollideEnter != null)
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
