using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CyclopsAttack
{
    Stomp,
    Throw,
    Laser,
}


public class CyclopsController : EnemyController
{
    [Header("Cyclops Properties")]
    public Animator cyclopsAnimator;

    public BossState bossState;
    public CyclopsAttack[] attacks;
    public GameObject particle;

    [Header("Stomp Attack")]
    public Transform stompPosition;
    public float timeStompAttack;
    public int numberOfShotsPerWay7W;
    public float amplitude7W;
    public float timeBtwShots7W;
    public float bulletSpeed7W;
    
    [Header("Camera Shake Stomp")]
    public float durationStomp;
    public float strengthStomp;
    public int vibrationStomp;
    public float randomnessStomp;
    public bool fadeOutStomp;

    [Header("Throw Attack")]
    public GameObject[] stones;
    public Transform throwPosition;
    public float throwSpeed;
    public float timeThrowAttack;
    public int numberOfShotsPerDiagonal4D;
    public float timeBtwShots4D;
    public float bulletSpeed4D;


    [Header("Laser Attack")] 
    public LayerMask layerMask;
    public Transform laserEyePosition;
    public Vector2 laserRange;
    public float timeLaserAttack;
    public int laserDamage;
    
    
    private CyclopsAttack _curAttack;
    private bool _attacking;
    private float _rage;
    private bool _stompAttacking;
    private bool _stompIt;
    private float _timeStompAttack;
    private WeaponComponent _weaponComponent;
    private bool _attackingThrow;
    private float _timeThrowAttack;
    private float _timeLaserAttack;
    private bool _attackingLaser;
    private Transform _explosionTransform;
    private TypeOfStone _currStone;
    private bool _laserCharged;
    private bool _afterWander;
    
    /*to-do
    
    Animations
    
    add in the animation frame throw the function throw stone and in the last frame of the laser charge, call chargeLaser()
    
    */
    public override void Start()
    {
        base.Start();
        _weaponComponent = GetComponent<WeaponComponent>();
    }

    public override void CheckTransitions()
    {
        currState = EnemyState.Wander;

        if (getIdle())
        {
            currState = EnemyState.Idle;
        }else if (_afterWander || _attacking)
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

        if (!GetWaiting() && _attacking)
        {
            SetWaiting(true);
            StartCoroutine(WaitingToAttack(idleTime));
        }
        cyclopsAnimator.SetBool("isIdle",true);
    }

