using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UISpellInfoComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_SpellName;
    [SerializeField] private TextMeshProUGUI m_Type;
    [SerializeField] private TextMeshProUGUI m_Chargeable;
    [SerializeField] private TextMeshProUGUI m_Uses;
    [SerializeField] private TextMeshProUGUI m_Cooldown;
    [SerializeField] private TextMeshProUGUI m_Description;

    public void UpdateInfo(Spell spell)
    {
        m_SpellName.text = spell.m_SpellName;
        m_Type.text = "Type: " + spell.m_CastType.ToString();
        m_Chargeable.text = "Chargeable:" + "No";
        m_Uses.text = "Uses: " + spell.m_SpellCharges.ToString();
        m_Cooldown.text = "Cooldown: "+ spell.m_SpellCooldown.ToString();
        m_Description.text = spell.m_SpellDescription;
    }
}
