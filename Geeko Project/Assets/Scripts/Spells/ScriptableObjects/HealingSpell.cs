using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[CreateAssetMenu (menuName = "Spells/HealOverTime")]
public class HealingSpell : Spell
{
    public float m_HealAmount = 1.0f;
    public float m_TickDealay = 0.3f;

    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner) {
            Vector3 dir = target == null ? ((Quaternion)spawn_rot * Vector3.right).normalized : (target.transform.position - owner.transform.position).normalized;
            Quaternion rot = GameplayStatics.GetRotationFromDir(dir);
            Vector2 speed = new Vector2(0, 0);

            GameObject obj = SpellUtilities.InstantiateSpell(m_Prefab, owner, target, this, (Vector3)spawn_pos, rot, spell_velocity: speed, tick_delay: m_TickDealay, tag: "SpellUninteractive");

            var sprite = obj.GetComponent<SpriteRenderer>();
            if (sprite)
                sprite.enabled = false;
            obj.GetComponent<SpellPrefabManager>().SetFollowerOwner(FollowWho.Player);
            var status = obj.GetComponent<SpellPrefabManager>().GetOwner().GetComponent<StatusComponent>();

            UnityAction<float, GameplayStatics.DamageType> action = (dmg, type) => Destroy(obj);

            status.AddOnTakeDamage( (dmg, type) => {
                action(dmg, type);
                status.RemoveOnTakeDamage(action);
            });

            return obj;
        }
        return null;
    }

    public override void StopConcentration(GameObject owner = null)
    {
    }

    public override void OnTick(GameObject obj)
    {
        var owner = obj.GetComponent<SpellPrefabManager>().GetOwner();
        if (!owner) {
            Destroy(obj);
            return;
        }
        owner.GetComponent<StatusComponent>().Heal(m_HealAmount);

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
