using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image m_HealthBar;
    [SerializeField] private Text m_HealthText;

    [SerializeField] private Image m_Spell_1_Icon;
    [SerializeField] private Image m_Spell_1_Border;
    [SerializeField] private Image m_Spell_1_CD;
    [SerializeField] private TextMeshProUGUI m_Spell_1_CDtext;
    [SerializeField] private TextMeshProUGUI m_Spell_1_Charges;

    [SerializeField] private Image m_Spell_2_Icon;
    [SerializeField] private Image m_Spell_2_Border;
    [SerializeField] private Image m_Spell_2_CD;
    [SerializeField] private TextMeshProUGUI m_Spell_2_CDtext;
    [SerializeField] private TextMeshProUGUI m_Spell_2_Charges;

    private StatusComponent m_StatusComponent;
    private SpellCastingComponent m_SpellComponent;
    // Update is called once per frame
    private void Start()
    {
        if (!m_StatusComponent)
        {
            m_StatusComponent = GetComponent<StatusComponent>();
            if (!m_StatusComponent)
                Debug.LogWarning("Actor StatusComponent wasn't successfully set or found. Actor won't be able to benefit from this component");

            UpdateHealthBar();
        }


        //TODO: Make function to update spell UI when you change your spell
        if (!m_SpellComponent)
        {
            m_SpellComponent = GetComponent<SpellCastingComponent>();
            if (!m_SpellComponent)
                Debug.LogWarning("Actor SpellCastingComponent wasn't successfully set or found. Actor won't be able to benefit from this component");
            else {
                if (m_SpellComponent.GetSpell(0) != null){
                    m_Spell_1_Icon.sprite = m_SpellComponent.GetSpell(0).m_Spell.m_SpellImage;
                    m_Spell_1_Border.sprite = m_SpellComponent.GetSpell(0).m_Spell.m_BorderImage;

                    if (m_SpellComponent.GetSpell(0).m_Spell.m_SpellCharges == 1)
                        m_Spell_1_Charges.gameObject.SetActive(false);
                    else
                        m_Spell_1_Charges.text = m_SpellComponent.GetSpell(0).m_Spell.m_SpellCharges.ToString();
                }
                else m_Spell_1_Icon.gameObject.SetActive(false);

                if (m_SpellComponent.GetSpell(1) != null)
                {
                    m_Spell_2_Icon.sprite = m_SpellComponent.GetSpell(1).m_Spell.m_SpellImage;
                    m_Spell_2_Border.sprite = m_SpellComponent.GetSpell(1).m_Spell.m_BorderImage;

                    if (m_SpellComponent.GetSpell(1).m_Spell.m_SpellCharges == 1)
                        m_Spell_2_Charges.gameObject.SetActive(false);
                    else
                        m_Spell_2_Charges.text = m_SpellComponent.GetSpell(1).m_Spell.m_SpellCharges.ToString();
                }
                else m_Spell_2_Icon.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateHealthBar()
    {
        if(m_HealthBar)
            m_HealthBar.fillAmount = m_StatusComponent.GetCurrentHealth() / m_StatusComponent.GetMaxHealth();
        if (m_HealthText)
            m_HealthText.text = ((int)m_StatusComponent.GetCurrentHealth()).ToString() + " / " + ((int)m_StatusComponent.GetMaxHealth()).ToString();
    }

    public void Update()
    {
        UpdateSpellUI();
    }

    public void UpdateSpellUI()
    {
        if (m_SpellComponent.GetSpell(0) != null)
        {
            m_Spell_1_Charges.text = m_SpellComponent.GetSpell(0).m_RemainingCharges.ToString();
            if (!m_SpellComponent.GetSpell(0).m_IsSpellOnCD)
                m_Spell_1_CD.gameObject.SetActive(false);
            else {
                m_Spell_1_CD.gameObject.SetActive(true);
                float pct_s1 = m_SpellComponent.GetSpell(0).m_RemainingCD / m_SpellComponent.GetSpell(0).GetTotalCD();
                m_Spell_1_CD.fillAmount = pct_s1;
                m_Spell_1_CDtext.text = m_SpellComponent.GetSpell(0).m_RemainingCD.ToString("0.0");
            }
        }

        if (m_SpellComponent.GetSpell(1) != null)
        {
            m_Spell_2_Charges.text = m_SpellComponent.GetSpell(1).m_RemainingCharges.ToString();

            if (!m_SpellComponent.GetSpell(1).m_IsSpellOnCD)
                m_Spell_2_CD.gameObject.SetActive(false);
            else
            {
                m_Spell_2_CD.gameObject.SetActive(true);
                float pct_s2 = m_SpellComponent.GetSpell(1).m_RemainingCD / m_SpellComponent.GetSpell(1).GetTotalCD();
                m_Spell_2_CD.fillAmount = pct_s2;
                m_Spell_2_CDtext.text = m_SpellComponent.GetSpell(1).m_RemainingCD.ToString("0.0");

            }
        }

    }

}
