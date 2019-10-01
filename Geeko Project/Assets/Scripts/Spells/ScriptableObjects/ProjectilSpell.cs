using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (menuName = "Spells/ProjectileSpell")]
public class ProjectilSpell : Spell
{
    public float m_Damage = 10.0f;
    public float m_ProjectileSpeed = 700.0f;

    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner) {
            Vector3 dir = target == null ? ((Quaternion)spawn_rot * Vector3.right).normalized : (target.transform.position - owner.transform.position).normalized;
            Quaternion rot = GameplayStatics.GetRotationFromDir(dir);
            Vector2 speed = new Vector2(m_ProjectileSpeed * dir.x, m_ProjectileSpeed * dir.y);

            GameObject obj = SpellUtilities.InstantiateSpell(m_Prefab, owner, target, this, (Vector3)spawn_pos, rot, spell_velocity:speed, tag:"SpellUninteractive");
            // ParseSpellActions(this, obj, target);
            return obj;
        }
        return null;
    }
    public override void StopConcentration(GameObject owner = null)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTick(GameObject obj)
    {
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

        SpellUtilities.DamageOnCollide(target.gameObject, s_manager, m_Damage, SpellUtilities.invalid2);
        if (target_obj != s_manager.GetOwner() && GameplayStatics.ObjHasTag(target_obj, SpellUtilities.entities))
            GameObject.Destroy(src);
    }

    public override void SpellTriggerTick(Collider2D target, GameObject src)
    {
    }
}
