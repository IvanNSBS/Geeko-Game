using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCastingComponent : MonoBehaviour
{

    [SerializeField] private List<SpellData> m_Spells = new List<SpellData>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(SpellData spell in m_Spells)
            spell.StartSpellData(this.gameObject);
    }

    public SpellData GetSpell(int idx) {
        if (m_Spells.Count > idx && idx >= 0) {
            return m_Spells[idx];
        }
        else
            return null;
    }
    public bool CastSpell(int idx)
    {
        if (GetSpell(idx) != null){
            GetSpell(idx).CastSpell();
            return true;
        }
        return false;
    }

    // Update is called once per frame
    public void Update()
    {

        foreach(SpellData spell in m_Spells) {
            if (spell.m_IsSpellOnCD)
                spell.UpdateCD(Time.deltaTime);
        }
    }
}
