using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// This script is used to manage the Main Spell Prefab
// collision and other useful spell behavior

public enum FollowWho { Null, Player, Target };
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
    private float m_RemainingTickTime = 0.0f;

    [HideInInspector] public float m_TimeToLive;
    [HideInInspector] public float m_TimeAlive = 0.0f;
    [HideInInspector] public GameObject m_Target = null;
    [HideInInspector] public Quaternion m_SpawnRot;
    [HideInInspector] public bool m_Toggle = false;
    [HideInInspector] public Vector3? m_TargetInitialPos;
    [HideInInspector] public float m_TickDelay = 0.0f;

    private FollowWho m_FollowTarget = FollowWho.Null;
    public Vector3 m_FollowOffset = new Vector3(0, 0, 0);
    private bool m_UseAimedTarget = false;
    public void SetFollowerOwner(FollowWho value) { m_FollowTarget = value; }
    public void SetUseAimedTarget(bool value) { m_UseAimedTarget = value; }
    public void ResetTickLock()
    {
        m_RemainingTickTime = m_TickDelay;
    }

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
        if (m_UseAimedTarget)
            m_Target = GetOwner().GetComponent<PlayerController>().target;
        if (m_FollowTarget == FollowWho.Player) {

            Vector3 z = Vector3.zero;
            gameObject.transform.position = Vector3.SmoothDamp(
                gameObject.transform.position, GetOwner().transform.position + m_FollowOffset, ref z, 0.1f);

        }
        if (m_FollowTarget == FollowWho.Target && m_Target)
            gameObject.transform.position = m_Target.transform.position + m_FollowOffset;

        if (m_OnUpdate != null && m_RemainingTickTime == m_TickDelay)
            m_OnUpdate.Invoke(this.gameObject);

        m_TimeAlive += Time.deltaTime;
        m_RemainingTickTime -= Time.deltaTime;

        if (m_TimeToLive != 0.0f && m_TimeAlive > m_TimeToLive)
            Destroy(gameObject);

        if (m_RemainingTickTime <= 0.0f) ResetTickLock();
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
