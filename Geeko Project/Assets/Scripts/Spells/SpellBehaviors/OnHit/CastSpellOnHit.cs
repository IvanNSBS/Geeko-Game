using UnityEngine;

[CreateAssetMenu (menuName = "SpellBehavior/On Hit/Cast Spell On Hit")]
public class CastSpellOnHit : OnHitBehavior
{
    public override void OnCollisionHitEvent(Collision2D collision, GameObject src, OnCollisionData data)
    {
        var m_SpellToCast = data.m_CastSpellOnHitData.m_Spell;
        if (!m_SpellToCast)
            return;
        var invalids = data.m_CastSpellOnHitData.m_invalid;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();
        if (!data.m_CastSpellOnHitData.m_EnemyOnly)
            SpellUtilities.CastSpellOnCollide(collision.gameObject, s_manager, m_SpellToCast, GameplayStatics.GetTriggerContactPoint(collision.gameObject, src), invalids);
        else if (collision.gameObject.CompareTag("Enemy"))
            SpellUtilities.CastSpellOnCollide(collision.gameObject, s_manager, m_SpellToCast, GameplayStatics.GetTriggerContactPoint(collision.gameObject, src));
    }

    public override void OnTriggerHitEvent(Collider2D collision, GameObject src, OnCollisionData data)
    {
        var m_SpellToCast = data.m_CastSpellOnHitData.m_Spell;
        if (!m_SpellToCast)
            return;
        var invalids = data.m_CastSpellOnHitData.m_invalid;
        SpellPrefabManager s_manager = src.GetComponent<SpellPrefabManager>();
        if (!data.m_CastSpellOnHitData.m_EnemyOnly)
            SpellUtilities.CastSpellOnCollide(collision.gameObject, s_manager, m_SpellToCast, GameplayStatics.GetTriggerContactPoint(collision.gameObject, src), invalids);
        else if (collision.gameObject.CompareTag("Enemy"))
            SpellUtilities.CastSpellOnCollide(collision.gameObject, s_manager, m_SpellToCast, GameplayStatics.GetTriggerContactPoint(collision.gameObject, src));
    }
}