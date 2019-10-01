using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (menuName = "Spells/TriggerProjectile")]
public class TriggerProjectile : Spell
{
    public float m_Damage = 10.0f;
    public float m_ProjectileSpeed = 15.0f;
    public float m_Delay = 0.1f;
    [SerializeField] public GameplayStatics.DamageEvent DMG;

    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner) {
            Vector3 dir = target == null ? ((Quaternion)spawn_rot * Vector3.right).normalized : (target.transform.position - owner.transform.position).normalized;
            Quaternion rot = GameplayStatics.GetRotationFromDir(dir);
            Vector2 speed = new Vector2(m_ProjectileSpeed * dir.x, m_ProjectileSpeed * dir.y);

            GameObject obj = SpellUtilities.InstantiateSpell(m_Prefab, owner, target, this, (Vector3)spawn_pos, rot, spell_velocity:speed, tick_delay:m_Delay ,tag:"SpellUninteractive");

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
        if (m_OnHitEffect)
        {
            GameObject fx = MonoBehaviour.Instantiate(m_OnHitEffect);
            fx.transform.position = obj.transform.position + new Vector3(0, 0, -15);

            Vector3 pos = fx.transform.position;
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(pos, 2.0f);

            foreach (Collider2D overlap in overlaps)
            {
                if (overlap.gameObject.CompareTag("Enemy"))
                {
                    StatusComponent st = overlap.GetComponent<StatusComponent>();
                    if (st)
                        st.TakeDamage(m_Damage);
                }
            }
        }
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


        if (target_obj != s_manager.GetOwner() && GameplayStatics.ObjHasTag(target_obj, SpellUtilities.room))
        {
            GameObject.Destroy(src);
        }
    }

    public override void SpellTriggerTick(Collider2D target, GameObject src)
    {
    }
}
