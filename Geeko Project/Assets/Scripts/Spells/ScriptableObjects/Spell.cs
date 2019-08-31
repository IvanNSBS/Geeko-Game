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
