using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class ImpController : EnemyController
{
    public Animator impAnimator;

    public override void Iddle()
    {
        base.Iddle();
        impAnimator.SetBool("isIdle",true);
        impAnimator.SetBool("isAttacking",false);
      //  impAnimator.SetBool("isTakingDamage",false);
    }
    
    public override void Attack()
    {
        base.Attack();
        impAnimator.SetBool("isAttacking",true);
        impAnimator.SetBool("isIdle",false);
     //   impAnimator.SetBool("isTakingDamage",false);
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
        impAnimator.SetBool("isAttacking",false);
   //     impAnimator.SetBool("isTakingDamage",false);
    }

    public void onHit()
    {
        impAnimator.SetBool("isTakingDamage",true);
      //  Debug.Log(impAnimator.GetBool("isTakingDamage"));
        impAnimator.SetBool("isIdle",false);
        impAnimator.SetBool("isAttacking",false);
        Invoke("offHit",0.2f);
        
    }

    public void offHit()
    {
        impAnimator.SetBool("isTakingDamage",false);
    }
    
    public override void StopMovement()
    {
        base.StopMovement();
        impAnimator.SetBool("isMoving",false);
    }
}
