using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SpellPickupComponent : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider2D m_PickupArea;
    private Spell m_SpellObject;
    private Vector3 m_StartPos;
    public Spell GetSpellObject() { return m_SpellObject; }
    public void SetSpellPickup(Spell spell)
    {
        m_SpellObject = spell;
        Debug.Log("Setting spell to: " + m_SpellObject.m_SpellName);
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

    void Start()
    {
        m_PickupArea = GetComponent<BoxCollider2D>();
        if (!m_PickupArea)
            Debug.LogWarning("Couldn't get spell collider. You won't be able to pickup the item");

        // SetSpellPickup(m_SpellObject);
        m_StartPos = gameObject.transform.position;
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

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = m_StartPos + new Vector3(0, Mathf.Sin(Time.time)*5.0f, 0) * Time.deltaTime;
    }
}
