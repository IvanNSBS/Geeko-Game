using UnityEngine;

[CreateAssetMenu (menuName = "SpellBehavior/On Hit/Apply Damage")]
public class ApplyDamage : OnHitBehavior
{
    public override void OnCollisionHitEvent(Collision2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();

        SpellUtilities.DamageOnCollide(
            collision.gameObject, s_manager, data.m_ApplyDamage.m_Damage, data.m_ApplyDamage.m_ValidHitActors, data.m_ApplyDamage.m_DamageType);
    }

    public override void OnTriggerHitEvent(Collider2D collision, GameObject src, OnCollisionData data)
    {
        GameObject target_obj = collision.gameObject;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();

        SpellUtilities.DamageOnCollide(
            collision.gameObject, s_manager, data.m_ApplyDamage.m_Damage, data.m_ApplyDamage.m_ValidHitActors, data.m_ApplyDamage.m_DamageType);
    }
}