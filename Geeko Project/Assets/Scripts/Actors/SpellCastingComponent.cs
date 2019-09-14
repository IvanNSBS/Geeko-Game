using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCastingComponent : MonoBehaviour
{
    //List of actor spells
    [SerializeField] private List<SpellData> m_Spells = new List<SpellData>();
    [SerializeField] Transform m_SpawnPoint = null;
    [SerializeField] Transform m_SpawnParent = null;


    private GameObject m_Target = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_SpawnPoint) m_SpawnPoint = gameObject.transform;
        if (!m_SpawnParent) m_SpawnParent = gameObject.transform;
        foreach(SpellData spell in m_Spells)
            spell.StartSpellData(this.gameObject, m_SpawnPoint, m_SpawnParent);
    }

    public void SetTarget(GameObject target) { m_Target = target; }

    public SpellData GetSpell(int idx) {
        if (m_Spells.Count > idx && idx >= 0) {
            return m_Spells[idx];
        }
        else
            return null;
    }
    public void CastSpell(int idx)
    {
        if (GetSpell(idx) != null){
            GetSpell(idx).CastSpell(m_Target);
        }
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
