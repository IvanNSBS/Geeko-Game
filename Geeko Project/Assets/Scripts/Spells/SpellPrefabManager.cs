using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// This script is used to manage the Main Spell Prefab
// collision and other useful spell behavior
public class SpellPrefabManager : MonoBehaviour
{
    [SerializeField] private UnityEvent<GameObject> m_OnCollideEnter;   // called when a collision happens and the
                                                                        // collider is not a trigger
    [SerializeField] private UnityEvent<GameObject> m_OnTriggerTick;    // Called on a collision stay
    [SerializeField] private UnityEvent<GameObject> m_OnCollisionTick;  // Called on a trigger stay
    [SerializeField] private UnityEvent<GameObject> m_OnTriggerEnter;   // called when a collision happens and collider
                                                                        // is a trigger

    // private List<GameObject> m_CollidingActors = new List<GameObject>();
    private Spell m_Owner;
 
    public void SetOwner(Spell n_owner) { m_Owner = n_owner; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player")) || (other.CompareTag("Door")) || (other.CompareTag("Wall")))
        {
            Debug.Log("hit something");
            if(m_OnTriggerEnter != null) m_OnTriggerEnter.Invoke(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Player")) || (collision.gameObject.CompareTag("Door")) || (collision.gameObject.CompareTag("Wall")))
        {
            Debug.Log("collided with hit something");
            if(m_OnCollideEnter != null) m_OnCollideEnter.Invoke(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("Player")) || (collision.gameObject.CompareTag("Door")) || (collision.gameObject.CompareTag("Wall")))
        {
            Debug.Log("collided with hit something");
            if (m_OnTriggerTick != null) m_OnTriggerTick.Invoke(collision.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Staying on Collision...");
        if ((collision.gameObject.CompareTag("Player")) || (collision.gameObject.CompareTag("Door")) || (collision.gameObject.CompareTag("Wall")))
        {
            Debug.Log("collided with hit something");
            if (m_OnCollisionTick != null) m_OnCollisionTick.Invoke(collision.gameObject);
        }
    }
}
