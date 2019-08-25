using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class Spell : ScriptableObject
{
    public string m_SpellName = "New Spell";
    public float m_SpellCooldown = 0.0f;
    public Sprite m_GameSprite;
    public Image m_SpellImage;
    public Image m_BorderImage;
    public AudioClip m_SpellSound;
    public List<string> m_ActorsToIgnore = new List<string>(); // List of tags of valid entities
                                                               // that can interact with the spell
    [HideInInspector] public GameObject m_SpellOwner;
    [HideInInspector] public GameObject m_Prefab;
    [HideInInspector] public GameObject m_InstantiatedPrefab;

    public abstract void Initialize(GameObject obj);
    public abstract void CastSpell();
    public abstract void ApplyDamage(GameObject obj);
}
