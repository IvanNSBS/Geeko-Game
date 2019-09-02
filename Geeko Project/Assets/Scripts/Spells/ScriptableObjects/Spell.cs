using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class Spell : ScriptableObject
{
    public GameObject m_Prefab;
    public GameObject m_OnHitEffect;
    public string m_SpellName = "New Spell";
    public float m_SpellCooldown = 0.0f;
    public float m_SpellDuration = 0.0f;
    public Sprite m_GameSprite;
    public Sprite m_SpellImage;
    public Sprite m_BorderImage;
    public AudioClip m_SpellSound;
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
    [HideInInspector] public GameObject owner;
    public SpellData(Spell spell)
    {
        m_Spell = spell;
        m_RemainingCD = 0.0f;
        m_IsSpellOnCD = false;
    }
    public SpellData(Spell spell, GameObject obj)
    {
        m_Spell = spell;
        m_RemainingCD = 0.0f;
        m_IsSpellOnCD = false;
        owner = obj;
    }

    public void SetOwner(GameObject obj) { owner = obj; }
    public float GetTotalCD() { return m_Spell.m_SpellCooldown; }
    public bool CastSpell()
    {
        if (!m_IsSpellOnCD)
        {
            m_Spell.m_SpellOwner = owner;
            m_Spell.CastSpell();
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
            if (m_RemainingCD == 0.0f)
                m_IsSpellOnCD = false;
        }
    }
}