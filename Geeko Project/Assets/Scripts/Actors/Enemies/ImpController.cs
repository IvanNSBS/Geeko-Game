using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class ImpController : EnemyController
{
    public Animator impAnimator;

    private bool _iddleAnimation = false;
    private bool _attackAnimation = false;

    public override void Iddle()
    {
        base.Iddle();
        impAnimator.SetBool("isIdle",true);
        //impAnimator.SetBool("isAttacking",false);
    }
    
    /*
    public override void Attack()
    {
        base.Attack();
        impAnimator.SetBool("isAttacking",true);
        impAnimator.SetBool("isIdle",false);
    }
         */
    public override GameObject Shooting()
    {
        GameObject aux = base.Shooting();
        if (aux != null) //shooting
        {
            impAnimator.SetTrigger("isAttacking");
            impAnimator.SetBool("isIdle",false);
        }
        
        return aux;
    }

    public void IdlingAfterAttack()
    {
        impAnimator.SetBool("isIdle",true);
    }
    
    public override void Death()
    {
        
    }

    public void OnDeath()
    {
        impAnimator.SetBool("isDead",true);
    }
    
    public override void MoveEnemy(Vector3 dir, float speed)
    {
        base.MoveEnemy(dir,speed);
        impAnimator.SetBool("isMoving",true);
        impAnimator.SetBool("isIdle",false);
       // impAnimator.SetBool("isAttacking",false);
    }

    public void onHit()
    {
        impAnimator.SetTrigger("isTakingDamage");
        impAnimator.SetBool("isIdle",false);
      //  impAnimator.SetBool("isAttacking",false);
        
    }
    
    public override void StopMovement()
    {
        base.StopMovement();
        impAnimator.SetBool("isMoving",false);
    }
}
