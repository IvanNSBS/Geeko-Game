using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "Spells/ManaShield")]
public class ManaShieldConc : Spell
{
    [SerializeField] private float m_ShieldHP = 20.0f;
    [SerializeField] private float m_Radius = 3.0f;
    [SerializeField] private float m_IFrameTime = 1.0f;
    [SerializeField] private Material m_Material;
    [SerializeField] private List<OnStartData> m_OnStartEvents;
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
            Collider2D circle = obj.GetComponent<SpellPrefabManager>().GetOwner().GetComponent<Collider2D>();
            Vector3 pos = circle.bounds.center + new Vector3(0, 0, -15);
            //obj.transform.position = pos;
            // GameplayStatics.AddQuad(obj, m_Material);

            //obj.GetComponent<SpellPrefabManager>().SetFollowerOwner(FollowWho.Player);
            //obj.GetComponent<SpellPrefabManager>().m_FollowOffset = new Vector3(0, 0, -15) + circle.bounds.center - obj.GetComponent<SpellPrefabManager>().GetOwner().transform.position;
            //obj.GetComponent<SpellPrefabManager>().m_FollowSmoothDamp = 0.0f;

            foreach (var startevent in m_OnStartEvents)
                startevent.m_StartBehavior.OnStartEvent(obj, startevent);

            if (m_Material)
                obj.GetComponent<MeshRenderer>().material = m_Material;

            obj.GetComponent<SpellPrefabManager>().m_TimeToLive = m_SpellDuration;
            obj.GetComponent<SpellPrefabManager>().AddTriggerTick(this.SpellTriggerTick);
            obj.GetComponent<SpellPrefabManager>().AddOnUpdate(this.OnTick);

            StatusComponent status = obj.AddComponent<StatusComponent>() as StatusComponent;
            status.SetMaxHealth(m_ShieldHP);
            status.Heal(m_ShieldHP);
            status.m_CanUseIFrames = true;
            status.m_IFrameTime = m_IFrameTime;
            status.m_DamagePopupOverride = GameplayStatics.DamageType.MagicShield;
            status.AddOnDeath( () => Destroy(obj) );

            return obj;

        }
        return null;
    }
    public override void StopConcentration(GameObject spell = null)
    {

    }

    public override void OnTick(GameObject obj)
    {
        //CircleCollider2D circle = obj.GetComponent<SpellPrefabManager>().GetOwner().GetComponent<CircleCollider2D>();
        //Vector3 pos = circle.bounds.center + new Vector3(0,0,-15);
        //obj.transform.position = pos;
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
