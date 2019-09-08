﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class SpellUtilities
{
    public static List<string> invalid  = new List<string> { "Item", "SpellUninteractive", "Untagged" };
    public static List<string> entities = new List<string> { "Wall", "Door" };
    public static void PullTargetToSrc(
        GameObject target, 
        GameObject src, 
        float pull_str, 
        List<GameObject> objs_to_ignore,
        List<string> tags_to_ignore,
        bool negate_ignore = false)
    {
        if (GameplayStatics.ObjHasTag(target, tags_to_ignore, negate_ignore))
            return;
        if (target.GetComponent<Rigidbody2D>() && !GameplayStatics.IsObjInList(target.gameObject, objs_to_ignore))
        {
            Vector3 pos = src.transform.position;
            Vector3 enemy = target.transform.position;
            Vector3 dir = pos - enemy;
            dir.Normalize();
            Vector3 newpos = (dir * 0.001f * pull_str);
            newpos = new Vector3(newpos.x, newpos.y, 0);
            target.transform.position += newpos;
            //target.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //target.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
            //target.GetComponent<Rigidbody2D>().AddForce(dir * m_PullStrength);
        }
    }

    public static bool UpdateSpellTTL(GameObject obj, Spell spell_data)
    {
        SpellPrefabManager manager = obj.GetComponent<SpellPrefabManager>();
        if (manager && spell_data.m_SpellDuration != 0.0f)
        {
            manager.m_TimeToLive -= Time.deltaTime;
            if (manager.m_TimeToLive < 0.0f)
                MonoBehaviour.Destroy(obj);

            return true;
        }
        return false;
    }

    public static GameObject InstantiateSpell(
        GameObject spell_prefab,
        GameObject owner,
        Spell spell,
        Vector3 position,
        Quaternion rotation,
        bool isTrigger = true,
        Vector2 spell_velocity = default(Vector2),
        GameplayStatics.DefaultColliders col = GameplayStatics.DefaultColliders.Polygon, 
        string tag = null)
    {
        GameObject obj = MonoBehaviour.Instantiate(spell_prefab);
        if (tag != null)
            obj.tag = tag;
        obj.GetComponent<SpellPrefabManager>().m_TimeToLive = spell.m_SpellDuration;
        obj.GetComponent<SpellPrefabManager>().SetOwner(owner);
        obj.GetComponent<Rigidbody2D>().velocity = spell_velocity;
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        return obj;
    }

    public static bool DamageOnCollide(
        GameObject target, 
        SpellPrefabManager src, 
        float damage, 
        List<string> ignore = null)
    {
        if (ignore != null)
            if (GameplayStatics.ObjHasTag(target, ignore))
                return false;

        if (src)
            if (target != src.GetOwner()) { 
                GameplayStatics.ApplyDamage(target, damage);
                return true;
            }
        return false;
    }

    public static bool SpawnEffectOnCollide(
        GameObject target, 
        SpellPrefabManager src, 
        GameObject effect, 
        List<string> ignore = null)
    {
        if (ignore != null)
            if (GameplayStatics.ObjHasTag(target, ignore))
                return false;

        if (src)
            if (target != src.GetOwner()) {
                GameObject fx = MonoBehaviour.Instantiate(effect);
                fx.transform.position = GameplayStatics.GetTriggerContactPoint(src.gameObject);
                return true;
            }

        return false;
    }

    public static bool CastSpellOnCollide(
        GameObject target, 
        SpellPrefabManager src, 
        Spell spell, 
        Vector3? spawn_pos = null, 
        List<string> ignore = null)
    {
        if (ignore != null)
            if (GameplayStatics.ObjHasTag(target, ignore))
                return false;

        if (src)
            if (target != src.GetOwner()) { 
                spell.CastSpell(src.GetOwner(), spawn_pos);
                return true;
            }

        return false;
    }
}