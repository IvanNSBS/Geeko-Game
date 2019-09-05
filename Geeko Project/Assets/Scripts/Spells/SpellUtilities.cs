﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class SpellUtilities
{
    public static void PullTargetToSrc(
        GameObject target, 
        GameObject src, 
        float pull_str, 
        List<GameObject> objs_to_ignore,
        List<string> tags_to_ignore)
    {
        if (GameplayStatics.ObjHasTag(target, tags_to_ignore))
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
}