using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// THIS IS A TEST SCRIPT!!
[CreateAssetMenu (menuName = "Spells/ProjectileSpell")]
public class ProjectilSpell : Spell
{
    public float m_Damage = 10.0f;
    public float m_ProjectileSpeed = 700.0f;

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
        }
    }

    public void Collide(GameObject target_obj, GameObject source_obj) {

        SpellPrefabManager s_manager = target_obj.GetComponent<SpellPrefabManager>();
        if (target_obj != m_SpellOwner && (target_obj.GetComponent<StatusComponent>() || target_obj.CompareTag("Wall") || target_obj.CompareTag("Door"))) {
            StatusComponent obj_status = target_obj.GetComponent<StatusComponent>();
            if(obj_status) obj_status.TakeDamage(m_Damage);

            if (s_manager) {
                if (s_manager.GetOwner() != m_SpellOwner)
                {
                    if (m_OnHitEffect) {
                        //TODO: Have collider info on collide function to spawn fx on the correct position
                        GameObject fx = Instantiate(m_OnHitEffect);
                        fx.transform.position = target_obj.transform.position;
                    }
                    Destroy(source_obj);
                }
            }
            else
            {
                if (m_OnHitEffect)
                {
                    //TODO: Have collider info on collide function to spawn fx on the correct position
                    GameObject fx = Instantiate(m_OnHitEffect);
                    fx.transform.position = target_obj.transform.position;
                }
                Destroy(source_obj);
            }
        }
    }

    public override void OnTick(GameObject obj)
    {
        throw new System.NotImplementedException();
    }
}
