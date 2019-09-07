/*
 File: GameplayStatics.cs
 Contains several GameplayStatic functions and other Utilities functions
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

internal class Timer : MonoBehaviour
{
    private UnityEvent m_OnTick;
    private float m_TimeToLive = 0.0f;
    private float m_RemainingTime = 0.0f;

    public void InitTimer(float ttl, UnityAction action)
    {
        m_TimeToLive = ttl;
        m_RemainingTime = ttl;
        m_OnTick.AddListener(action);
    }
    public float GetTotalTime() { return m_TimeToLive; }
    public float GetRemainingTime() { return m_RemainingTime; }
    private void Update()
    {
        if (m_OnTick != null)
            m_OnTick.Invoke();
        m_RemainingTime -= Time.deltaTime;
        if (m_RemainingTime <= 0.0f)
            Destroy(this);
    }
}
public static class GameplayStatics
{
    public enum DefaultColliders { Box, Circle, Capsulse };
    public class CollisionEvent : UnityEvent<Collision2D, GameObject> { }   // Collision Event layout
    public class TriggerEvent : UnityEvent<Collider2D, GameObject> { }      // Trigger Event layout
    public class SpellEvent : UnityEvent<GameObject> { }    // SpellEvent Layout. 
                                                            // Only uses the game object that casted spell

    public static void AddTimer(GameObject obj, float ttl, UnityAction action)
    {
        if (obj) {
            Timer timer = obj.AddComponent<Timer>();
            timer.InitTimer(ttl, action);
        }
    }
    public static bool IsObjInList(GameObject obj, List<GameObject> list)
    {
        foreach (GameObject o in list)
            if (obj == o)
                return true;

        return false;
    }

    public static bool ObjHasTag(GameObject obj, List<string> list)
    {
        foreach (string tag in list)
            if (obj.CompareTag(tag))
                return true;

        return false;
    }

    //TODO: Make more tests to be sure if when it enters trigger it'll
    //      really get the contact point correctly
    public static Vector3 GetTriggerContactPoint(GameObject src)
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(src.transform.position, src.transform.forward);
        return hit.point;
    }

    public static bool ApplyDamage(GameObject src, float amount)
    {
        StatusComponent status = src.GetComponent<StatusComponent>();
        if (status) {
            status.TakeDamage(amount);
            return true;
        }
        else
        {
            Debug.LogWarning("Source GameObj has no Status Component. Can't apply damage to him");
            return false;
        }
    }

    public static bool AddQuad(GameObject obj, Material mat = null)
    {
        MeshFilter quad = obj.AddComponent<MeshFilter>();
        if (quad)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            if (go)
            {
                quad.mesh = go.GetComponent<MeshFilter>().mesh;
                quad.gameObject.AddComponent<MeshRenderer>();

                if (mat)
                    quad.GetComponent<MeshRenderer>().material = mat;

                GameObject.Destroy(go);
                return true;
            }
            else {
                Debug.LogWarning("Failed to instantiate Quad Primitive");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("Couldn't Add Quad to Selected GameObject");
            return false;
        }
    }

    public static bool AddSprite(GameObject obj, Sprite spr = null, RuntimeAnimatorController anim = null)
    {
        SpriteRenderer sprite = obj.AddComponent<SpriteRenderer>();
        if (sprite)
        {
            if (spr)
                sprite.sprite = spr;

            Animator animator = obj.AddComponent<Animator>();
            if (animator)
                if (anim) animator.runtimeAnimatorController = anim;

            else Debug.LogError("Failed to instantiate Animator");

            return true;
        }
        Debug.LogError("Couldn't add SpriteRenderer to Selected GameObject");
        return false;
    }

    public static bool AddDefaultCollider(GameObject obj, DefaultColliders type, bool is_trigger = false)
    {
        switch (type)
        {
            case DefaultColliders.Box:
                BoxCollider2D bcol = obj.AddComponent<BoxCollider2D>();
                if (bcol)
                    return true;
                bcol.isTrigger = is_trigger;
                return false;

            case DefaultColliders.Capsulse:
                CapsuleCollider2D ccol = obj.AddComponent<CapsuleCollider2D>();
                if (ccol)
                    return true;
                ccol.isTrigger = is_trigger;
                return false;

            case DefaultColliders.Circle:
                CircleCollider2D cicol = obj.AddComponent<CircleCollider2D>();
                if (cicol)
                    return true;
                cicol.isTrigger = is_trigger;
                return false;
        }
        return false;
    }
}
