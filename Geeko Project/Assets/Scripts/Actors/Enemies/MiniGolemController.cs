using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGolemController : EnemyController
{
    public Animator miniGolemController;

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

    /*
    public void lastHitFrame()
    {
        if ((!miniGolemController.GetBool("isMoving"))&&  (getBasicMeleeAttack()))
        {
            miniGolemController.SetBool("isIdle",true);
        }
    }

    /*
    public void notDamaging()
    {
        _damaging = false;
    }

    /*
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            if (!_damaging)
            {
                other.gameObject.GetComponent<StatusComponent>().TakeDamage(10);
                _damaging = true;
                Invoke("notDamaging",1);
            }
        }
    }*/

    public override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        if (!getBasicMeleeAttack() && other.collider.CompareTag("Player"))
        {
            ResetBasicAttack();
        }
    }
}
