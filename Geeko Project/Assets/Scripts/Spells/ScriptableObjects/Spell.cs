using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class Spell : ScriptableObject
{
    /*
     * Spell is the parent ScriptableObject that defines the shape of a Spell
     */
    public float m_SpellCooldown = 0.0f;

    // how much time the instantiated prefab shoud live. If the default
    // duration is set to 0, it won't be destroyed with a timer
    public float m_SpellDuration = 0.0f;
    public int m_SpellCharges = 1; 

    public Sprite m_GameSprite;
    public Sprite m_SpellImage; // Spell Icon image
    public Sprite m_BorderImage; // Border of the spell icon
    public AudioClip m_SpellSound; // Sound to play when the spell is cast
    public GameObject m_OnHitEffect; // FX to play if the spell hitted something

    public GameObject m_Prefab; // SpellPrefab

    public List<string> m_ActorsToIgnore = new List<string>(); // List of tags of valid entities
                                                               // that can interact with the spell
    [HideInInspector] public GameObject m_SpellOwner;
    public abstract void Initialize(GameObject obj);
    public abstract void CastSpell(); // What happens when the player casts such 
    public abstract void ApplyDamage(GameObject obj);
    public abstract void OnTick(GameObject obj); // Function to be called on the instantiated prefab Update()
}

[System.Serializable]
public class SpellData
{
    public Spell m_Spell;
    [HideInInspector] public float m_RemainingCD;
    [HideInInspector] public bool m_IsSpellOnCD;
    [HideInInspector] public GameObject m_Owner; // Who is casting the spell
    [HideInInspector] public int m_RemainingCharges;
    public void StartSpellData(GameObject owner)
    {
        m_Owner = owner;
        m_RemainingCD = 0.0f;
        m_IsSpellOnCD = false;
        m_RemainingCharges = m_Spell.m_SpellCharges;
    }


    public void SetOwner(GameObject obj) { m_Owner = obj; }
    public float GetTotalCD() { return m_Spell.m_SpellCooldown; }
    public bool CastSpell()
    {
        //if the spell is not on CD or it has charges
        if (!m_IsSpellOnCD || m_RemainingCharges > 0)
        {
            // TODO: Saving owner on ScriptableObject is bad, since it's static!
            m_Spell.m_SpellOwner = m_Owner; // guarantee the owner is set
            m_Spell.CastSpell();

            m_RemainingCharges--;
            // only update cooldown if it's not already on cooldown
            // i.e when there's no charges left 
            if (!m_IsSpellOnCD)
                m_RemainingCD = m_Spell.m_SpellCooldown;
            m_IsSpellOnCD = true;

            return true;
        }
        return false;
    }

    public void UpdateCD(float delta_time)
    {
        if (m_IsSpellOnCD)
        {
            m_RemainingCD -= delta_time; // update remaining time and clamping it
            m_RemainingCD = Mathf.Clamp(m_RemainingCD, 0.0f, m_Spell.m_SpellCooldown);
            if (m_RemainingCD == 0.0f) {
                // if CD ended, but there's still charges to be recharged,
                // increase charges and restart CD timer
                if (m_RemainingCharges + 1 < m_Spell.m_SpellCharges) {
                    m_RemainingCharges++;
                    m_RemainingCD = m_Spell.m_SpellCooldown;
                }
                // if CD ended but there's no charges left,
                // end cooldown and restore charge
                else if(m_RemainingCharges + 1 == m_Spell.m_SpellCharges) {
                    m_RemainingCharges++;
                    m_IsSpellOnCD = false;
                }
            }
        }
    }
}