using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "Spells/Black Hole")]
public class BlackHole : Spell
{
    [SerializeField] private float m_PullStrength= 50.0f;
    [SerializeField] private float m_Radius = 5.0f;
    [SerializeField] private Material m_Material;
    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner)
        {
            GameObject obj = Instantiate(m_Prefab);

            obj.GetComponent<SpellPrefabManager>().SetOwner(owner);
            obj.tag = "SpellUninteractive";
            Destroy(obj.GetComponent<BoxCollider2D>());

            obj.AddComponent<CircleCollider2D>();
            obj.GetComponent<CircleCollider2D>().radius = 0.5f;
            obj.GetComponent<CircleCollider2D>().isTrigger = true;
            obj.transform.localScale *= (2*m_Radius);
            if(spawn_pos != null)
                obj.transform.position = (Vector3)spawn_pos;

            // GameplayStatics.AddQuad(obj, m_Material);

            if (m_Material)
                obj.GetComponent<MeshRenderer>().material = m_Material;

            obj.GetComponent<SpellPrefabManager>().m_TimeToLive = m_SpellDuration;
            obj.GetComponent<SpellPrefabManager>().AddTriggerTick(this.SpellTriggerTick);
            obj.GetComponent<SpellPrefabManager>().AddOnUpdate(this.OnTick);
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
        List<GameObject> ignore = new List<GameObject>();
        GameObject owner = src.GetComponent<SpellPrefabManager>().GetOwner();
        if (owner)
            ignore.Add(owner);
        SpellUtilities.PullTargetToSrc(target.gameObject, src, m_PullStrength, ignore, SpellUtilities.invalid);
    }
}
