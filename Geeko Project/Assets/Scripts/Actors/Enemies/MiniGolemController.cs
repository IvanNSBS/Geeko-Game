using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGolemController : EnemyController
{
    [Header("MiniGoleam Properties")]
    public Animator miniGolemController;

    [Header("Death Pattern")] 
    public int howManySinWaves;
    public int numberOfBullets;
    public float speedBullets;
    public float cooldownBullets;
    public float amplitude;
    public float waveLength;

    private WeaponComponent _weaponComponent;
    public override void Start()
    {
        base.Start();
        _weaponComponent = GetComponent<WeaponComponent>();
    }

    public override void CheckTransitions()
    {
        if (IsPlayerInRange(sightRange))
        {
            currState = EnemyState.Follow;
        }
        
        else if (!IsPlayerInRange(sightRange))
        {
            currState = EnemyState.Wander;
        }

        if (getIdle())
        {
            currState = EnemyState.Idle;
        }
        else if (IsPlayerInAttackRange(attackRange) || BasicAttacking() ) //something here
        {
            currState = EnemyState.Attack;
        }

        if (GetCurrentHealth() <= 0)
        {
            currState = EnemyState.Die;
        }
    }
    
    public override void Idle()
    {
        base.Idle();
        miniGolemController.SetBool("isIdle",true);
        miniGolemController.SetBool("isMoving",false);
    }
    public override void Attack()
    {
        if (stateHasChanged)
        {
            StopMovement();
        }
        if (!getBasicMeleeAttack())
        {
            miniGolemController.SetBool("isIdle",false);
            miniGolemController.SetTrigger("isAttacking");
        }
        
        base.Attack();
        
    }
    public void IdlingAfterAttack()
    {
        miniGolemController.SetBool("isIdle",true);
        StopMovement();
    }
    public void OnDeath()
    {
        miniGolemController.SetBool("isDead",true);
    }

    public override void ExplodeWhenDie()
    {
        Vector2 _dir = PlayerDirection();
        var angleCte = 360 / howManySinWaves;
        for (int i = 0; i < howManySinWaves; i++)
        {
            _weaponComponent.SineWave(_dir.Rotate(angleCte*i),amplitude,numberOfBullets,waveLength,cooldownBullets,speedBullets);
        }

    }

    public override void MoveEnemy(Vector3 dir, float speed)
    {
        base.MoveEnemy(dir,speed);
        if (!BasicAttacking()) { //if basic attack was used then ...
            miniGolemController.SetBool("isMoving",true);
            miniGolemController.SetBool("isIdle",false); 
        }
    }
    
    public void onHit()
    {
        miniGolemController.SetTrigger("isTakingDamage");
    }
    
    public override void StopMovement()
    {
        base.StopMovement();
        miniGolemController.SetBool("isMoving",false);
        miniGolemController.SetBool("isIdle",true);
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        if (!getBasicMeleeAttack() && other.collider.CompareTag("Player"))
        {
            ResetBasicAttack();
        }
    }
}
