using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using DG.Tweening;

public class ImpController : EnemyController
{
    [Header("Animation")]
    [Tooltip("Animation of the enemy")]
    public Animator impAnimator;

    public int howManyPatternsBeforeIdle;
    public int numberOfBullets;
    public float timeDisappear;
    public float degreesPerSecond;

    private WeaponComponent _weaponComponent;
    private int _shots;
    private bool _shooting;
    private bool _attackEnded;
    private bool _shotAnimation;

    private bool _moving;


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


    public override void Idle()
    {
        base.Idle();
        
        if (!GetWaiting() && _attackEnded)
        {
            SetWaiting(true);
            _shots = 0;
            StartCoroutine(WaitingToAttack(idleTime));
            impAnimator.SetBool("isIdle",true);
        }
    }

    public override void Attack()
    {
        if (stateHasChanged)
        {
            StopMovement();  // so the player cant be in moving animation, without shooting when enemystate = atack
        }

        base.Attack();

    }
    
    private IEnumerator WaitingToAttack(float time)
    {
        yield return new WaitForSeconds(time);
        SetWaiting(false);
        _attackEnded = false;
        _shots = 0;
        _shooting = false;
        _moving = true;
    }
    
    public override void Reload()
    {
        base.Reload();
        if(_shooting)
        {
            _shooting = false;
        }
    }
    
    public override GameObject Shooting()
    {
        if (!_shooting)
        {
            if (_shots >= howManyPatternsBeforeIdle)
            {
                _attackEnded = true;
                return null;
            }

            _shots += 1;
            _shooting = true;
            _shotAnimation = true;
            var pd = PlayerDirection();
            _weaponComponent.Linear(pd,numberOfBullets,_weaponComponent.cooldown,_weaponComponent.speed);
            _weaponComponent.SetHomingRotational(GetPlayer(),degreesPerSecond).SetDisappearAfter(timeDisappear);
            impAnimator.SetTrigger("isAttacking");
            impAnimator.SetBool("isMoving",false);
            impAnimator.SetBool("isIdle",false);
        }
        
        return null;
    }

    public void IdlingAfterAttack()
    {
        _shotAnimation = false;
        if (_moving)
        {
            impAnimator.SetBool("isMoving",true);
        }
        else
        {
            impAnimator.SetBool("isIdle", true);
        }
    }
    

    public void OnDeath()
    {
        impAnimator.SetBool("isDead",true);
    }
    
    public override void MoveEnemy(Vector3 dir, float speed)
    {
        base.MoveEnemy(dir,speed);
        _moving = true;
        if (!_shotAnimation)
        {
            impAnimator.SetBool("isMoving",true);
            impAnimator.SetBool("isIdle", false);
        }
        
    }

    public override void Start()
    {
        base.Start();
       
        if (howManyPatternsBeforeIdle <= 0)
        {
            howManyPatternsBeforeIdle = 1;
        }
        _weaponComponent = GetComponent<WeaponComponent>();

    }

    public void onHit()
    {
        impAnimator.SetTrigger("isTakingDamage");
    }
    
    public override void StopMovement()
    {
        base.StopMovement();
        _moving = false;
        impAnimator.SetBool("isMoving",false);
        impAnimator.SetBool("isIdle",true);
    }
}
