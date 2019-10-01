using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SpellSelectionComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject m_SingleSpellPickup;
    [SerializeField] private GameObject m_NewSpell;
    [SerializeField] private GameObject m_NewSpell1;
    [SerializeField] private GameObject m_NewSpell2;

    [SerializeField] private GameObject m_PlayerSpell1;
    [SerializeField] private GameObject m_PlayerSpell2;
    [SerializeField] private GameObject m_Player;
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private UISpellInfoComponent m_NewSpellInfo;
    [SerializeField] private UISpellInfoComponent m_OldSpellInfo;
    private SpellCastingComponent m_SpellCastingComponent;

    private int m_SelectedOldSpellIdx = -1;
    private int m_SelectedNewSpellIdx = 0;

    private List<Spell> m_NewSpellData = new List<Spell>();
    private Spell m_OldSpell;
    private GameObject m_SpellItem;
    private bool m_MultipleSelection = false;

    private int m_NumberOfPlayerSpells = 0;

    void Start()
    {
        m_SpellCastingComponent = m_Player.GetComponent<SpellCastingComponent>();
        //UpdateSpellInfo(m_PlayerSpell1, m_SpellCastingComponent.GetSpell(0).m_Spell);
        //UpdateSpellInfo(m_PlayerSpell2, m_SpellCastingComponent.GetSpell(1).m_Spell);
        //if (m_ConfirmButton)
        //{
        //    Debug.Log("Button Set");
        //    m_ConfirmButton.interactable = false;
        //}
        //if (m_NewSpellInfo)
        //    m_NewSpellInfo.gameObject.SetActive(false);
        //if (m_OldSpellInfo)
        //    m_OldSpellInfo.gameObject.SetActive(false);

    }
    public void UpdateOldSelectedIdx(int n_idx) {
        if (n_idx != -1 && m_SelectedNewSpellIdx != -1) 
            m_ConfirmButton.interactable = true;
        else
            m_ConfirmButton.interactable = false;

        m_SelectedOldSpellIdx = n_idx;
        if (m_OldSpellInfo && n_idx != -1 && m_NumberOfPlayerSpells >= 2) {
            m_OldSpellInfo.gameObject.SetActive(true);
            m_OldSpellInfo.UpdateInfo(m_SpellCastingComponent.GetSpell(n_idx).m_Spell);
        }
        else
            m_OldSpellInfo.gameObject.SetActive(false);

    }
    public void UpdateNewSelectedIdx(int n_idx)
    {
        Debug.Log("old selected idx = " + m_SelectedOldSpellIdx);
        Debug.Log("nidx = " + m_SelectedOldSpellIdx);
        m_SelectedNewSpellIdx = n_idx;
        if (n_idx != -1 && m_SelectedOldSpellIdx != -1) {
            Debug.Log("Button is interactanble");
            m_ConfirmButton.interactable = true;
        }
        else
            m_ConfirmButton.interactable = false;

        if (m_NewSpellInfo && n_idx != -1) {
            Debug.Log("Here i am");
            m_NewSpellInfo.gameObject.SetActive(true);
            m_NewSpellInfo.UpdateInfo(m_NewSpellData[n_idx]);
        }
        else
            m_NewSpellInfo.gameObject.SetActive(false);

    }
    public void SpawnSelection( List<Spell> nspell, GameObject sitem, bool multiple = false ) {
        gameObject.SetActive(true);

        m_SpellCastingComponent = m_Player.GetComponent<SpellCastingComponent>();
        m_MultipleSelection = multiple;
        m_NewSpell1.SetActive(multiple);
        m_NewSpell2.SetActive(multiple);
        m_NewSpellData = nspell;

        m_SelectedNewSpellIdx = nspell.Count > 1 ? -1 : 0;

        m_SpellItem = sitem;

        m_NumberOfPlayerSpells = 0;
        m_NumberOfPlayerSpells = m_SpellCastingComponent.GetSpell(0).m_Spell == null ? m_NumberOfPlayerSpells : m_NumberOfPlayerSpells + 1;
        m_NumberOfPlayerSpells = m_SpellCastingComponent.GetSpell(1).m_Spell == null ? m_NumberOfPlayerSpells : m_NumberOfPlayerSpells + 1;

        if (m_NumberOfPlayerSpells == 1)
            UpdateOldSelectedIdx(1);
        else if (m_NumberOfPlayerSpells == 0)
            UpdateOldSelectedIdx(0);
        else
            UpdateOldSelectedIdx(-1);
        UpdateNewSelectedIdx(m_SelectedNewSpellIdx);

        UpdateSpells();

        Time.timeScale = 0;
    }
    public void OnConfirm() {
        if (m_NewSpell && m_SelectedOldSpellIdx != -1)
        {
            m_OldSpell = m_SpellCastingComponent.GetSpell(m_SelectedOldSpellIdx).m_Spell;
            m_SpellCastingComponent.SetSpellAt(m_SelectedOldSpellIdx, m_NewSpellData[m_SelectedNewSpellIdx]);
            m_Player.GetComponent<PlayerUIManager>().UpdateSpellIconAndBorders();
            gameObject.SetActive(false);
            Destroy(m_SpellItem);
            if (m_OldSpell)
            {
                var pickup_prefab = Instantiate(m_SingleSpellPickup);
                var mv = m_SpellCastingComponent.gameObject.GetComponent<MovementComponent>();
                float dir = mv.GetSprite().flipX ? 1 : -1;
                pickup_prefab.transform.position = m_SpellCastingComponent.gameObject.transform.position + new Vector3(0.9f * dir, 0, 0);
                var pickup = pickup_prefab.GetComponent<SpellPickupComponent>();
                pickup.SetSpellPickup(m_OldSpell);
            }
        }
        m_OldSpell = null;
        m_NewSpellData = null;
        UpdateOldSelectedIdx(-1);
        UpdateNewSelectedIdx(-1);
        Time.timeScale = 1;
    }
    public void OnCancel() {
        UpdateOldSelectedIdx(-1);
        UpdateNewSelectedIdx(-1);
        m_NewSpellData = null;
        m_SpellItem = null;
        this.gameObject.SetActive(false);
        Time.timeScale = 1;

    }
    private void UpdateSpellInfo(GameObject spell, Spell spelldata)
    {
        if (!spelldata) {
            spell.SetActive(false);
            return;
        }
        spell.SetActive(true);
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
        UpdateSpellInfo(m_NewSpell, m_NewSpellData[0]);
        if(m_NewSpellData.Count > 1)
        {
            UpdateSpellInfo(m_NewSpell1, m_NewSpellData[1]);
            UpdateSpellInfo(m_NewSpell2, m_NewSpellData[2]);
        }
        if (m_NumberOfPlayerSpells == 2) {
            UpdateSpellInfo(m_PlayerSpell1, m_SpellCastingComponent.GetSpell(0).m_Spell);
            UpdateSpellInfo(m_PlayerSpell2, m_SpellCastingComponent.GetSpell(1).m_Spell);
        }
        else
        {
            UpdateSpellInfo(m_PlayerSpell1, null);
            UpdateSpellInfo(m_PlayerSpell2, null);
        }
    }
}
