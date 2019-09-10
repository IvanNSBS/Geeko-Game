using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGolemController : EnemyController
{
    public Animator miniGolemController;
    private bool _damaging;
    public override void Iddle()
    {
        base.Iddle();
        miniGolemController.SetBool("isIdle",true);
    }
    public override void Attack()
    {
        if (stateHasChanged)
        {
            StopMovement();
        }
        if (!getBasicMeleeAttack())
        {
            miniGolemController.SetTrigger("isAttacking");
            miniGolemController.SetBool("isIdle",false);
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
        if (getBasicMeleeAttack()) { //if basic attack was used then ...
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
    }

    public void lastHitFrame()
    {
        if ((!miniGolemController.GetBool("isMoving"))&&  (getBasicMeleeAttack()))
        {
            miniGolemController.SetBool("isIdle",true);
        }
    }

    public void notDamaging()
    {
        _damaging = false;
    }

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
    }
}
