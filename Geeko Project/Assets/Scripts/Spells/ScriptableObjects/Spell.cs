using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SpellCastType { FireAndForget, Concentration }
[System.Serializable]
public abstract class Spell : ScriptableObject
{
    /*
     * Spell is the parent ScriptableObject that defines the shape of a Spell
     */
    public string m_SpellName = "New Spell";
    public string m_SpellDescription = "This is a description";
    public float m_SpellCooldown = 0.0f;
    public SpellCastType m_CastType = SpellCastType.FireAndForget;

    // how much time the instantiated prefab shoud live. If the default
    // duration is set to 0, it won't be destroyed with a timer
    public float m_SpellDuration = 0.0f;
    public int m_SpellCharges = 1;
    public bool m_UseAllAvailableCharges = false;
    public Sprite m_SpellImage;  // Spell Icon image
    public Sprite m_BorderImage; // Border of the spell icon
    public GameObject m_Prefab;  // SpellPrefab

    public AudioClip m_SpellSound; // Sound to play when the spell is cast
    public GameObject m_OnHitEffect; // FX to play if the spell hitted something


    //TODO: Start using m_Invalid and create Collision Layer variable for spell
    public List<string> m_Invalid = SpellUtilities.invalid; // List of tags of entities
                                                            // that can't interact with the spell
    public abstract GameObject CastSpell(
        GameObject owner, 
        GameObject target = null, 
        Vector3? spawn_pos = null, 
        Quaternion? spawn_rot = null); // What happens when the player casts such 
    public abstract void StopConcentration(GameObject owner = null); // What happens when the player casts such 
    public abstract void OnTick(GameObject obj); // Function to be called on the instantiated prefab Update()
    public abstract void SpellCollisionEnter(Collision2D target, GameObject src);
    public abstract void SpellCollisionTick(Collision2D target, GameObject src);
    public abstract void SpellTriggerEnter(Collider2D target, GameObject src);
    public abstract void SpellTriggerTick(Collider2D target, GameObject src);

}

[System.Serializable]
public class SpellData
{
    // TODO: make those booleans a spell cast state enum
    public Spell m_Spell = null;
    [HideInInspector] public float m_RemainingCD;
    [HideInInspector] public bool m_IsSpellOnCD;
    [HideInInspector] public bool m_UsingCharges;
    [HideInInspector] public bool m_OverCharging;
    [HideInInspector] public GameObject m_Owner; // Who is casting the spell
    [HideInInspector] public int m_RemainingCharges;
    [HideInInspector] public Transform m_SpawnPoint;
    [HideInInspector] public Transform m_SpawnParent;

    [HideInInspector] public GameObject m_InstantiatedSpell = null;
    [HideInInspector] public bool m_IsConcentrating = false;
    public void StartSpellData(GameObject owner, Transform pt, Transform pt_parent)
    {
        if (!m_Spell) {
            Debug.Log("spell was null!!!");
            return;
        }
        m_Owner = owner;
        m_RemainingCD = 0.0f;
        m_IsSpellOnCD = false;
        m_RemainingCharges = m_Spell.m_SpellCharges;
        m_SpawnPoint = pt;
        m_SpawnParent = pt_parent;
    }

    public void SetOwner(GameObject obj) { m_Owner = obj; }
    public float GetTotalCD() { return m_Spell.m_SpellCooldown; }
    public bool CastSpell(GameObject target = null)
    {
        if (!m_Spell)
            return false;
        GameObject actual_target = m_Spell.m_CastType == SpellCastType.FireAndForget ? target : m_Owner;
        //if the spell is not on CD or it has charges
        if ((!m_IsSpellOnCD || m_RemainingCharges > 0) && !m_IsConcentrating && !m_Spell.m_UseAllAvailableCharges)
        {
            m_IsConcentrating = m_Spell.m_CastType == SpellCastType.Concentration;
            m_InstantiatedSpell = m_Spell.CastSpell(m_Owner, actual_target, m_SpawnPoint.position, m_SpawnParent.rotation);

            m_RemainingCharges--;
            // only update cooldown if it's not already on cooldown
            // i.e when there's no charges left 
            if (!m_IsSpellOnCD)
                m_RemainingCD = m_Spell.m_SpellCooldown;
            m_IsSpellOnCD = true;

            return true;
        }
        else if (m_Spell.m_UseAllAvailableCharges && !m_UsingCharges && m_RemainingCharges > 0)
        {
            m_IsConcentrating = m_Spell.m_CastType == SpellCastType.Concentration;
            m_RemainingCD = m_Spell.m_SpellCooldown;
            m_IsSpellOnCD = true;

            float delay = 0.05f;
            float ttl = delay * m_RemainingCharges*1.1f;
            GameplayStatics.Delay(delay*m_RemainingCD*1.1f, () => m_UsingCharges=true, () => m_UsingCharges = false);
            GameplayStatics.AddTimer(m_Owner, ttl, delay, () => {
                if (m_RemainingCharges > 0)
                {
                    m_InstantiatedSpell = m_Spell.CastSpell(m_Owner, actual_target, m_SpawnPoint.position, m_SpawnParent.rotation);
                    m_RemainingCharges--;
                }
            });

            return true;
        }
        else if (m_IsConcentrating)
        {
            m_Spell.StopConcentration(m_InstantiatedSpell);
            m_IsConcentrating = false;
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