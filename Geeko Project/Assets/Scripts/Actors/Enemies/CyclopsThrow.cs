using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum TypeOfStone{
    Red,
    Black,
    Green,
}

public class CyclopsThrow : MonoBehaviour
{
    public TypeOfStone stone;

    private Vector2 _direction;
    private bool _startThrow;
    private MovementComponent _movementComponent;
    private float _speed;
    private CyclopsController _cyclops;
    private Tween _tween;
    private void Start()
    {
        _movementComponent = GetComponent<MovementComponent>();
    }

    public void ThrowStone(CyclopsController cyclops,Vector2 direction, float speed)
    {
        _startThrow = true;
        _direction = direction;
        _speed = speed;
        _cyclops = cyclops;
    }
    void Update()
    {
        if (_startThrow)
        {
            _movementComponent.Move(_direction.x*_speed*Time.deltaTime,_direction.y*_speed*Time.deltaTime);
           transform.Rotate(0,0,1);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Room") || other.collider.CompareTag("Door") || other.collider.CompareTag("Wall") || other.collider.CompareTag("Player"))
        {
            if (other.collider.CompareTag("Player"))
            {
                other.gameObject.GetComponent<StatusComponent>().TakeDamage(20);
                print("Stone hitted the player");
            }
            
            _cyclops.StoneCollision(stone,transform);
            _cyclops.CameraShake();
            
        }
        Destroy(gameObject);
    }
}
