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

            // GameplayStatics.AddQuad(obj, m_Material);

            if (m_Material)
                obj.GetComponent<MeshRenderer>().material = m_Material;

            obj.GetComponent<SpellPrefabManager>().m_TimeToLive = m_SpellDuration;
            obj.GetComponent<SpellPrefabManager>().AddTriggerTick(this.Pull);
            obj.GetComponent<SpellPrefabManager>().AddOnUpdate(this.OnTick);
        }
    }

    public void Pull(Collider2D target, GameObject src)
    {
        List<GameObject> ignore = new List<GameObject>();
        ignore.Add(m_SpellOwner);

        //TODO: Move to ig_tag to class List
        List<string> ig_tag = new List<string>();
        ig_tag.Add("SpellUninteractive");
        ig_tag.Add("Item");
        SpellUtilities.PullTargetToSrc(target.gameObject, src, m_PullStrength, ignore, ig_tag);
    }

    public override void OnTick(GameObject obj)
    {
        SpellUtilities.UpdateSpellTTL(obj, this);
    }
}
