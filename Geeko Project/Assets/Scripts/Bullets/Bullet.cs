using System;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public string targetTag;

    public float maxSpeed;
    public bool ignoreRock;
    private string _instantiatorTag;
    private GameObject _instantiator;

    private Transform _homingTarget;
    private bool _homingRotational = false;
    private bool _homingDirectional = false;
    
    private float _rotationalHomingMaxDegrees;
    
    private float _directionalHomingMaxForce;

    private bool _sine = false;
    private float _sineStartingAngle;
    private float _sineAmplitude;
    private float _sineStartingTime;
    private float _sinePeriod;
    private Vector2 _sinePrincipalVelocity;
    private Vector2 _sinePerpendicularUnit;

    private bool _willDisappear = false;
    private float _disappearIn;

    public void SetInstantiator(GameObject instantiator)
    {
        _instantiator = instantiator;
        _instantiatorTag = instantiator.tag;
    }

    public GameObject GetInstantiator()
    {
        return _instantiator;
    }

    private static readonly List<string> TagsToIgnore = new List<string>() { "Untagged", "Room", "SpellUninteractive", "Item", "Wall", "Door" };

    private bool ShouldCollide(Collider2D other)
    {
        var gameObj = other.gameObject;
        var rockCollide = !ignoreRock && (gameObj).CompareTag("Rock");
        var shouldNotIgnore = (rockCollide || GameplayStatics.ObjHasTag(gameObj, TagsToIgnore, true));
        var ignoreInstantiator = shouldNotIgnore && !other.gameObject.CompareTag(_instantiatorTag);
        return ignoreInstantiator;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpellInteractive")) {
            var owner = other.gameObject.GetComponent<SpellPrefabManager>().GetOwner();
            if (owner == GetInstantiator())
                return;
        }

        if (ShouldCollide(other))
        {
            GameplayStatics.ApplyDamage(other.gameObject, 10);
            Destroy(gameObject);
        }

        //if (targetTag.Equals("All"))
        //{
        //if (other.CompareTag("Player")|| other.CompareTag("Enemy") || other.CompareTag("DestructibleObject") || (other.CompareTag("SpellInteractive") && !GetInstantiator().CompareTag("Player")))
        //{
        //    other.gameObject.GetComponent<StatusComponent>().TakeDamage(10);
        //    Destroy(gameObject);
        //} else if (other.CompareTag("Wall") || other.CompareTag("Door") || other.CompareTag("Rock"))
        //{
        //    Destroy(gameObject);
        //}
        //} 
        //else // if (other.CompareTag("SpellInteractive")  || other.CompareTag(targetTag) || other.CompareTag("Wall") || other.CompareTag("Door") || other.CompareTag("DestructibleObject") || other.CompareTag("Rock"))
        //{
        //    List<string> tags_to_ignore = new List<string>() { "Untagged", "Room", "" };
        //    if (other.CompareTag(targetTag) || other.CompareTag("DestructibleObject") || (other.CompareTag("SpellInteractive") && !GetInstantiator().CompareTag("Player"))) 
        //    {
        //        other.gameObject.GetComponent<StatusComponent>().TakeDamage(10);
        //    } 
        //    Destroy(gameObject);
        //}
    }

    public void HomeRotational(Transform target, float degreesPerSecond)
    {
        _homingTarget = target;
        _rotationalHomingMaxDegrees = degreesPerSecond * Time.fixedDeltaTime;;
        _homingRotational = true;
    }

    public void HomeDirectional(Transform target, float acceleration)
    {
        _homingDirectional = true;
        _homingTarget = target;
        _directionalHomingMaxForce = acceleration * Time.fixedDeltaTime;
    }

    public void Sine(float amplitude, float period, bool flip)
    {
        _sine = true;
        _sineAmplitude = amplitude;
        _sinePeriod = period;
        _sineStartingAngle = flip? 270 * Mathf.Deg2Rad : 90 * Mathf.Deg2Rad;
        _sinePrincipalVelocity = rb.velocity;
        
        _sineStartingTime = Time.time;
        _sinePerpendicularUnit = rb.velocity.Rotate(90).normalized;
    }

    public float DisappearIn
    {
        get => _disappearIn;
        set
        {
            _willDisappear = true;
            _disappearIn = value;
        }
    }

    private Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }

    private void HomeRotational()
    {
        if (!_homingRotational) return;
        
        var direction = (Vector2)_homingTarget.position - rb.position;
        var angle = Vector2.SignedAngle(rb.velocity, direction);
        if (Math.Abs(angle) > _rotationalHomingMaxDegrees)
        {
            angle = Math.Sign(angle) * _rotationalHomingMaxDegrees;
        }
        rb.velocity = rb.velocity.Rotate(angle);
    }

    private void HomeDirectional()
    {
        if (!_homingDirectional) return;
        
        var direction = (Vector2)_homingTarget.position - rb.position;
        var desiredSpeed = direction.normalized * maxSpeed;
        var desiredForce = desiredSpeed - rb.velocity;
        
        var actualForce = desiredForce;
        if (desiredForce.magnitude > _directionalHomingMaxForce)
        {
            actualForce = desiredForce.normalized * _directionalHomingMaxForce;
        }

        rb.velocity += actualForce;
    }

    private void Sine()
    {
        if (!_sine) return;

        var positionInCycle = ((Time.time - _sineStartingTime) % _sinePeriod) / _sinePeriod;
        var angle = (positionInCycle) * 2 * Mathf.PI;
        var multiplier = Mathf.Sin(_sineStartingAngle + angle) * _sineAmplitude;
        rb.velocity = _sinePrincipalVelocity + _sinePerpendicularUnit * multiplier;
    }

    private void Disappear()
    {
        if (!_willDisappear) return;

        if (_disappearIn <= 0)
        {
            Destroy(gameObject);
            return;
        }
        
        _disappearIn -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        HomeRotational();
        HomeDirectional();
        Sine();
        Disappear();
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
