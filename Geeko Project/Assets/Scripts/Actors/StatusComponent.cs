using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class StatusComponent : MonoBehaviour
{

    [SerializeField] private float m_MaxHealth = 100.0f;
    [SerializeField] private UnityEvent Die;
    [SerializeField] private UnityEvent OnTakeDamage;
    [SerializeField] private UnityEvent OnReceiveHeal;
    [SerializeField] private UnityEvent OnSetMaxHealth;


    private float m_CurrentHealth;
    private delegate void m_HealthHandler();

    public float GetCurrentHealth() { return m_CurrentHealth; }
    public float GetMaxHealth() { return m_MaxHealth; }
    public void SetMaxHealth(float newhealth) { m_MaxHealth = newhealth; OnSetMaxHealth.Invoke(); }
    public void TakeDamage(float amount)
    {
        m_CurrentHealth -= amount;
        Mathf.Clamp(m_CurrentHealth, 0.0f, m_MaxHealth);
        if (m_CurrentHealth <= 0.0f) Die.Invoke();
        OnTakeDamage.Invoke();
    }
    public void Heal(float amount) {
        m_CurrentHealth += amount;
        Mathf.Clamp(m_CurrentHealth, 0.0f, m_MaxHealth);
        OnReceiveHeal.Invoke();
    }

    //Default function called when the actor is killed.   
    public void Killed() { Destroy(gameObject); }
    // Start is called before the first frame update
    void Start()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
