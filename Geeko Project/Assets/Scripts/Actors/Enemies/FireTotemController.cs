﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FireTotemController : EnemyController
{
    [Header("Animation")]
    [Tooltip("Animation of the enemy")]
    public Animator fireTotemAnimator;

    private bool _idleAnimation = false;


    public override void Start()
    {
        base.Start();
    }
    public override void Wander()
    {
        base.Wander();
        
        if(_idleAnimation)
        {
            fireTotemAnimator.SetBool("isIdle",true);
        }
    }
    public override void Idle()
    {
        base.Idle();
        
        if(_idleAnimation)
        {
            fireTotemAnimator.SetBool("isIdle",true);
        }
    }

    public void FinishAppear()
    {
        fireTotemAnimator.SetBool("isIdle",true);
        _idleAnimation = true;
    }

    public override void Attack()
    {
        base.Attack();
        flipStaticEnemy();
    }

    public override GameObject Shooting()
    {
        GameObject aux = base.Shooting();
        
        if (aux != null) //shooting
        {
            fireTotemAnimator.SetTrigger("isAttacking");
            fireTotemAnimator.SetBool("isIdle",false);
        }
        
        return aux;
    }
    
    
    public void onHit()
    {
        fireTotemAnimator.SetTrigger("isTakingDamage");
    }
    
    public void OnDeath()
    {
        fireTotemAnimator.SetBool("isDead",true);
    }
    
    public void IdlingAfterAttack()
    {  
        fireTotemAnimator.SetBool("isIdle",true);
    }
    
}
