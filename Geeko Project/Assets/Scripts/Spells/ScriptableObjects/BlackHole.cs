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
    public override void ApplyDamage(GameObject obj)
    {
        throw new System.NotImplementedException();
    }

    public override void CastSpell()
    {
        if (m_Prefab && m_SpellOwner)
        {
            Vector3 from = Input.mousePosition;
            from = Camera.main.ScreenToWorldPoint(from);
            GameObject obj = Instantiate(m_Prefab);
            obj.transform.position = new Vector3(from.x, from.y, 3.0f);


            obj.GetComponent<SpellPrefabManager>().SetOwner(m_SpellOwner);
            obj.tag = "SpellUninteractive";
            Destroy(obj.GetComponent<BoxCollider2D>());

            obj.AddComponent<CircleCollider2D>();
            obj.GetComponent<CircleCollider2D>().radius = 0.5f;
            obj.GetComponent<CircleCollider2D>().isTrigger = true;
            obj.transform.localScale *= (2*m_Radius);
            if (m_Material)
                obj.GetComponent<MeshRenderer>().material = m_Material;

            obj.GetComponent<SpellPrefabManager>().m_TimeToLive = m_SpellDuration;
            obj.GetComponent<SpellPrefabManager>().AddTriggerTick(this.Pull);
            obj.GetComponent<SpellPrefabManager>().AddOnUpdate(this.OnTick);
        }
    }

    public void Pull(GameObject target, GameObject src)
    {
        if (target.CompareTag("SpellUninteractive"))
            return;
        if (target.GetComponent<Rigidbody2D>() && target.gameObject != m_SpellOwner)
        {
            Vector3 pos = src.transform.position;
            Vector3 enemy = target.transform.position;
            Vector3 dir = pos - enemy;
            dir.Normalize();
            Vector3 newpos = (dir * 0.001f * m_PullStrength);
            newpos = new Vector3(newpos.x, newpos.y, 0);
            target.transform.position += newpos;
            //target.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //target.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
            //target.GetComponent<Rigidbody2D>().AddForce(dir * m_PullStrength);
        }
    }

    public override void OnTick(GameObject obj)
    {
        SpellPrefabManager manager = obj.GetComponent<SpellPrefabManager>();
        if (manager && m_SpellDuration != 0.0f)
        {
            manager.m_TimeToLive -= Time.deltaTime;
            if (manager.m_TimeToLive < 0.0f)
                Destroy(obj);
        }
    }

    public override void Initialize(GameObject obj)
    {
        throw new System.NotImplementedException();
    }
}
