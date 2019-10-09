using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SpellPickupComponent : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider2D m_PickupArea;
    private Spell m_SpellObject;
    [SerializeField] private bool m_UseRandomDrop = false;
    [SerializeField] private List<Spell> m_PossibleSpells;

    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private Image m_Icon;
    [SerializeField] private Image m_Border;
    public Spell GetSpellObject() { return m_SpellObject; }
    public void SetSpellPickup(Spell spell)
    {
        if (m_Icon)
            m_Icon.sprite = spell.m_SpellImage;
        if (m_Border)
            m_Border.sprite = spell.m_BorderImage;
        if (m_Text)
            m_Text.text = spell.m_SpellName;
    }
    void GetOneRandomSpell()
    {
        int idx = Random.Range(0, m_PossibleSpells.Count);
        m_SpellObject = m_PossibleSpells[idx];
    }

    void Start()
    {
        if (m_UseRandomDrop)
            GetOneRandomSpell();
        m_PickupArea = GetComponent<BoxCollider2D>();
        if (!m_PickupArea)
            Debug.LogWarning("Couldn't get spell collider. You won't be able to pickup the item");

        SetSpellPickup(m_SpellObject);
        // m_StartPos = gameObject.transform.position;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player_spellcomponent = collision.gameObject.GetComponent<SpellCastingComponent>();
            var selection = collision.gameObject.GetComponentsInChildren<SpellSelectionComponent>(includeInactive: true);
            Debug.Log("trying to spawn spell list as: " + m_SpellObject.m_SpellName);
            selection[0].SpawnSelection( new List<Spell>() { m_SpellObject }, this.gameObject );
        }
    }

}
