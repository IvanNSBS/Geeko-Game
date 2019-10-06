using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : EnemyController
{
    [Header("Snake Properties")] 
    public Animator snakeAnimator;

    private bool _dash;


    public override void Holding()
    {
        base.Holding();
        snakeAnimator.SetBool("isHolding",true);
        snakeAnimator.SetBool("isIdle",false);
    }

    public override IEnumerator StopHolding(float timeToGetEnemyPos, float timeAfterGotEnemyPos)
    {
        return base.StopHolding((holdingTime / 4) * 3, holdingTime / 4);
    }

    public override void Dash()
    {
        base.Dash();
        if (!_dash)
        {
            _dash = true;
            StartDashAnimation();
            Debug.Log("te peguei!");
        }
    }

    private void StartDashAnimation()
    {
        snakeAnimator.SetBool("isHolding", false);
        snakeAnimator.SetBool("isDashing", true);
    }

    private void StopDashAnimation()
    {
        snakeAnimator.SetBool("isDashing",false);
    }
    
    public void OnHit()
    {
        snakeAnimator.SetTrigger("isTakingDamage");
    }
    
    public override void ResetDash()
    {
        base.ResetDash();
        Debug.Log("reseting..");
        StopDashAnimation();
    }

    public void OnDeath()
    {
        snakeAnimator.SetBool("isDead",true);
    }
    
    public override void StopMovement()
    {
        base.StopMovement();
        snakeAnimator.SetBool("isMoving",false);
    }
    
    public override void Idle()
    {
        base.Idle();
        snakeAnimator.SetBool("isIdle",true);
        if (_dash)
        {
            _dash = false;
        }
    }

    public override void MoveEnemy(Vector3 dir, float speed)
    {
        base.MoveEnemy(dir, speed);
        snakeAnimator.SetBool("isMoving", true);
        snakeAnimator.SetBool("isIdle", false);
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        if (IsEnemyDashing())
        {
            ResetDash();
        }
        base.OnCollisionEnter2D(other);
    }
}
