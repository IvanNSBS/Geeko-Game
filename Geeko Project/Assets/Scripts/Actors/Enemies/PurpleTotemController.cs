using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleTotemController : EnemyController
{
   [Header("Purple Totem Properties")] 
   public Animator purpleTotemAnimator;
   public int circlesShotsBeforeIdle;

   private WeaponComponent _weaponComponent;
   private bool _idleAnimation = false;
   private Vector2 _shootingDir = new Vector2(1,1);
   private bool _shooting;
   private bool _attackEnded;
   private int _shots=0;

   public override void Start()
   {
      base.Start();
      _weaponComponent = GetComponent<WeaponComponent>();
      if (circlesShotsBeforeIdle <= 0)
      {
         print("circlesShots <=0, circles will be 1");
         circlesShotsBeforeIdle = 1;
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
         purpleTotemAnimator.SetBool("isIdle",true);
      }
   }
   
   public override void Idle()
   {
      base.Idle();

      if (!GetWaiting() && _attackEnded)
      {
         SetWaiting(true);
         StartCoroutine(waitingToAttack(idleTime));
         purpleTotemAnimator.SetBool("isIdle",true);
         purpleTotemAnimator.SetBool("isAttacking",false);
      }
      
   }

   private IEnumerator waitingToAttack(float time)
   {
      yield return new WaitForSeconds(time);
      SetWaiting(false);
      _attackEnded = false;
      _shots = 0;
      _shooting = false;
   }
   
   public void FinishAppear()
   {
      purpleTotemAnimator.SetBool("isIdle",true);
      _idleAnimation = true;
   }
   
   public override void Attack()
   {
      base.Attack();
      flipStaticEnemy();
   }
   
   public override GameObject Shooting()
   {
      if (!_shooting) //shooting
      {
         _shooting = true;
         _shots += 1;
         _shootingDir = Quaternion.Euler(0, 0, 27.5f) * _shootingDir;
         _weaponComponent.Spiral(_shootingDir,8,0,1,_weaponComponent.speed );
         purpleTotemAnimator.SetBool("isAttacking",true);
         purpleTotemAnimator.SetBool("isIdle",false);
         if (_shots >= circlesShotsBeforeIdle)
         {
            _attackEnded = true;
         }
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

   public void OnDeath()
   {
      purpleTotemAnimator.SetBool("isDead",true);
   }
   
   public void IdlingAfterAttack()
   {  
      purpleTotemAnimator.SetBool("isIdle",true);
      purpleTotemAnimator.SetBool("isAttacking",false);
   } 
   
}
