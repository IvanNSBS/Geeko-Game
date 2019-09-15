using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "Spells/ManaShield")]
public class ManaShieldConc : Spell
{
    [SerializeField] private float m_ShieldHP = 20.0f;
    [SerializeField] private float m_Radius = 3.0f;
    [SerializeField] private float m_DmgMitigation = 0.7f;
    [SerializeField] private float m_SlowAmount = 1.0f;
    [SerializeField] private Material m_Material;
    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner)
        {
            GameObject obj = Instantiate(m_Prefab);

            obj.GetComponent<SpellPrefabManager>().SetOwner(owner);
            obj.tag = "SpellInteractive";
            Destroy(obj.GetComponent<BoxCollider2D>());

            obj.AddComponent<CircleCollider2D>();
            obj.GetComponent<CircleCollider2D>().radius = 0.5f;
            obj.GetComponent<CircleCollider2D>().isTrigger = true;
            obj.transform.localScale *= (2*m_Radius);
            CircleCollider2D circle = obj.GetComponent<SpellPrefabManager>().GetOwner().GetComponent<CircleCollider2D>();
            Vector3 pos = circle.bounds.center + new Vector3(0, 0, -15);
            obj.transform.position = pos;
            // GameplayStatics.AddQuad(obj, m_Material);

            if (m_Material)
                obj.GetComponent<MeshRenderer>().material = m_Material;

            if (owner.GetComponent<EffectManagerComponent>())
            {
                Debug.Log("Setting it");
                owner.GetComponent<EffectManagerComponent>().AddToSpeedMult(-m_SlowAmount);
            }

            obj.GetComponent<SpellPrefabManager>().m_TimeToLive = m_SpellDuration;
            obj.GetComponent<SpellPrefabManager>().AddTriggerTick(this.SpellTriggerTick);
            obj.GetComponent<SpellPrefabManager>().AddOnUpdate(this.OnTick);
            obj.GetComponent<SpellPrefabManager>().AddOnDestruction( () => { owner.GetComponent<EffectManagerComponent>().AddToSpeedMult(m_SlowAmount); });

            StatusComponent status = obj.AddComponent<StatusComponent>() as StatusComponent;
            status.SetMaxHealth(m_ShieldHP);
            status.Heal(m_ShieldHP);
            // TODO: Set can use iframe
            status.AddOnTakeDamage( x => { if ( status.GetCurrentHealth() <= 0.0f) owner.GetComponent<StatusComponent>().TakeDamage(x * m_DmgMitigation); } );
            //status.AddOnDeath( status.Killed );

            return obj;

        }
        return null;
    }
    public override void StopConcentration(GameObject spell = null)
    {
        if (spell) {
            spell.GetComponent<SpellPrefabManager>().GetOwner().GetComponent<EffectManagerComponent>().AddToSpeedMult(m_SlowAmount);
            GameObject.Destroy(spell);
        }
    }

    public override void OnTick(GameObject obj)
    {
        CircleCollider2D circle = obj.GetComponent<SpellPrefabManager>().GetOwner().GetComponent<CircleCollider2D>();
        Vector3 pos = circle.bounds.center + new Vector3(0,0,-15);
        obj.transform.position = pos;
        SpellUtilities.UpdateSpellTTL(obj, this);
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
