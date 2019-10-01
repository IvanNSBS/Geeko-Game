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
    [SerializeField] private Image m_Spell_1_CD; // Spell CD square
    [SerializeField] private TextMeshProUGUI m_Spell_1_CDtext; // Remaining CD text
    [SerializeField] private TextMeshProUGUI m_Spell_1_Charges; // Spell Charges display

    [SerializeField] private Image m_Spell_2_Icon;
    [SerializeField] private Image m_Spell_2_Border;
    [SerializeField] private Image m_Spell_2_CD; // Spell CD square
    [SerializeField] private TextMeshProUGUI m_Spell_2_CDtext; // Remaining CD text
    [SerializeField] private TextMeshProUGUI m_Spell_2_Charges; // Spell Charges display

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

            UpdateHealthBar(0); // Update Canvas HealthBar
        }
        if (!m_SpellComponent)
        {
            m_SpellComponent = GetComponent<SpellCastingComponent>();
            if (!m_SpellComponent)
                Debug.LogWarning("Actor SpellCastingComponent wasn't successfully set or found. Actor won't be able to benefit from this component");
        }
        UpdateSpellIconAndBorders();
    }

    public void UpdateHealthBar(float damage)
    {
        if(m_HealthBar)
            m_HealthBar.fillAmount = m_StatusComponent.GetCurrentHealth() / m_StatusComponent.GetMaxHealth(); // get pct of fill
        if (m_HealthText)
            m_HealthText.text = ((int)m_StatusComponent.GetCurrentHealth()).ToString() + " / " + ((int)m_StatusComponent.GetMaxHealth()).ToString(); // update txt
    }

    public void ToggleImageChildren(Image img, bool show = true) {
        for( int i = 1; i < img.transform.childCount; i++)
            img.transform.GetChild(i).gameObject.SetActive(show);
        img.color = show ? new Color(1, 1, 1, 0.47f) : new Color(0, 0, 0, 0.47f);
    }

    public void UpdateSpellIconAndBorders()
    {
        if(m_SpellComponent)
        {
            if (m_SpellComponent.GetSpell(0).m_Spell != null)
            { // If player does have Spell1 Set
                ToggleImageChildren(m_Spell_1_Icon, true);
                m_Spell_1_Icon.sprite = m_SpellComponent.GetSpell(0).m_Spell.m_SpellImage; // set UI Image
                m_Spell_1_Border.sprite = m_SpellComponent.GetSpell(0).m_Spell.m_BorderImage; // Set UI border

                if (m_SpellComponent.GetSpell(0).m_Spell.m_SpellCharges == 1) // if the spell only has 1 charge, do not display it
                    m_Spell_1_Charges.gameObject.SetActive(false);
                else {
                    m_Spell_1_Charges.text = m_SpellComponent.GetSpell(0).m_Spell.m_SpellCharges.ToString(); // show and update it otherwise
                    m_Spell_1_Charges.gameObject.SetActive(true);
                }
            }
            else ToggleImageChildren(m_Spell_1_Icon, false); ; // hide spell if player dont have it

            if (m_SpellComponent.GetSpell(1).m_Spell != null) // If player does have Spell2 Set
            {
                ToggleImageChildren(m_Spell_2_Icon, true); //m_Spell_2_Icon.gameObject.SetActive(true);
                m_Spell_2_Icon.sprite = m_SpellComponent.GetSpell(1).m_Spell.m_SpellImage; // Set UI Image
                m_Spell_2_Border.sprite = m_SpellComponent.GetSpell(1).m_Spell.m_BorderImage; // Set UI Border

                if (m_SpellComponent.GetSpell(1).m_Spell.m_SpellCharges == 1) // if the spell only has 1 charge, do not display it
                    m_Spell_2_Charges.gameObject.SetActive(false);
                else {
                    m_Spell_2_Charges.text = m_SpellComponent.GetSpell(1).m_Spell.m_SpellCharges.ToString(); // show and update it otherwise
                    m_Spell_2_Charges.gameObject.SetActive(true);
                }
            }
            else ToggleImageChildren(m_Spell_2_Icon, false); ; // hide spell if player dont have it
        }
    }

    public void UpdateSpellUI()
    {
        if (m_SpellComponent.GetSpell(0).m_Spell != null)
        {
            m_Spell_1_Charges.text = m_SpellComponent.GetSpell(0).m_RemainingCharges.ToString();// update charges text

            if (!m_SpellComponent.GetSpell(0).m_IsSpellOnCD) // hide rotating CD if spell is not on CD
                m_Spell_1_CD.gameObject.SetActive(false);
            else {
                m_Spell_1_CD.gameObject.SetActive(true); // show it and update remaining CD display
                float pct_s1 = m_SpellComponent.GetSpell(0).m_RemainingCD / m_SpellComponent.GetSpell(0).GetTotalCD();
                m_Spell_1_CD.fillAmount = pct_s1;
                m_Spell_1_CDtext.text = m_SpellComponent.GetSpell(0).m_RemainingCD.ToString("0.0"); // update remaining CD txt
            }
        }

        if (m_SpellComponent.GetSpell(1).m_Spell != null)
        {
            m_Spell_2_Charges.text = m_SpellComponent.GetSpell(1).m_RemainingCharges.ToString(); // update charges text

            if (!m_SpellComponent.GetSpell(1).m_IsSpellOnCD) // hide rotating CD if spell is not on CD
                m_Spell_2_CD.gameObject.SetActive(false);
            else
            {
                m_Spell_2_CD.gameObject.SetActive(true); // show it and update remaining CD display
                float pct_s2 = m_SpellComponent.GetSpell(1).m_RemainingCD / m_SpellComponent.GetSpell(1).GetTotalCD();
                m_Spell_2_CD.fillAmount = pct_s2;
                m_Spell_2_CDtext.text = m_SpellComponent.GetSpell(1).m_RemainingCD.ToString("0.0"); // update remaining CD txt
            }
        }
    }

    public void Update()
    {
        UpdateSpellUI();
    }

}
