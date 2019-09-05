using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class StatusComponent : MonoBehaviour
{

    [SerializeField] private float m_MaxHealth = 100.0f;
    [SerializeField] private bool m_CanUseIFrames = false;
    [SerializeField] private float m_IFrameTime = 0.2f;
    [SerializeField] private float m_HitFlashDuration = 0.2f;
    [SerializeField] private UnityEvent Die;
    [SerializeField] private UnityEvent OnTakeDamage;  // Useful/used to update things like UI without Update method
    [SerializeField] private UnityEvent OnReceiveHeal; // Useful/used to update things like UI without Update method
    [SerializeField] private UnityEvent OnSetMaxHealth;// Useful/used to update things like UI without Update method
    [HideInInspector] public bool m_IsInvincible = false;  // used for iFrames. Mobs won't use it ever
    private float m_CurrentHealth;
    private delegate void m_HealthHandler();

    public float GetCurrentHealth() { return m_CurrentHealth; }
    public float GetMaxHealth() { return m_MaxHealth; }
    public void SetMaxHealth(float newhealth) { m_MaxHealth = newhealth; OnSetMaxHealth.Invoke(); }
    public void TakeDamage(float amount)
    {
        if (!m_IsInvincible) // if the target can take damage
        {
            m_CurrentHealth -= amount; // reduce your health
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0.0f, m_MaxHealth);
            if (m_CurrentHealth <= 0.0f) Die.Invoke(); //if the actor is dead, call death event
            OnTakeDamage.Invoke(); // call take damage event

            StartCoroutine(FlashSprite(m_HitFlashDuration)); // flash the sprite material if it can
            if (m_CanUseIFrames) // if the actor can use i frames
            {
                m_IsInvincible = true; // make it invincible for the iframe duration
                StartCoroutine(StartIFrame(m_IFrameTime));
            }
        }
    }

    private IEnumerator StartIFrame(float time)
    {
        yield return new WaitForSeconds(time);
        m_IsInvincible = false;
    }

    private IEnumerator FlashSprite(float duration)
    {
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        if (sprite) {
            sprite.material.SetInt("_TookDamage", 1);
            sprite.material.SetFloat("_CurrTime", Time.timeSinceLevelLoad);
        }
        yield return new WaitForSeconds(duration);
        if (sprite)
            sprite.material.SetInt("_TookDamage", 0);
    }

    public void Heal(float amount) {
        m_CurrentHealth += amount;
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0.0f, m_MaxHealth);
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
