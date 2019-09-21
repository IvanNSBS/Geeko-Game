using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SpellPickupComponent : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider2D m_PickupArea;
    [SerializeField] private Spell m_SpellObject;

    public Spell GetSpellObject() { return m_SpellObject; }

    void Start()
    {
        m_PickupArea = GetComponent<BoxCollider2D>();
        if (!m_PickupArea)
            Debug.LogWarning("Couldn't get spell collider. You won't be able to pickup the item");

        if (m_SpellObject)
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                if (count < 2)
                {
                    var sprite = child.GetComponent<SpriteRenderer>();
                    if (count == 0)
                        sprite.sprite = m_SpellObject.m_SpellImage;
                    else if (count == 1)
                        sprite.sprite = m_SpellObject.m_BorderImage;
                }
                else
                {
                    var text = child.GetComponent<TextMeshPro>();
                    text.text = m_SpellObject.m_SpellName;
                }
                count++;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player_spellcomponent = collision.gameObject.GetComponent<SpellCastingComponent>();
            var selection = collision.gameObject.GetComponentsInChildren<SpellSelectionComponent>(includeInactive: true);
            selection[0].SpawnSelection( m_SpellObject, this.gameObject );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
