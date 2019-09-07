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
            Vector3 from = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 at = owner.transform.position;
            Vector2 dir = new Vector2(from.x - at.x, from.y - at.y).normalized;

            Vector3 pos = owner.transform.position;
            Quaternion rot = GameplayStatics.GetRotationFromDir(dir);
            Vector2 speed = new Vector2(m_ProjectileSpeed * dir.x, m_ProjectileSpeed * dir.y);

            GameObject obj = SpellUtilities.InstantiateSpell(m_Prefab, owner, this, pos, rot, spell_velocity: speed, tag: "SpellUninteractive");
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
