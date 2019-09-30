using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (menuName = "Spells/WindBless")]
public class WindBless : Spell
{
    [SerializeField] float m_SpeedMult = 0.2f;
    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner) {
            Debug.Log("Target = " + target);
            Quaternion rot = Quaternion.identity;
            Vector2 speed = new Vector2(0, 0);

            GameObject obj = SpellUtilities.InstantiateSpell(m_Prefab, owner, target, this, (Vector3)spawn_pos, rot, spell_velocity:speed, tick_delay:0.0f ,tag:"SpellUninteractive");

            var sprite = obj.GetComponent<SpriteRenderer>();
            if (sprite)
                sprite.enabled = false;
            var manager = obj.GetComponent<SpellPrefabManager>();
            manager.SetFollowerOwner(FollowWho.Player);
            manager.GetOwner().GetComponent<EffectManagerComponent>().AddToSpeedMult(m_SpeedMult);

            obj.GetComponent<SpellPrefabManager>().AddOnDestruction( () => {
                obj.GetComponent<SpellPrefabManager>().GetOwner().layer = LayerMask.NameToLayer("Player");
                manager.GetOwner().GetComponent<EffectManagerComponent>().AddToSpeedMult(-m_SpeedMult);
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
        var manager = obj.GetComponent<SpellPrefabManager>();
        manager.GetOwner().layer = LayerMask.NameToLayer("PlayerInvulnerable");

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
