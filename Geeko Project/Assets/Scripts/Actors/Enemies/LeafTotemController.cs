using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class LeafTotemController : EnemyController
{
   [Header("Leaf Totem Properties")] 
   public Animator leafTotemAnimator;
   public int howManyShotsBeforeIdle;
   public float hiddenTime;
   
   private WeaponComponent _weaponComponent;
   private Vector2 _shootingDir = new Vector2(1,1);
   private bool _idleAnimation = false;
   private bool _shooting;
   private bool _attackEnded;
   private int _shots=0;
   private bool _hidden;
   private Collider2D _collider;

   [Header("Random Shoot pattern")] 
   public float amplitudeDegrees;
   public int numberOfShots;
   
   
   
   public override void Start()
   {
      base.Start();
      
      _weaponComponent = GetComponent<WeaponComponent>();
      _collider = GetComponent<Collider2D>();
      
      if (howManyShotsBeforeIdle <= 0)
      {
         howManyShotsBeforeIdle = 1;
      }
      
      leafTotemAnimator.SetBool("GoingUp",true);
      leafTotemAnimator.SetBool("Hidden",false);
   }

   
   public override void CheckTransitions()
   {
      
      currState = EnemyState.Attack;
      
      if ( getIdle() || _attackEnded || _hidden)
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
         leafTotemAnimator.SetBool("isIdle",true);
      }
   }
   
   public override void Idle()
   {
      base.Idle();

      if (!GetWaiting() && _attackEnded)
      {
         SetWaiting(true);
         StartCoroutine(WaitingToAttack(idleTime));
         leafTotemAnimator.SetBool("isIdle",true);
         leafTotemAnimator.SetBool("isAttacking",false);
      }else if (!GetWaiting() && _hidden)
      {
         SetWaiting(true);
         StartCoroutine(HiddenWait());
         leafTotemAnimator.SetBool("GoingDown", true);
         leafTotemAnimator.SetBool("isIdle",false);
      }

   }

   private IEnumerator HiddenWait()
   {
      yield return new WaitForSeconds(hiddenTime);
      SetWaiting(false);
      _hidden = false;
      leafTotemAnimator.SetBool("GoingUp",true);
      leafTotemAnimator.SetBool("Hidden",false);
   }

   public void Hidden()
   {
      leafTotemAnimator.SetBool("GoingDown",false);
      leafTotemAnimator.SetBool("Hidden",true);
      if (currState != EnemyState.Die)
      {
         _collider.enabled = false;
      }
   }
   
   private IEnumerator WaitingToAttack(float time)
   {
      yield return new WaitForSeconds(time);
      SetWaiting(false);
      _attackEnded = false;
      _shots = 0;
      _shooting = false;
      _hidden = true;
   }
   
   public void FinishAppear()
   {
      flipStaticEnemy();
      leafTotemAnimator.SetBool("isIdle",true);
      leafTotemAnimator.SetBool("GoingUp",false);
      _collider.enabled = true;
      _idleAnimation = true;
   }

   public override GameObject Shooting()
   {
      if (!_shooting) //shooting
      {
         if (_shots >= howManyShotsBeforeIdle)
         {
            _attackEnded = true;
            return null;
         }
         
         flipStaticEnemy();
         _shooting = true;
         _shots += 1;
         _shootingDir = PlayerDirection();
         _weaponComponent.RandomGauss(_shootingDir,amplitudeDegrees,numberOfShots,_weaponComponent.cooldown,_weaponComponent.speed );
         leafTotemAnimator.SetBool("isAttacking",true);
         leafTotemAnimator.SetBool("isIdle",false);
         
      }
        
      return null;
   }

   public override bool flipStaticEnemy()
   {
      var flipChildren = base.flipStaticEnemy();
      if (flipChildren)
      {
         for (int i = 0; i < transform.childCount; i++)
         {
            var child = transform.GetChild(i);
            child.localPosition = new Vector3(-child.localPosition.x,child.localPosition.y,child.localPosition.z);
         }

         return true;
      }
      else
      {
         return false;
      }
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
      leafTotemAnimator.SetBool("isDead",true);
   }

}
