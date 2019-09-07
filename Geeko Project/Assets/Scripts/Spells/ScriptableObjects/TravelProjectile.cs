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
    public override void CastSpell(GameObject owner, Transform inst_transform = null)
    {
        if (m_Prefab && owner) {
            Vector3 from = Input.mousePosition;
            from = Camera.main.ScreenToWorldPoint(from);
            Vector3 at = owner.transform.position;
            Vector2 vec = new Vector2(from.x - at.x, from.y - at.y);
            vec.Normalize();

            GameObject obj = Instantiate(m_Prefab);
            obj.GetComponent<Rigidbody2D>().velocity = new Vector2(m_ProjectileSpeed*vec.x, m_ProjectileSpeed*vec.y);
            obj.transform.position = owner.transform.position + new Vector3(vec.x*1.5f, vec.y*1.5f, 0.0f);
            obj.tag = "SpellUninteractive";

            float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
            obj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            obj.GetComponent<SpellPrefabManager>().SetOwner(owner);
            obj.GetComponent<SpellPrefabManager>().AddTriggerEnter(this.Collide);
            obj.GetComponent<SpellPrefabManager>().AddOnUpdate(this.OnTick);
        }
    }

    public void Collide(Collider2D target, GameObject source_obj)
    {
        GameObject target_obj = target.gameObject;
        SpellPrefabManager s_manager = source_obj.GetComponent<SpellPrefabManager>();
        if (s_manager)
        {
            if (target_obj != s_manager.GetOwner() && (target_obj.GetComponent<StatusComponent>() || target_obj.CompareTag("Wall") || target_obj.CompareTag("Door")))
            {
                if (m_SpellToCast)
                {
                    Debug.Log("Casting spell!");
                    Transform transf = source_obj.transform;
                    transf.position = GameplayStatics.GetTriggerContactPoint(source_obj);
                    m_SpellToCast.CastSpell(s_manager.GetOwner(), transf);

                }
                else
                    Debug.LogError("No spell to cast!");
                Destroy(source_obj);
            }
        }
        
    }

    public override void OnTick(GameObject obj)
    {
        GameObject obj_owner = obj.GetComponent<SpellPrefabManager>().GetOwner();
        if (!obj_owner)
            return;

        if((obj.transform.position - obj_owner.transform.position).magnitude > m_TravelDistance)
            if (m_SpellToCast)
            {
                Debug.Log("Casting spell!");
                m_SpellToCast.CastSpell(obj_owner, obj.transform);
                Destroy(obj);
            }
    }
}
