using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCastingComponent : MonoBehaviour
{
    [SerializeField] private Spell m_Spell1;
    [SerializeField] private Spell m_Spell2;
    [SerializeField] private Spell m_Spell3;

    [HideInInspector] public float m_Spell1CurrentCD = 0.0f;
    [HideInInspector] public float m_Spell2CurrentCD = 0.0f;
    [HideInInspector] public float m_Spell3CurrentCD = 0.0f;

    [HideInInspector] public bool m_Spell1OnCD = false;
    [HideInInspector] public bool m_Spell2OnCD = false;
    [HideInInspector] public bool m_Spell3OnCD = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Spell1.m_SpellOwner = this.gameObject;
        m_Spell2.m_SpellOwner = this.gameObject;
    }

    public float GetSpell1TotalCD() { return m_Spell1.m_SpellCooldown; }
    public float GetSpell2TotalCD() { return m_Spell2.m_SpellCooldown; }
    public float GetSpell3TotalCD() { return m_Spell3.m_SpellCooldown; }
    public Spell GetSpell1() { return m_Spell1; }
    public Spell GetSpell2() { return m_Spell2; }
    public Spell GetSpell3() { return m_Spell3; }
    public void CastSpell1()
    {
        if (!m_Spell1OnCD) {
            m_Spell1.m_SpellOwner = gameObject;
            m_Spell1.CastSpell();
            m_Spell1CurrentCD = m_Spell1.m_SpellCooldown;
            m_Spell1OnCD = true;
        }
    }
    public void CastSpell2() {
        if (!m_Spell2OnCD) {
            m_Spell2.m_SpellOwner = gameObject;
            m_Spell2.CastSpell();
            m_Spell2CurrentCD = m_Spell2.m_SpellCooldown;
            m_Spell2OnCD = true;
        }
    }

    // TODO: Create Spell Class (not scriptable object) that holds data such as
    // Current spell cd, spell is on cooldown, spell prefab, scriptable object asset, etc,
    // to generalize the current behavior
    public void CastSpell3() {
        if (!m_Spell3OnCD) {
            m_Spell3.m_SpellOwner = gameObject;
            m_Spell3.CastSpell();
            m_Spell3CurrentCD = m_Spell3.m_SpellCooldown;
            m_Spell3OnCD = true;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (m_Spell1OnCD)
        {
            m_Spell1CurrentCD -= Time.deltaTime;
            m_Spell1CurrentCD = Mathf.Clamp(m_Spell1CurrentCD, 0.0f, m_Spell1.m_SpellCooldown);
            if (m_Spell1CurrentCD == 0.0f)
                m_Spell1OnCD = false;
        }
        if (m_Spell2OnCD)
        {
            m_Spell2CurrentCD -= Time.deltaTime;
            m_Spell2CurrentCD = Mathf.Clamp(m_Spell2CurrentCD, 0.0f, m_Spell2.m_SpellCooldown);
            if (m_Spell2CurrentCD == 0.0f)
                m_Spell2OnCD = false;
        }
        if (m_Spell3OnCD)
        {
            m_Spell3CurrentCD -= Time.deltaTime;
            m_Spell3CurrentCD = Mathf.Clamp(m_Spell3CurrentCD, 0.0f, m_Spell3.m_SpellCooldown);
            if (m_Spell3CurrentCD == 0.0f)
                m_Spell3OnCD = false;
        }
    }
}
