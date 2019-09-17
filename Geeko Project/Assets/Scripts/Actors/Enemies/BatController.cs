using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : EnemyController
{
    [Header("Bat properties")]
    public Animator batAnimator;
    

    public override void CheckTransitions()
    {
        if (IsPlayerInRange(sightRange) && currState != EnemyState.Die)
        {
            currState = EnemyState.Follow;
        }

        if (getIdle())
        {
            currState = EnemyState.Idle;
        }
        
        if (GetCurrentHealth() <= 0)
        {
            currState = EnemyState.Die;
        }
        
    }

    
    
    
    public void OnDeath()
    {
        batAnimator.SetBool("isDead",true);
    }
    
    public override void StopMovement()
    {
        base.StopMovement();
        batAnimator.SetBool("isMoving",false);
    }
    
    public override void Idle()
    {
        base.Idle();
        batAnimator.SetBool("isIdle",true);
    }

    public override void MoveEnemy(Vector3 dir, float speed)
    {
        base.MoveEnemy(dir, speed);
        batAnimator.SetBool("isMoving", true);
        batAnimator.SetBool("isIdle", false);
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        if (other.collider.CompareTag("Player"))
        {
            ResetFollowingTime();
            SetTimeFollowing(0);
        }
    }
}
