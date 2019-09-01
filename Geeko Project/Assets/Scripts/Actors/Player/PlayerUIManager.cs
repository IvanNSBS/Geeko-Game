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

    [SerializeField] private Image m_Spell_2_Icon;
    [SerializeField] private Image m_Spell_2_Border;
    [SerializeField] private Image m_Spell_2_CD;
    [SerializeField] private TextMeshProUGUI m_Spell_2_CDtext;

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
                if (m_SpellComponent.GetSpell1()){
                    m_Spell_1_Icon.sprite = m_SpellComponent.GetSpell1().m_SpellImage;
                    m_Spell_1_Border.sprite = m_SpellComponent.GetSpell1().m_BorderImage;
                }
                else m_Spell_1_Icon.gameObject.SetActive(false);

                if (m_SpellComponent.GetSpell2())
                {
                    m_Spell_2_Icon.sprite = m_SpellComponent.GetSpell2().m_SpellImage;
                    m_Spell_2_Border.sprite = m_SpellComponent.GetSpell2().m_BorderImage;
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
        if (m_SpellComponent.GetSpell1())
        {
            if (!m_SpellComponent.m_Spell1OnCD)
                m_Spell_1_CD.gameObject.SetActive(false);
            else {
                m_Spell_1_CD.gameObject.SetActive(true);
                float pct_s1 = m_SpellComponent.m_Spell1CurrentCD / m_SpellComponent.GetSpell1TotalCD();
                m_Spell_1_CD.fillAmount = pct_s1;
                m_Spell_1_CDtext.text = m_SpellComponent.m_Spell1CurrentCD.ToString("0.0");
            }
        }

        if (m_SpellComponent.GetSpell2())
        {
            if (!m_SpellComponent.m_Spell2OnCD)
                m_Spell_2_CD.gameObject.SetActive(false);
            else
            {
                m_Spell_2_CD.gameObject.SetActive(true);
                float pct_s2 = m_SpellComponent.m_Spell2CurrentCD / m_SpellComponent.GetSpell2TotalCD();
                m_Spell_2_CD.fillAmount = pct_s2;
                m_Spell_2_CDtext.text = m_SpellComponent.m_Spell2CurrentCD.ToString("0.0");
            }
        }

    }

}
