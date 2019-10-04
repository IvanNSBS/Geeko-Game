using UnityEngine;

[CreateAssetMenu(menuName = "SpellBehavior/On Hit/Destroy Self")]
public class DestroySelfEvent : OnHitBehavior
{
    public override void OnCollisionHitEvent(Collision2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();
        if (target_obj != s_manager.GetOwner() && GameplayStatics.ObjHasTag(target_obj, data.m_DestroySelfData.m_ValidHitActors))
            GameObject.Destroy(src);
    }

    public override void OnTriggerHitEvent(Collider2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();
        if (target_obj != s_manager.GetOwner() && GameplayStatics.ObjHasTag(target_obj, data.m_DestroySelfData.m_ValidHitActors))
            GameObject.Destroy(src);
    }
}