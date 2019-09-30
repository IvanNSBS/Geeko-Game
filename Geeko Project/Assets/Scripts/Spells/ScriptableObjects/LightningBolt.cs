using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEngine.Events;

[CreateAssetMenu (menuName = "Spells/Lightning Bolt")]
public class LightningBolt : Spell
{
    [SerializeField] private float m_Damage = 1350.0f;
    [SerializeField] private Material m_Material;
    [SerializeField] private int m_Iterations = 3;
    public UnityEvent Event;
    public override GameObject CastSpell(GameObject owner, GameObject target = null, Vector3? spawn_pos = null, Quaternion? spawn_rot = null)
    {
        if (m_Prefab && owner)
        {
            Vector3 dir = ((Quaternion)spawn_rot * Vector3.right).normalized;
            Vector3 rot_dir = ((Quaternion)spawn_rot * Vector3.up).normalized;
            Quaternion rot = GameplayStatics.GetRotationFromDir(rot_dir);

            Vector3 target_pos = owner.transform.position + dir * 8.5f;
            Vector3 spawn_position = spawn_pos != null ? (Vector3)spawn_pos : owner.transform.position;
            float distance = (spawn_position - target_pos).magnitude;

            Debug.DrawRay(spawn_position, dir*distance, Color.green, 1.0f);
            RaycastHit2D[] hit = Physics2D.CircleCastAll(spawn_position, 2.0f, dir, distance, layerMask: GameplayStatics.LayerEnemy);
            foreach(RaycastHit2D enemy in hit)
                GameplayStatics.ApplyDamage(enemy.collider.gameObject, m_Damage);
            hit = Physics2D.CircleCastAll(spawn_position, 2.0f, dir, distance, layerMask: LayerMask.GetMask("FlyingEnemy")); ;
            foreach (RaycastHit2D enemy in hit)
                GameplayStatics.ApplyDamage(enemy.collider.gameObject, m_Damage);

            GameObject obj = Instantiate(m_Prefab);
            obj.GetComponent<SpellPrefabManager>().SetOwner(owner);

            obj.tag = "SpellUninteractive";
            Destroy(obj.GetComponent<BoxCollider2D>());
            obj.transform.position = spawn_position + dir * distance / 2;
            obj.transform.rotation = rot;
            obj.transform.localScale = new Vector2(1.5f, distance);

            // GameplayStatics.AddQuad(obj, m_Material);

            if (m_Material)
            {
                obj.GetComponent<MeshRenderer>().material = m_Material;

                obj.GetComponent<SpellPrefabManager>().AddOnUpdate((objt) => {
                    SpellPrefabManager spm = objt.GetComponent<SpellPrefabManager>();
                    objt.GetComponent<MeshRenderer>().material.SetFloat("_Strength", spm.m_TimeToLive - spm.m_TimeAlive);
                });
            }

            obj.GetComponent<SpellPrefabManager>().m_TimeToLive = m_SpellDuration;
            obj.GetComponent<SpellPrefabManager>().AddTriggerTick(this.SpellTriggerTick);
            obj.GetComponent<SpellPrefabManager>().AddOnUpdate(this.OnTick);
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
    }
}
