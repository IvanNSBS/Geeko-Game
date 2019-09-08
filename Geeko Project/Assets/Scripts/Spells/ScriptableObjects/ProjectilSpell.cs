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

    public override void CastSpell(GameObject owner, Vector3? spawn_pos = null)
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
        if(m_OnHitEffect)
            SpellUtilities.SpawnEffectOnCollide(target_obj, s_manager, m_OnHitEffect, SpellUtilities.invalid);
        SpellUtilities.DamageOnCollide(target_obj, s_manager, m_Damage, SpellUtilities.invalid);

        if (target_obj != s_manager.GetOwner() && !GameplayStatics.ObjHasTag(target_obj, SpellUtilities.invalid))
            GameObject.Destroy(source_obj);
    }

    public override void OnTick(GameObject obj)
    {
        throw new System.NotImplementedException();
    }
}
