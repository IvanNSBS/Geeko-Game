using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MultipleSpellPickupComponent : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider2D m_PickupArea;
    List<Spell> m_SpellObjects = new List<Spell>(3);
    [SerializeField] private List<GameObject> m_SpellIcons;
    [SerializeField] private List<Spell> m_PossibleSpells;
    private Vector3 m_StartPos;
    public Spell GetSpellObjectAt(int idx) {
        if (idx < 0 && idx >= m_SpellObjects.Count)
            return null;
        return m_SpellObjects[idx];
    }
    public void SetMultipleSpellPickup(List<Spell> spells)
    {
        m_SpellObjects = spells;
        if (m_SpellObjects.Count > 0 && m_SpellIcons.Count > 0)
        {
            int cur_idx = 0;
            foreach (GameObject child in m_SpellIcons)
            {
                var img = child.transform.GetChild(0).GetComponent<SpriteRenderer>();
                var border = child.transform.GetChild(1).GetComponent<SpriteRenderer>();
                var text = child.transform.GetChild(2).GetComponent<TextMeshPro>();

                img.sprite = m_SpellObjects[cur_idx].m_SpellImage;
                border.sprite = m_SpellObjects[cur_idx].m_BorderImage;
                text.text = m_SpellObjects[cur_idx].m_SpellName;

                cur_idx++;
            }
        }
    }

    void GetThreeRandomSpells()
    {
        List<int> already_used = new List<int>();
        for (int i = 0; i < m_PossibleSpells.Count; i++) {

            int idx = Random.Range(0, m_PossibleSpells.Count);
            while (already_used.Contains(idx))
                idx = Random.Range(0, m_PossibleSpells.Count);

            already_used.Add(idx);
            m_SpellObjects.Add(m_PossibleSpells[idx]); 
        }
    }

    void Start()
    {
        m_SpellObjects = new List<Spell>();
        if (m_PossibleSpells.Count < 3)
            return;
        m_PickupArea = GetComponent<BoxCollider2D>();
        if (!m_PickupArea)
            Debug.LogWarning("Couldn't get spell collider. You won't be able to pickup the item");

        m_StartPos = gameObject.transform.position;
        GetThreeRandomSpells();
        SetMultipleSpellPickup(m_SpellObjects);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player_spellcomponent = collision.gameObject.GetComponent<SpellCastingComponent>();
            var selection = collision.gameObject.GetComponentsInChildren<SpellSelectionComponent>(includeInactive: true);
            selection[0].SpawnSelection( m_SpellObjects, this.gameObject, multiple: true );
        }
    }
}
