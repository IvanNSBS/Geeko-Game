using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class Spell : ScriptableObject
{
    public float m_SpellCooldown = 0.0f;
    public float m_SpellDuration = 0.0f;
    public int m_SpellCharges = 1;

    public Sprite m_GameSprite;
    public Sprite m_SpellImage;
    public Sprite m_BorderImage;
    public AudioClip m_SpellSound;
    public GameObject m_OnHitEffect;

    public GameObject m_Prefab;

    public List<string> m_ActorsToIgnore = new List<string>(); // List of tags of valid entities
                                                               // that can interact with the spell
    [HideInInspector] public GameObject m_SpellOwner;
    public abstract void Initialize(GameObject obj);
    public abstract void CastSpell();
    public abstract void ApplyDamage(GameObject obj);
    public abstract void OnTick(GameObject obj);
}

[System.Serializable]
public class SpellData
{
    public Spell m_Spell;
    [HideInInspector] public float m_RemainingCD;
    [HideInInspector] public bool m_IsSpellOnCD;
    [HideInInspector] public GameObject m_Owner;
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
        if (!m_IsSpellOnCD || m_RemainingCharges > 0)
        {
            m_Spell.m_SpellOwner = m_Owner;
            m_Spell.CastSpell();

            m_RemainingCharges--;
            if(!m_IsSpellOnCD)
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
            m_RemainingCD -= delta_time;
            m_RemainingCD = Mathf.Clamp(m_RemainingCD, 0.0f, m_Spell.m_SpellCooldown);
            if (m_RemainingCD == 0.0f) {
                if (m_RemainingCharges + 1 <= m_Spell.m_SpellCharges) {
                    m_RemainingCharges++;
                    m_RemainingCD = m_Spell.m_SpellCooldown;
                }
                if(m_RemainingCharges == m_Spell.m_SpellCharges)
                    m_IsSpellOnCD = false;
            }
        }
    }
}