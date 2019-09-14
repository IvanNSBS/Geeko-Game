using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "Spells/Slow Time")]
public class SlowTime : Spell
{
    [SerializeField] private float slow_amount = 0.9f;
    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner)
        {
            GameObject obj = Instantiate(m_Prefab);

            obj.GetComponent<SpellPrefabManager>().SetOwner(owner);
            obj.tag = "SpellUninteractive";
            Destroy(obj.GetComponent<BoxCollider2D>());
            Destroy(obj.GetComponent<MeshFilter>());
            Destroy(obj.GetComponent<MeshRenderer>());

            obj.GetComponent<SpellPrefabManager>().m_TimeToLive = m_SpellDuration*(1-slow_amount);
            obj.GetComponent<SpellPrefabManager>().AddTriggerTick(this.SpellTriggerTick);
            obj.GetComponent<SpellPrefabManager>().AddOnUpdate(this.OnTick);
            obj.GetComponent<SpellPrefabManager>().AddOnDestruction(() => { Time.timeScale = 1; });
            Time.timeScale = 1 - slow_amount;

            return obj;
        }
        return null;
    }
    public override void StopConcentration(GameObject spell = null)
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime *= Time.timeScale;
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
    }
}
