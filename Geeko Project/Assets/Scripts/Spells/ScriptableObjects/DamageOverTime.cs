using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (menuName = "Spells/DamageOverTime")]
public class DamageOverTime : Spell
{
    public float m_Damage = 1.0f;
    public float m_TickDealay = 0.3f;

    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner) {
            Debug.Log("Target = " + target);
            Quaternion rot = Quaternion.identity;
            Vector2 speed = new Vector2(0, 0);

            GameObject obj = SpellUtilities.InstantiateSpell(m_Prefab, owner, target, this, (Vector3)spawn_pos, rot, spell_velocity:speed, tick_delay:m_TickDealay ,tag:"SpellUninteractive");

            var sprite = obj.GetComponent<SpriteRenderer>();
            if (sprite)
                sprite.enabled = false;
            obj.GetComponent<SpellPrefabManager>().SetFollowerOwner(FollowWho.Target);

            return obj;
        }
        return null;
    }
    public override void StopConcentration(GameObject owner = null)
    {
    }

    public override void OnTick(GameObject obj)
    {
        var manager = obj.GetComponent<SpellPrefabManager>();
        GameplayStatics.ApplyDamage(manager.m_Target, m_Damage, GameplayStatics.DamageType.Fire);

    }
    public override void SpellCollisionEnter(Collision2D target, GameObject src)
    {
    }

    public override void SpellCollisionTick(Collision2D target, GameObject src)
    {
    }

    public override void SpellTriggerEnter(Collider2D target, GameObject src)
    {

    }

    public override void SpellTriggerTick(Collider2D target, GameObject src)
    {
    }
}
