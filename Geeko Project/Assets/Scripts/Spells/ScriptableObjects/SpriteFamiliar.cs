using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (menuName = "Spells/SpriteFamiliar")]
public class SpriteFamiliar : Spell
{
    [SerializeField] float m_TickDelay = 0.7f;
    [SerializeField] Spell m_SpellToCast = null;
    [SerializeField] List<OnStartData> m_StartBehaviors = new List<OnStartData>();
    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner) {
            Debug.Log("Target = " + target);
            Quaternion rot = Quaternion.identity;
            Vector2 speed = new Vector2(0, 0);

            GameObject obj = SpellUtilities.InstantiateSpell(m_Prefab, owner, target, this, (Vector3)spawn_pos, rot, spell_velocity:speed, tick_delay:m_TickDelay ,tag:"SpellUninteractive");

            var sprite = obj.GetComponent<SpriteRenderer>();
            if (sprite)
                sprite.enabled = false;

            foreach(var behavior in m_StartBehaviors)
            {
                behavior.m_StartBehavior.OnStartEvent(obj, behavior);
            }

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
        if (m_SpellToCast && manager.m_Target)
            m_SpellToCast.CastSpell(manager.GetOwner(), manager.m_Target, obj.transform.position);

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