    public override IEnumerator RandomlyWanderingIn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        if (currState == EnemyState.Idle)
        {
            setIdle(false);
        }
        SetWandering(false);
        SetWaiting(false);
        _afterWander = true;
    }

    public IEnumerator WaitingToAttack(float sec)
    {
        yield return new WaitForSeconds(sec);
        
        print("waited: "+sec+", minourState: "+currState);

        StompOrNot();

        _afterWander = false;
        SetWaiting(false);
        setIdle(false);
    }

    private void StompOrNot()
    {
        if (!_stompIt)
        {
            _stompIt = true;
            _curAttack = CyclopsAttack.Stomp;
        }
        else
        {
            _attacking = false;
            _stompIt = false;
        }
    }

    public CyclopsAttack ChooseAttack() //modified
    {
        int lottery = 0;
        int _throw = 0;
        int _stomp = 0;
        int _laser = 0;


        switch (bossState)
        {
            case (BossState.Normal):
                _throw = 50;
                _stomp = 100;
                _laser = 0;
                break;
            case (BossState.Enrage):
                _throw = 25;
                _stomp = 50;
                _laser = 100;
                break;
            case (BossState.Rage):
                _throw = 25;
                _stomp = 50;
                _laser = 100;
                break;
        }
        
        lottery = Random.Range(0, 100);

                if (lottery < _throw)
                {
                    return attacks[0];
                }
                else if (lottery < _stomp)
                {
                    return attacks[1];
                }
                else if (lottery < _laser)
                {
                    return attacks[2];
                }
                
                print("Bug in the lottery, number not in the range expected");
                return attacks[lottery];
    }

    public override void Attack()
    {
        if (stateHasChanged)
        {
            StopMovement(); 
        }
        
        if (!_attacking)
        {
            _curAttack = ChooseAttack();
            _attacking = true;
        }
        else
        {
            switch (_curAttack)
            {
                case(CyclopsAttack.Throw):
                    Throw();
                    break;
                case(CyclopsAttack.Stomp):
                    Stomp();
                    break;
                case(CyclopsAttack.Laser):
                    Laser();
                    break;
            }
        }
    }
    
    private void ShootPattern()
    {
        switch (_curAttack)
        {
            case CyclopsAttack.Throw:
                print("shooting pattern throw, the bugs on the table");
                break;
            case CyclopsAttack.Stomp:
                Spread7WayPattern();
                break;
            case CyclopsAttack.Laser:
                HitBoxLaser();
                break;
        }
    }

    public void LaserAnimation()
    {
        cyclopsAnimator.SetBool("Laser",true);
    }
    
    public void TurnOffLaser()
    {
        _laserCharged = false;
        laserEyePosition.gameObject.SetActive(false);
        cyclopsAnimator.SetBool("Laser",false);
        cyclopsAnimator.SetBool("isIdle",true);
    }
    public void ChargeLaser()
    {
        _laserCharged = true;
        laserEyePosition.gameObject.SetActive(true);
    }

    private void HitBoxLaser()
    {
        if (_laserCharged)
        {
            var position = laserEyePosition.position;
            var center = new Vector2(position.x + (laserRange.x / 2), position.y);
            Collider2D hit = Physics2D.OverlapBox(center, laserRange, 0, layerMask);
            if (hit)
            {
                hit.gameObject.GetComponent<StatusComponent>().TakeDamage(laserDamage);
                print("damaged by laser");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var position = laserEyePosition.position;
        var center = new Vector2(position.x + (laserRange.x / 2), position.y);
        Gizmos.DrawWireCube(center, laserRange);
    }

    private void Spread7WayPattern()
    {
        var dir = PlayerDirection();
        _weaponComponent.firePoint = stompPosition;
        _weaponComponent.SpreadSevenWay(dir,numberOfShotsPerWay7W,amplitude7W,timeBtwShots7W,bulletSpeed7W);
        
    }
    
    

    private void Throw()
    {
        if (_timeThrowAttack <= 0)
        {
            if (!_attackingThrow)
            {
                //animations things;
               cyclopsAnimator.SetBool("isThrowing",true);
               cyclopsAnimator.SetBool("isIdle",false);
               // thrown in the cyclop's throw frame 
                //hitbox??
                
                //ShootPattern();

                _attackingThrow = true;
                _timeThrowAttack = timeThrowAttack;
            }
            else
            {
              cyclopsAnimator.SetBool("isThrowing",false);
              cyclopsAnimator.SetBool("isIdle",true);
              _attackingThrow = false;
                setIdle(true);
            }
        }
        else
        {
            //LoopSpiningAnimation();
            _timeThrowAttack -= Time.deltaTime;
        }
    }

    public void CameraShake()
    {
        Camera.main.DOShakePosition(durationStomp,strengthStomp,vibrationStomp,randomnessStomp,fadeOutStomp);
    }

    public void StoneCollision(TypeOfStone stone,Transform transform)
    {
        _currStone = stone;
        _explosionTransform = transform;
        StoneExplosion();
        // ShootPattern();
    }

    public void ThrowStone()
    {
        var stone = Instantiate(ChooseStone(), throwPosition.position, Quaternion.identity);
        var stoneThrown = stone.GetComponent<CyclopsThrow>();
        stoneThrown.ThrowStone(this,PlayerDirection(),throwSpeed);
    }

    private GameObject ChooseStone()
    {
        var random = Random.Range(0, stones.Length);
        return stones[random];
    }

    private void StoneExplosion()
    {
        switch (_currStone)
        {
            case TypeOfStone.Red:
                FourDiagonalsPattern();
                break;
            case TypeOfStone.Black:
                break;
            case TypeOfStone.Green:
                break;
        }
    }

    private void FourDiagonalsPattern()
    {
        var dir = transform.position - _explosionTransform.position;
        _weaponComponent.firePoint = _explosionTransform;
        _weaponComponent.FourDiagonals(dir, numberOfShotsPerDiagonal4D, timeBtwShots4D, bulletSpeed4D);
    }

    private void Stomp()
    {
        if (_timeStompAttack <= 0)
        {
            if (!_stompAttacking)
            {
                cyclopsAnimator.SetBool("isStomping",true);
                cyclopsAnimator.SetBool("isIdle",false);
                
                CameraShake();
                //ShootPattern(); is now being called in the 4th frame of the hittingfloor animation
                
                _stompAttacking = true;
                _timeStompAttack = timeStompAttack;
            }
            else
            {
                cyclopsAnimator.SetBool("isIdle",true);
                cyclopsAnimator.SetBool("isStomping",false);
                _stompAttacking = false;
                setIdle(true);
            }
        }
        else
        {
            _timeStompAttack -= Time.deltaTime;
        }
    }
    
    private void Laser()
    {
        if (_timeLaserAttack <= 0)
        {
            if (!_attackingLaser)
            {
                //animations things;
                cyclopsAnimator.SetTrigger("GuardTheEye");

                _attackingLaser = true;
                _timeLaserAttack = timeLaserAttack;
            }
            else
            {
              //  EndSpinningAnimation();
              TurnOffLaser();
                _attackingLaser = false;
                setIdle(true);
            }
        }
        else
        {
           // LoopSpiningAnimation();
           ShootPattern();
            _timeLaserAttack -= Time.deltaTime;
        }
    }


    public void OnHit()
    {
        UpdateRage();
        cyclopsAnimator.SetTrigger("isTakingDamage");
    }
    
    public void OnDeath()
    {
        cyclopsAnimator.SetBool("isDead",true);
        particle.SetActive(false);
    }
    
    public void IdlingAfterAttack()
    {
        cyclopsAnimator.SetBool("isIdle",true);
    }
    public override void MoveEnemy(Vector3 dir, float speed)
    {
        base.MoveEnemy(dir, speed);
        cyclopsAnimator.SetBool("isMoving",true);
        cyclopsAnimator.SetBool("isIdle", false);
        
    }
    
    public override void StopMovement()
    {
        base.StopMovement();
        cyclopsAnimator.SetBool("isMoving",false);
        cyclopsAnimator.SetBool("isIdle",true);
    }
    
    public void OnFlip()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.localPosition = new Vector3(-child.localPosition.x,child.localPosition.y,child.localPosition.z);
        }
    }
    
    public void UpdateRage()
    {
        var previousMinotaurState = bossState;
        var aux = (GetCurrentHealth() / getMaximumHealth()) * 100;
        _rage = 100 - aux;
        
        if (_rage >= 70)
        {
            bossState = BossState.Rage;
            
        }else if (_rage >= 35)
        {
            bossState = BossState.Enrage;
        }

        if (previousMinotaurState != bossState)
        {
            Debug.Log("Rage in ("+_rage+") Updated to: "+bossState+" mode, with life(%): "+aux);
           /*
            if (bossState == BossState.Rage)
            {
                speed = speed + 0.25f;
                
                idleTime = idleTime - 0.5f;

                var weapon = GetComponent<WeaponComponent>();
                weapon.speed = weapon.speed + 1f;

                particle.SetActive(true);
            }else if (bossState == BossState.Enrage)
            {
                speed = speed + 0.25f;
                idleTime = idleTime - 0.25f;
                
                var weapon = GetComponent<WeaponComponent>();
                weapon.speed = weapon.speed + 2f;
                
            }
            */
            
        }
        
    }
    
}
