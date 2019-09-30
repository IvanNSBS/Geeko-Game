using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class StatusComponent : MonoBehaviour
{
    [SerializeField] private GameObject m_DMGPopup;
    [SerializeField] private float m_MaxHealth = 100.0f;
    [SerializeField] public bool m_CanUseIFrames = false;
    [SerializeField] public float m_IFrameTime = 0.2f;
    [SerializeField] private float m_HitFlashDuration = 0.2f;
    [SerializeField] private UnityEvent m_OnDeath;
    [SerializeField] private GameplayStatics.DamageEvent m_OnTakeDamage;  // Useful/used to update things like UI without Update method
    [SerializeField] private GameplayStatics.DamageEvent m_OnReceiveHeal; // Useful/used to update things like UI without Update method
    [SerializeField] private UnityEvent m_OnSetMaxHealth;// Useful/used to update things like UI without Update method
    [HideInInspector] public bool m_IsInvincible = false;  // used for iFrames. Mobs won't use it ever
    private float m_CurrentHealth;
    public GameplayStatics.DamageType m_DamagePopupOverride = GameplayStatics.DamageType.Null;

    public float GetCurrentHealth() { return m_CurrentHealth; }
    public float GetMaxHealth() { return m_MaxHealth; }
    public void SetMaxHealth(float newhealth) {
        m_MaxHealth = newhealth;
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0.0f, newhealth);
        if (m_OnSetMaxHealth != null)
            m_OnSetMaxHealth.Invoke();
    }

    public void AddOnTakeDamage(UnityAction<float, GameplayStatics.DamageType> action)
    {
        if (m_OnTakeDamage == null)
            m_OnTakeDamage = new GameplayStatics.DamageEvent();
        m_OnTakeDamage.AddListener(action);
    }
    public void AddOnReceiveHeal(UnityAction<float, GameplayStatics.DamageType> action)
    {
        if (m_OnReceiveHeal == null)
            m_OnReceiveHeal = new GameplayStatics.DamageEvent();
        m_OnReceiveHeal.AddListener(action);
    }
    public void AddOnSetMaxHealth(UnityAction action)
    {
        if (m_OnSetMaxHealth == null)
            m_OnSetMaxHealth = new UnityEvent();
        m_OnSetMaxHealth.AddListener(action);
    }

    public void AddOnDeath(UnityAction action)
    {
        if (m_OnDeath == null)
            m_OnDeath = new UnityEvent();
        m_OnDeath.AddListener(action);
    }


    public void TakeDamage(float amount, GameplayStatics.DamageType type = GameplayStatics.DamageType.Normal)
    {
        if (!m_IsInvincible) // if the target can take damage
        {
            m_CurrentHealth -= amount; // reduce your health
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0.0f, m_MaxHealth);
            if (m_CurrentHealth <= 0.0f && m_OnDeath != null) m_OnDeath.Invoke(); //if the actor is dead, call death event
            if (m_OnTakeDamage != null) m_OnTakeDamage.Invoke(amount, type); // call take damage event
            if (m_DamagePopupOverride == GameplayStatics.DamageType.Null)
                GameplayStatics.SpawnDmgPopup(transform.position, amount, type);
            else
                GameplayStatics.SpawnDmgPopup(transform.position, amount, m_DamagePopupOverride);

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

    public void Heal(float amount, GameplayStatics.DamageType type = GameplayStatics.DamageType.Heal) {
        m_CurrentHealth += amount;
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0.0f, m_MaxHealth);
        if(m_OnReceiveHeal != null)
            m_OnReceiveHeal.Invoke(amount, type);
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
