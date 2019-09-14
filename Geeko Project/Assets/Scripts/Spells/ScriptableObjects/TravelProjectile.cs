using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Spells/TravelSpell")]
public class TravelProjectile : Spell
{
    public Spell m_SpellToCast;
    public float m_TravelDistance = 10.0f;
    public float m_ProjectileSpeed = 1.0f;
    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner) {
            Vector3 dir = target == null ? ((Quaternion)spawn_rot * Vector3.right).normalized : (target.transform.position - owner.transform.position).normalized;
            Quaternion rot = GameplayStatics.GetRotationFromDir(dir);
            Vector2 speed = new Vector2(m_ProjectileSpeed * dir.x, m_ProjectileSpeed * dir.y);

            GameObject obj = SpellUtilities.InstantiateSpell(m_Prefab, owner, target, this, (Vector3)spawn_pos, rot, spell_velocity: speed, tag: "SpellUninteractive");
            return obj;
        }

        return null;
    }
    public override void OnTick(GameObject obj)
    {
        GameObject obj_owner = obj.GetComponent<SpellPrefabManager>().GetOwner();
        if (!obj_owner)
            return;

        if ((obj.transform.position - obj_owner.transform.position).magnitude > m_TravelDistance)
            if (m_SpellToCast)
            {
                m_SpellToCast.CastSpell(obj_owner, null, obj.transform.position);
                Destroy(obj);
            }
    }
    public override void StopConcentration(GameObject owner = null)
    {
        throw new System.NotImplementedException();
    }

    public override void SpellCollisionEnter(Collision2D target, GameObject src)
    {
    }

    public override void SpellCollisionTick(Collision2D target, GameObject src)
    {
    }

    public override void SpellTriggerEnter(Collider2D target, GameObject src)
    {
        GameObject target_obj = target.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();
        SpellUtilities.CastSpellOnCollide(target.gameObject, s_manager, m_SpellToCast, GameplayStatics.GetTriggerContactPoint(src), SpellUtilities.invalid);
        if (target_obj != s_manager.GetOwner() && !GameplayStatics.ObjHasTag(target_obj, SpellUtilities.invalid))
            GameObject.Destroy(src);
    }

    public override void SpellTriggerTick(Collider2D target, GameObject src)
    {
    }
}
