using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (menuName = "Spells/Bezier Projectile")]
public class BezierProjectile : Spell
{
    public float m_Damage = 10.0f;
    public float m_SpeedMult = 1.0f;
    Vector2 v3_to_v2(Vector3 v) { return new Vector2(v.x, v.y); }

    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner) {
            Vector3 dir = target == null ? ((Quaternion)spawn_rot * Vector3.right).normalized : (target.transform.position - owner.transform.position).normalized;
            Quaternion rot = GameplayStatics.GetRotationFromDir(dir);
            Vector2 speed = new Vector2(0,0);

            GameObject obj = SpellUtilities.InstantiateSpell(m_Prefab, owner, target, this, (Vector3)spawn_pos, rot, spell_velocity:speed, tag:"SpellUninteractive");
            GameObject obj2 = SpellUtilities.InstantiateSpell(m_Prefab, owner, target, this, (Vector3)spawn_pos, rot, spell_velocity:speed, tag:"SpellUninteractive");
            obj2.GetComponent<SpellPrefabManager>().m_Toggle = true;

            obj.GetComponent<SpellPrefabManager>().m_TargetInitialPos = target == null ? v3_to_v2(owner.transform.position + 8 * dir) : v3_to_v2(target.transform.position);
            obj2.GetComponent<SpellPrefabManager>().m_TargetInitialPos = target == null ? v3_to_v2(owner.transform.position + 8 * dir) : v3_to_v2(target.transform.position);

            AudioSource.PlayClipAtPoint(m_SpellSound, owner.transform.position);

            return obj;
        }
        return null;
    }

    void eval_curve(float t, Vector2 p0, Vector2 p1, Vector2 p2, out Vector2 point, out Vector2 derivative)
    {
        Vector2 p11 = (1 - t) * p0 + t * p1;
        Vector2 p21 = (1 - t) * p1 + t * p2;

        point = (1 - t) * p11 + t * p21;

        Vector2 d0 = p0 - p1;
        Vector2 d1 = p1 - p2;
        derivative = 3*((1 - t) * d0 + t * d1);
    }

    public override void StopConcentration(GameObject owner = null)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTick(GameObject obj)
    {
        GameObject owner = obj.GetComponent<SpellPrefabManager>().GetOwner();
        Vector3 target = (Vector3)obj.GetComponent<SpellPrefabManager>().m_TargetInitialPos;
        Vector2 point, derivative;


        Vector3 dir = (obj.GetComponent<SpellPrefabManager>().m_SpawnRot * Vector3.right).normalized;

        Vector3 up_down = obj.GetComponent<SpellPrefabManager>().m_Toggle? new Vector3(1.8f, -2.4f) : new Vector3(1.8f, 2.4f);

        Vector2 p0 = owner.transform.position;
        Vector2 p1 = v3_to_v2((GameplayStatics.GetRotationFromDir(dir)*up_down) + (owner.transform.position));
        Vector2 p2 = target;

        eval_curve(obj.GetComponent<SpellPrefabManager>().m_TimeAlive*m_SpeedMult, p0, p1, p2, out point, out derivative);
        dir = (point - v3_to_v2(obj.transform.position)).normalized;
        obj.transform.position = point;
        obj.transform.rotation = GameplayStatics.GetRotationFromDir(dir);
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
        if (m_OnHitEffect)
            SpellUtilities.SpawnEffectOnCollide(target_obj, s_manager, m_OnHitEffect, SpellUtilities.invalid2);
        SpellUtilities.DamageOnCollide(target_obj, s_manager, m_Damage, SpellUtilities.invalid2);


        if (target_obj != s_manager.GetOwner() && !GameplayStatics.ObjHasTag(target_obj, SpellUtilities.invalid2))
        {
            GameObject.Destroy(src);
        }

        // SpellUtilities.DestroyOnCollide(target_obj, s_manager, SpellUtilities.invalid2, true);
        //if (target_obj != s_manager.GetOwner() && !GameplayStatics.ObjHasTag(target_obj, SpellUtilities.invalid))
        //    GameObject.Destroy(src);
    }

    public override void SpellTriggerTick(Collider2D target, GameObject src)
    {

    }
}
