using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using GameAnalyticsSDK.Setup;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public string targetTag;

    private GameObject _instantiator;

    private float _maxDegrees;
    private Transform _homingTarget;
    private bool _homing = false;

    public void SetInstantiator(GameObject instantiator)
    {
        _instantiator = instantiator;
    }

    public GameObject GetInstantiator()
    {
        return _instantiator;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (targetTag.Equals("All"))
        {
            if (other.CompareTag("Player")|| other.CompareTag("Enemy") || other.CompareTag("DestructibleObject"))
            {
                other.gameObject.GetComponent<StatusComponent>().TakeDamage(10);
                Destroy(gameObject);
            } else if (other.CompareTag("Wall") || other.CompareTag("Door"))
            {
                Destroy(gameObject);
            }

        } else if (other.CompareTag(targetTag) || other.CompareTag("Wall") || other.CompareTag("Door") || other.CompareTag("DestructibleObject"))
        {
            if (other.CompareTag(targetTag) || other.CompareTag("DestructibleObject")) 
            {
                other.gameObject.GetComponent<StatusComponent>().TakeDamage(10);
            } 
            Destroy(gameObject);
        }
    }

    public void Home(Transform target, float degreesPerSecond)
    {
        _homingTarget = target;
        _maxDegrees = degreesPerSecond * Time.fixedDeltaTime;;
        _homing = true;
    }

    private float _lastUpdate;

    private void Start()
    {
        _lastUpdate = Time.time;
    }
    
    private Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }

    private void FixedUpdate()
    {
        if (_homing)
        {
            var direction = (Vector2)_homingTarget.position - rb.position;
            var angle = Vector2.SignedAngle(rb.velocity, direction);
            if (Math.Abs(angle) > _maxDegrees)
            {
                angle = Math.Sign(angle) * _maxDegrees;
            }
            rb.velocity = rb.velocity.Rotate(angle);
        }
    }
}
 
public static class Vector2Extension {
     
    public static Vector2 Rotate(this Vector2 v, float degrees) {
        var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
        var tx = v.x;
        var ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}
