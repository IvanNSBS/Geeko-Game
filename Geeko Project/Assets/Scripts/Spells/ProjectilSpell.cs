using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// THIS IS A TEST SCRIPT!!
[CreateAssetMenu (menuName = "Spells/ProjectileSpell")]
public class ProjectilSpell : Spell
{
    public float m_Damage = 10.0f;
    public float m_ProjectileSpeed = 10.0f;

    private float m_Magnitude = 10.0f;
    private SpellCastingComponent m_SpellComponent;

    public override void CastSpell()
    {
    }

    public override void Initialize(GameObject obj)
    {
        m_SpellComponent = obj.GetComponent<SpellCastingComponent>();
    }
}
