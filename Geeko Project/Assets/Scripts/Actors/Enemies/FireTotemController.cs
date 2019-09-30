using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class FireTotemController : EnemyController
{
    [Header("Animation")]
    [Tooltip("Animation of the enemy")]
    public Animator fireTotemAnimator;
    
    [Header("Linear Pattern")]
    [FormerlySerializedAs("howManyShotsBeforeIdle")]
    public int howManyLinearShotsBeforeIdle;
    public int numberOfBullets;

    [Header("Death 5-way Pattern")] 
    
    public int numberOfBullets5W;
    public float speedBullets5W;
    public float cooldownBullets5W;
    public float amplitude;
    
    private bool _idleAnimation = false;
    private WeaponComponent _weaponComponent;
    private bool _shooting;
    private bool _attackEnded;
    private int _shots;

    public override void Start()
    {
        base.Start();
        _weaponComponent = GetComponent<WeaponComponent>();
        
        if (howManyLinearShotsBeforeIdle <= 0)
        {
            howManyLinearShotsBeforeIdle = 1;
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
            if (_shots >= howManyLinearShotsBeforeIdle)
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

    public override void ExplodeWhenDie()
    {
        var gambiarra = gameObject.AddComponent<WeaponComponent>();
        gambiarra.cooldown = cooldownBullets5W;
        gambiarra.firePoint = transform;
        gambiarra.owner = gameObject;
        gambiarra.speed = speedBullets5W;
        gambiarra.targetTag = "Player";
        gambiarra.bulletPrefab = projectile;
        gambiarra.SpreadFiveWay(PlayerDirection(),numberOfBullets5W,amplitude,_weaponComponent.cooldown,_weaponComponent.speed);
    }

    public void OnDeath()
    {
        fireTotemAnimator.SetBool("isDead",true);
    }

}
