using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SpellSelectionComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject m_NewSpell;
    [SerializeField] private GameObject m_PlayerSpell1;
    [SerializeField] private GameObject m_PlayerSpell2;
    [SerializeField] private GameObject m_Player;

    private SpellCastingComponent m_SpellCastingComponent;
    private int m_SelectedSpellIdx = -1;
    private Spell m_NewSpellData;
    private GameObject m_SpellItem;
    void Start()
    {
        m_SpellCastingComponent = m_Player.GetComponent<SpellCastingComponent>();
        UpdateSpellInfo(m_PlayerSpell1, m_SpellCastingComponent.GetSpell(0).m_Spell);
        UpdateSpellInfo(m_PlayerSpell2, m_SpellCastingComponent.GetSpell(1).m_Spell);
    }
    public void UpdateSelectedIdx(int n_idx) { m_SelectedSpellIdx = n_idx; }
    public void SpawnSelection( Spell nspell, GameObject sitem ) {
        m_NewSpellData = nspell;
        gameObject.SetActive(true);
        m_SpellItem = sitem;
        UpdateSpells();
    }
    public void OnConfirm() {
        if (m_NewSpell && m_SelectedSpellIdx != -1)
        {
            m_SpellCastingComponent.SetSpellAt(m_SelectedSpellIdx, m_NewSpellData);
            m_Player.GetComponent<PlayerUIManager>().UpdateSpellIconAndBorders();
            gameObject.SetActive(false);
            Destroy(m_SpellItem);
        }
    }
    public void OnCancel() {
        UpdateSelectedIdx(-1);
        m_NewSpellData = null;
        m_SpellItem = null;
        this.gameObject.SetActive(false);
    }
    private void UpdateSpellInfo(GameObject spell, Spell spelldata)
    {
        int count = 0;
        spell.GetComponent<Image>().sprite = spelldata.m_SpellImage;
        foreach (Transform child in spell.transform)
        {
            if (count == 0)
                child.GetComponent<Image>().sprite = spelldata.m_BorderImage;
            else
                child.GetComponent<TextMeshProUGUI>().text = spelldata.m_SpellName;
            count++;
        }
    }
    private void UpdateSpells()
    {
        m_SpellCastingComponent = m_Player.GetComponent<SpellCastingComponent>();
        UpdateSpellInfo(m_NewSpell, m_NewSpellData);
        UpdateSpellInfo(m_PlayerSpell1, m_SpellCastingComponent.GetSpell(0).m_Spell);
        UpdateSpellInfo(m_PlayerSpell2, m_SpellCastingComponent.GetSpell(1).m_Spell);
    }
}
