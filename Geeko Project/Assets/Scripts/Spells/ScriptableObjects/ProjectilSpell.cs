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

    public override void CastSpell(GameObject owner, Transform inst_transform = null)
    {
        if (m_Prefab && owner) {
            Vector3 from = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 at = owner.transform.position;
            Vector2 dir = new Vector2(from.x - at.x, from.y - at.y).normalized;

            Vector3 pos = owner.transform.position;
            Quaternion rot = GameplayStatics.GetRotationFromDir(dir);
            Vector2 speed = new Vector2(m_ProjectileSpeed * dir.x, m_ProjectileSpeed * dir.y);

            GameObject obj = SpellUtilities.InstantiateSpell(m_Prefab, owner, this, pos, rot, spell_velocity:speed, tag:"SpellUninteractive");
            obj.GetComponent<SpellPrefabManager>().AddTriggerEnter(this.Collide);
        }
    }

    public void Collide(Collider2D target, GameObject source_obj)
    {
        GameObject target_obj = target.gameObject;
        SpellPrefabManager s_manager = source_obj.GetComponent<SpellPrefabManager>();
        if (target_obj != s_manager.GetOwner() && (target_obj.GetComponent<StatusComponent>() || target_obj.CompareTag("Wall") || target_obj.CompareTag("Door"))) {
            StatusComponent obj_status = target_obj.GetComponent<StatusComponent>();
            if(obj_status) obj_status.TakeDamage(m_Damage);

            if (s_manager) {
                if (s_manager.GetOwner() != target_obj)
                {
                    if (m_OnHitEffect) {
                        //TODO: Have collider info on collide function to spawn fx on the correct position
                        GameObject fx = Instantiate(m_OnHitEffect);
                        fx.transform.position = GameplayStatics.GetTriggerContactPoint(source_obj);
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
                    fx.transform.position = GameplayStatics.GetTriggerContactPoint(source_obj);
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
