using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (menuName = "Spells/TravellingSpell")]
public class TravellingSpell : Spell
{
    public Spell m_SpellToCast;
    public float m_TravelDistance = 10.0f;
    public float m_ProjectileSpeed = 1.0f;
    public override void CastSpell()
    {
        if (m_Prefab && m_SpellOwner) {
            Vector3 from = Input.mousePosition;
            from = Camera.main.ScreenToWorldPoint(from);
            Vector3 at = m_SpellOwner.transform.position;
            Vector2 vec = new Vector2(from.x - at.x, from.y - at.y);
            vec.Normalize();

            GameObject obj = Instantiate(m_Prefab);
            obj.GetComponent<Rigidbody2D>().velocity = new Vector2(m_ProjectileSpeed*vec.x, m_ProjectileSpeed*vec.y);
            obj.transform.position = m_SpellOwner.transform.position + new Vector3(vec.x*1.5f, vec.y*1.5f, 0.0f);

            float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
            obj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            obj.GetComponent<SpellPrefabManager>().SetOwner(m_SpellOwner);
            obj.GetComponent<SpellPrefabManager>().AddCollideEnter(this.Collide);
            obj.GetComponent<SpellPrefabManager>().AddTriggerEnter(this.Collide);
            obj.GetComponent<SpellPrefabManager>().AddOnUpdate(this.OnTick);
        }
    }

    public void Collide(GameObject target_obj, GameObject source_obj) {

        SpellPrefabManager s_manager = target_obj.GetComponent<SpellPrefabManager>();
        if (target_obj != m_SpellOwner && (target_obj.GetComponent<StatusComponent>() || target_obj.CompareTag("Wall") || target_obj.CompareTag("Door"))) {
            if (s_manager) {
                if (s_manager.GetOwner() != m_SpellOwner)
                {
                    if (m_SpellToCast) {
                        Debug.Log("Casting spell!");
                        m_SpellToCast.m_SpellOwner = m_SpellOwner;
                        m_SpellToCast.CastSpell();
                    }
                    else
                        Debug.LogError("No spell to cast!");
                    Destroy(source_obj);
                }
            }
            else
            {
                if (m_SpellToCast) {
                    Debug.Log("Casting spell!");
                    m_SpellToCast.m_SpellOwner = m_SpellOwner;
                    m_SpellToCast.CastSpell();
                }
                else
                    Debug.LogError("No spell to cast!");
                Destroy(source_obj);
            }
        }
    }

    public override void OnTick(GameObject obj)
    {
        if((obj.transform.position - m_SpellOwner.transform.position).magnitude > m_TravelDistance)
            if (m_SpellToCast)
            {
                Debug.Log("Casting spell!");
                m_SpellToCast.m_SpellOwner = m_SpellOwner;
                m_SpellToCast.CastSpell();
                Destroy(obj);
            }
    }
}
