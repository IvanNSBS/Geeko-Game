using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// THIS IS A TEST SCRIPT!!
[CreateAssetMenu (menuName = "Spells/ProjectileSpell")]
public class ProjectilSpell : Spell
{
    public float m_Damage = 10.0f;
    public float m_ProjectileSpeed = 700.0f;

    private float m_Magnitude = 10.0f;
    private SpellCastingComponent m_SpellComponent;

    public override void CastSpell()
    {
        if (m_Prefab && gameObject) {
            Vector3 from = Input.mousePosition;
            from = Camera.main.ScreenToWorldPoint(from);
            Vector3 at = gameObject.transform.position;

            Vector2 vec = new Vector2(from.x - at.x, from.y - at.y);
            vec.Normalize();

            GameObject obj;
            obj = Instantiate(m_Prefab);
            obj.GetComponent<Rigidbody2D>().velocity = new Vector2(m_ProjectileSpeed*vec.x, m_ProjectileSpeed*vec.y);
            obj.transform.position = gameObject.transform.position + new Vector3(vec.x*0.2f, vec.y*0.2f, 0.0f);
            // obj.transform.rotation = ;
        }
    }

    public override void Initialize(GameObject obj)
    {
        m_SpellComponent = obj.GetComponent<SpellCastingComponent>();
    }
}
