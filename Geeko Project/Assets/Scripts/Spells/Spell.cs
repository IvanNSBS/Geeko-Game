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
    [SerializeField] public GameObject m_Prefab;

    public abstract void Initialize(GameObject obj);
    public abstract void CastSpell(); 
}
