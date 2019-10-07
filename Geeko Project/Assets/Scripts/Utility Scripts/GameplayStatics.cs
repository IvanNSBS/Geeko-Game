/*
 File: GameplayStatics.cs
 Contains several GameplayStatic functions and other Utilities functions
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
internal class Timer : MonoBehaviour
{
    private UnityEvent m_OnTick;
    private float m_TickDelay = 0.0f;
    private float m_TimeToLive = 0.0f;

    private float m_RemainingTime = 0.0f;
    private float m_RemainingDelay = 0.0f;

    private bool is_Locked = false;
    private GameObject m_Owner = null;

    public void InitTimer(GameObject owner, float ttl, float delay, UnityAction action)
    {
        m_TimeToLive = ttl;
        m_RemainingTime = ttl;
        m_TickDelay = delay;
        m_RemainingDelay = delay;

        if (m_OnTick == null)
            m_OnTick = new UnityEvent();
        m_OnTick.AddListener(action);
        m_Owner = owner;
    }
    public float GetTotalTime() { return m_TimeToLive; }
    public float GetRemainingTime() { return m_RemainingTime; }

    private void Update()
    {
        if (m_OnTick != null && !is_Locked) {
            m_OnTick.Invoke();
            StartCoroutine( GameplayStatics.Delay(m_TickDelay, () => is_Locked = true, () => is_Locked = false, !is_Locked) );
        }
        m_RemainingTime -= Time.deltaTime;
        if (m_RemainingTime <= 0.0f)
            Destroy(this);
        if (m_Owner == null)
            Destroy(this);
    }
}

public static class GameplayStatics
{
    public static int LayerMap = 1 << 8;
    public static int LayerPlayer = 1 << 9;
    public static int LayerEnemy = 1 << 10;

    public static int idxLayerMap = 8;
    public static int idxLayerPlayer = 9;
    public static int idxLayerEnemy = 10;

    public enum DefaultColliders { Box, Circle, Capsulse, Polygon };
    public enum DamageType { Null, Normal, Fire, Heal, MagicShield }; // null shoulde be used as if it was nullptr
    [System.Serializable]
    public class DamageEvent : UnityEvent<float, DamageType> { }   // Damage Event layout
    public class CollisionEvent : UnityEvent<Collision2D, GameObject> { }   // Collision Event layout
    public class TriggerEvent : UnityEvent<Collider2D, GameObject> { }      // Trigger Event layout
    public class SpellEvent : UnityEvent<GameObject> { }    // SpellEvent Layout. 
                                                            // Only uses the game object that casted spell

    public static void AddTimer(GameObject obj, float ttl, float delay, UnityAction action)
    {
        if (obj)
        {
            Timer timer = obj.AddComponent<Timer>();
            timer.InitTimer(obj, ttl, delay, action);
        }
        else
            Debug.LogWarning("Invalid GameObject to attach timer to");
    }

    public static Color GetDamageColor(DamageType type)
    {
        switch (type)
        {
            case DamageType.Fire:
                return new Color(1.0f, 0.6f, 0.0f, 1.0f);
            case DamageType.Heal:
                return Color.green;
            case DamageType.Normal:
                return Color.red;
            case DamageType.MagicShield:
                return new Color(0, 0.403f, 0.941f, 1.0f);
            case DamageType.Null:
                return new Color(0,0,0,0);
        }
        return new Color(0, 0, 0, 0);
    }

    public static IEnumerator Delay(
        float delay,
        UnityAction call_before,
        UnityAction call_after = null,
        bool condition = true)
    {
        if (condition)
        {
            call_before.Invoke();
            yield return new WaitForSeconds(delay);
            if (call_after != null)
                call_after.Invoke();
        }
    }

    public static bool SpawnDmgPopup(Vector3 position, float damage, DamageType type = DamageType.Normal, bool add_minus = true)
    {
        //var obj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("DmgPopUp/DmgPopUp"));
        var obj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("DmgPopUp/DmgPopupCanvas"));
        var rect = obj.transform.GetChild(0).GetComponent<RectTransform>();

        var point = RectTransformUtility.WorldToScreenPoint(Camera.main, position + new Vector3(Random.Range(-0.3f, 0.3f), 0.15f));
        rect.transform.position = point;
        // rect.transform.position = position + new Vector3(Random.Range(-0.7f, 0.7f), 0.3f);

        var text = obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        string txt = "";
        if (add_minus)
            txt = "-";
        txt += Mathf.FloorToInt(damage).ToString();
        text.text = txt;
        text.color = GetDamageColor(type);
        return true;
    }

    public static bool IsObjInList(GameObject obj, List<GameObject> list)
    {
        foreach (GameObject o in list)
            if (obj == o)
                return true;

        return false;
    }

    public static bool ObjHasTag(GameObject obj, List<string> list, bool negate_ignore = false)
    {
        foreach (string tag in list)
            if (obj.CompareTag(tag)) {
                return !negate_ignore;
            }

        return negate_ignore;
    }

    //TODO: Make more tests to be sure if when it enters trigger it'll
    //      really get the contact point correctly
    public static Vector3 GetTriggerContactPoint(GameObject target, GameObject src, bool debug = false, float debug_duration = 1.0f)
    {
        RaycastHit2D hit;
        Vector3 dir = target.transform.position - src.transform.position;
        float length = dir.magnitude+ 20; //tamanho um pouco maior que a sala
        hit = Physics2D.Raycast(src.transform.position, dir.normalized, length);
        if(debug)
            Debug.DrawLine(src.transform.position, target.transform.position, Color.green, debug_duration);
        return hit.point;
    }

    public static bool ApplyDamage(GameObject src, float amount, DamageType type = DamageType.Normal)
    {
        StatusComponent status = src.GetComponent<StatusComponent>();
        if (status) {
            status.TakeDamage(amount, type);
            return true;
        }
        else
        {
            // Debug.LogWarning("Source GameObj has no Status Component. Can't apply damage to him");
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

    public static Quaternion GetRotationFromDir( Vector2 dir )
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
    public static Quaternion GetRotationFromLookAt(Vector3 from, Vector3 at)
    {
        Vector2 dir = new Vector2(from.x - at.x, from.y - at.y).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }


    public static bool AddDefaultCollider(GameObject obj, DefaultColliders type, bool is_trigger = false)
    {
        switch (type)
        {
            case DefaultColliders.Box:
                BoxCollider2D bcol = obj.AddComponent<BoxCollider2D>();
                if (bcol) { 
                    bcol.isTrigger = is_trigger;
                    return true;
                }
                return false;

            case DefaultColliders.Capsulse:
                CapsuleCollider2D ccol = obj.AddComponent<CapsuleCollider2D>();
                if (ccol){
                    ccol.isTrigger = is_trigger;
                    return true;
                }
                return false;

            case DefaultColliders.Circle:
                CircleCollider2D cicol = obj.AddComponent<CircleCollider2D>();
                if (cicol){
                    cicol.isTrigger = is_trigger;
                    return true;
                }
                return false;
            case DefaultColliders.Polygon:
                PolygonCollider2D poly = obj.AddComponent<PolygonCollider2D>();
                if (poly){
                    poly.isTrigger = is_trigger;
                    poly.autoTiling = true;
                    return true;
                }
                return false;
        }
        return false;
    }
}
