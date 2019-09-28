using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FireTotemController : EnemyController
{
    [Header("Animation")]
    [Tooltip("Animation of the enemy")]
    public Animator fireTotemAnimator;
    public int howManyShotsBeforeIdle;
    public int numberOfBullets;
    
    private bool _idleAnimation = false;
    private WeaponComponent _weaponComponent;
    private bool _shooting;
    private bool _attackEnded;
    private int _shots;

    public override void Start()
    {
        base.Start();
        _weaponComponent = GetComponent<WeaponComponent>();
        
        if (howManyShotsBeforeIdle <= 0)
        {
            howManyShotsBeforeIdle = 1;
        }
        
    }
    
    public override void CheckTransitions()
    {
      
        currState = EnemyState.Attack;
      
        if ( getIdle() || _attackEnded)
        {
            currState = EnemyState.Idle;
        }
        else if (IsPlayerInAttackRange(attackRange))
        {
            currState = EnemyState.Attack;
        }
      
        if (GetCurrentHealth() <= 0)
        {
            currState = EnemyState.Die;
        }
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
        
        if (!GetWaiting() && _attackEnded)
        {
            SetWaiting(true);
            StartCoroutine(WaitingToAttack(idleTime));
            fireTotemAnimator.SetBool("isIdle",true);
            fireTotemAnimator.SetBool("isAttacking",false);
        }
    }

    private IEnumerator WaitingToAttack(float time)
    {
        yield return new WaitForSeconds(time);
        SetWaiting(false);
        _attackEnded = false;
        _shots = 0;
        _shooting = false;
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
        if (!_shooting)
        {
            if (_shots >= howManyShotsBeforeIdle)
            {
                _attackEnded = true;
                return null;
            }

            _shots += 1;
            _shooting = true;
            _weaponComponent.Linear(PlayerDirection(),numberOfBullets,_weaponComponent.cooldown,_weaponComponent.speed);
            fireTotemAnimator.SetTrigger("isAttacking");
            fireTotemAnimator.SetBool("isIdle",false);
        }
        
        return null;
    }

    public override void Reload()
    {
        base.Reload();
        if (_shooting)
        {
            _shooting = false;
        }
    }

    public void onHit()
    {
        fireTotemAnimator.SetTrigger("isTakingDamage");
    }
    
    public void OnDeath()
    {
        fireTotemAnimator.SetBool("isDead",true);
    }

}
