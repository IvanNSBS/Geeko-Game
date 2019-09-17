
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using GameAnalyticsSDK.Setup;

public enum MinotaurState
{
    Normal,
    Stressed,
    Rage
}

public enum MinotaurAttack
{
    Poke,
    Axe,
    Spin,
    Dash,
    FloorHit,
}
public class MinotaurController : EnemyController
{
    [Header("Minotaur Properties")]
    public Animator minotaurAnimator;
    public Transform axeAttackPosition;
    public Transform pokePosition;
    public MinotaurState minotaurState = MinotaurState.Normal;
    public MinotaurAttack[] attacks;

    public LayerMask layerMask;
    public float axeAttackRange;
    public Vector2 pokeAttackRange;
    public float timeBtwAxeAttack;
    public float timeBtwPokeAttack;
    public float timeBtwFloorHitAttack;
    public float timeSpinning;
    [Tooltip("time that a function DashChanceLottery will be called, every (x) seconds while rage following.")]
    public float timeToCallDashChanceLottery;
    [Tooltip("Chance of Dash in every x seconds")]
    [Range(0,100)]
    public float chanceToDashInRage;
    public bool allowToDashInSequence;

    [Header("Camera Shake")]
    public float duration;
    public float strength;
    public int vibration;
    public float randomness;
    public bool fadeOut;
    
    private float _rage = 0;
    private bool _attacking=false;
    private MinotaurAttack _curAttack;
    private bool _attackingAxe;
    private bool _attackingPoke;
    private bool _attackingFloorHit;
    private float _timeBtwAxeAttack=0;
    private float _timeBtwPokeAttack=0;
    private float _timeBtwFloorHitAttack;
    private float _timeBtwSpinAttack;
    private bool _waitingIdle;
    private bool _floorHit;
    private bool _spin;
    private bool _dash;
    private bool _changeState;
    private bool _attackingSpin;
    private bool _looping;
    private Coroutine _coroutine;
    private float _timeToCallDashChanceLottery;
    private bool _waitingDashCooldown;
    
    /*
     *To-do after implement the minotaur states:
     * 
     *
     * spin hit box doenst exist for now
     * 
     * Stressed and Rage visual feedback 
     * 
     */

    public override void Start()
    {
        base.Start();
        _timeToCallDashChanceLottery = timeToCallDashChanceLottery;
    }

    public override void CheckTransitions()
    {
        if (IsPlayerInRange(sightRange) && currState != EnemyState.Die)
        {
            currState = EnemyState.Follow;
        }
        
        if ((getIdle()))
        {
            currState = EnemyState.Idle;
        }else if (IsEnemyHoldingToDash())
        {
            currState = EnemyState.Hold;
        }else if (IsEnemyDashing())
        {
            if (!_dash)
            {
                _dash = true;
                StartDashAnimation();
            }
            currState = EnemyState.Dash;
        }else if (IsPlayerInAttackRange(attackRange) || _attacking)
        {
            currState = EnemyState.Attack;
        }
        
        if (GetCurrentHealth() <= 0)
        {
            currState = EnemyState.Die;
        }
        
    }

    private void StartDashAnimation()
    {
        minotaurAnimator.SetBool("isHoldingDash", false);
        minotaurAnimator.SetBool("isDashing", true);
    }

    private void StopDashAnimation()
    {
        minotaurAnimator.SetBool("isDashing",false);
        _dash = false;
    }
    
    public override void OnCollisionEnter2D(Collision2D other)
    {
        if (IsEnemyDashing())
        {
            Camera.main.DOShakePosition(duration,strength,vibration,randomness,fadeOut);
            if (other.collider.CompareTag("Player"))
            {
                other.gameObject.GetComponent<StatusComponent>().TakeDamage(20);
                Debug.Log("Minotaur hit the player while dashing");
            }
            
            if (other.collider.CompareTag("Room") || other.collider.CompareTag("Door") || other.collider.CompareTag("Wall"))
            {
                projectile.SetActive(true);
                var contact = other.GetContact(0);
                var point = Vector2.MoveTowards(contact.point, gameObject.transform.position,0.2f);
                SpiralPattern(point, transform.position,36,0,1);
                Invoke("DeactivateWeapon",0+0.1f);
            } 
            
            ResetDash();
        }
        else
        {
            base.OnCollisionEnter2D(other);
        }
    }

    public override void ResetDash()
    {
        base.ResetDash();
        StopDashAnimation();
    }

    /*
     public override void Holding()
    {
        base.Holding();
        minotaurAnimator.SetBool("isHoldingDash",true);
    }
    */

    public override void Wander()
    {
        currState = EnemyState.Follow;
        ResetFollowingTime();
    }

    public override void Dash()
    {
        base.Dash();
        if (IsEnemyDashing())
        {
            
        }
    }
    
    public override void Follow()
    {
        base.Follow();
        
        if (minotaurState == MinotaurState.Rage)
        {
            if (allowToDashInSequence)
            {
                _waitingDashCooldown = false;
            }
            
            if (!_waitingDashCooldown)
            {
                var time = GetTimeFollowing();
                var threshold = maxTimeFollowing - _timeToCallDashChanceLottery;

                if ( (time <= threshold) && !(time<= 0) )
                {
                    
                    Debug.Log("Sorteando-> time: " + time + ", threshold: "+threshold);
                    _timeToCallDashChanceLottery = _timeToCallDashChanceLottery+ timeToCallDashChanceLottery;
                    if (DashLottery())
                    {
                        StartHoldToDash();
                        minotaurAnimator.SetBool("isHoldingDash",true);
                        _waitingDashCooldown = true;
                        _timeToCallDashChanceLottery = timeToCallDashChanceLottery;
                        
                    }
                    
                }
                else if (time <= 0) //time's up
                {
                    _timeToCallDashChanceLottery = timeToCallDashChanceLottery; //reset local time dashchance
                }
            }
        }
    }

    public bool DashLottery()
    {
        
        var random = Random.Range(0, 100);
        if (random <= chanceToDashInRage-1)
        {
            Debug.Log("dash lottery win");
            return true;
        }
        Debug.Log("dash lottery lose");
        return false;
    }
    
    public MinotaurAttack ChooseAttack() //modified
    {
        int lottery = 0;
        int poke=50;
        int axe=100;
        int spin=0;
        int dash=0;
        
        /*
        switch (minotaurState)
        {
            case(MinotaurState.Normal):
                poke = 50; 
                axe = 100;
                break;
            case(MinotaurState.Stressed):
                poke = 25;
                axe = 50;
                spin = 100;
                break;
            case(MinotaurState.Rage):
                poke = 10;
                axe = 20;
                spin = 50;
                dash = 100;
                break;
        }*/
        
        lottery = Random.Range(0, 100);

        if (lottery < poke)
        {
            return attacks[0];
        } 
        else if (lottery < axe)
        {
            return attacks[1];
        }
        /*
        else if (lottery < spin)
        {
            return attacks[2];
        }else if (lottery < dash)
        {
            return attacks[3];
        }*/
        Debug.Log("Bug in the lottery, number not in the range expected");
        return attacks[lottery];
    }
    
    public override void BasicAttack() //minotaur Attacks
    {
        if (!_attacking)
        {
            _curAttack = ChooseAttack();
            _attacking = true;
        }
        else
        {
            switch (_curAttack)
            {
                case(MinotaurAttack.Poke):
                    Poke();
                    break;
                case(MinotaurAttack.Axe):
                    AxeAttack();
                    break;
                case(MinotaurAttack.FloorHit):
                    FloorHit();
                    break;
                case(MinotaurAttack.Spin):
                    Spin();
                    break;
                case(MinotaurAttack.Dash):
                    Debug.Log("the bug is on the table");
                    StartHoldToDash();
                    break;
            }
        }
    }
    private void AxeAttack()
    {
        if (_timeBtwAxeAttack <= 0)
        {
            //attack
            if (!_attackingAxe)
            {
                minotaurAnimator.SetTrigger("isAttackingAxe");
                minotaurAnimator.SetBool("isIdle",false);
                
                HitBoxAxe();
                _attackingAxe = true;
                _timeBtwAxeAttack = timeBtwAxeAttack;
            }
            else
            {
                _attackingAxe = false;
                setIdle(true); //idling after attack
            }
        }
        else
        {
            _timeBtwAxeAttack -= Time.deltaTime;
        }
    }

    private void HitBoxAxe()
    {
        
        Collider2D hit = Physics2D.OverlapCircle(axeAttackPosition.position, axeAttackRange, layerMask);
        if (hit)
        {
            hit.gameObject.GetComponent<StatusComponent>().TakeDamage(15);
            Debug.Log("damaged by axe");
        }
        /*
        else
        {
            Debug.Log("got nothing in the axe");
        } */
    }

    private void HitBoxPoke()
    {
        Collider2D hit = Physics2D.OverlapBox(pokePosition.position, pokeAttackRange, 0,layerMask);
        if (hit)
        {
            hit.gameObject.GetComponent<StatusComponent>().TakeDamage(10);
            Debug.Log("damaged by poke");
        }/*
        else
        {
            Debug.Log("got nothing in the poke");
        }*/
    }
    
    private void Poke()
    {
        if (_timeBtwPokeAttack <= 0)
        {
            //attack
            if (!_attackingPoke)
            {
                minotaurAnimator.SetTrigger("isAttackingPoke");
                minotaurAnimator.SetBool("isIdle",false);
                
                HitBoxPoke();
                
                _attackingPoke = true;
                _timeBtwPokeAttack = timeBtwPokeAttack;
            }
            else
            {
                _attackingPoke = false;
                setIdle(true); //idling after attack
            }
        }
        else
        {
            _timeBtwPokeAttack -= Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(axeAttackPosition.position,axeAttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pokePosition.position, pokeAttackRange);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(projectile.transform.position, 0.1f);
    }

    private void Spin()
    {
        if (_timeBtwSpinAttack <= 0)
        {
            if (!_attackingSpin)
            {
                //animations things;
                StartSpinningAnimation();
                
                //hitbox??
                
                ShootPattern();

                _attackingSpin = true;
                _timeBtwSpinAttack = timeSpinning;
            }
            else
            {
                EndSpinningAnimation();
                _attackingSpin = false;
                setIdle(true);
            }
        }
        else
        {
            LoopSpiningAnimation();
            _timeBtwSpinAttack -= Time.deltaTime;
        }
    }

    private void LoopSpiningAnimation()
    {
        if (!_looping)
        {
            _looping = true;
            minotaurAnimator.SetBool("SpinLoop",true);
        }
    }
    
    private void EndSpinningAnimation()
    {
        minotaurAnimator.SetBool("SpinLoop",false);
        minotaurAnimator.SetBool("SpinEnd",true);
        _looping = false;
    }

    private void IdleAfterSpinAnimation()
    {
        minotaurAnimator.SetBool("SpinEnd",false);
    }
    
    private void StartSpinningAnimation()
    {
        minotaurAnimator.SetTrigger("isAttackingSpin");
        minotaurAnimator.SetBool("isIdle", false);
    }
    
    private void FloorHit()
    {
        if (_timeBtwFloorHitAttack <= 0)
        {
            if (!_attackingFloorHit)
            {
                minotaurAnimator.SetBool("isHittingFloor",true);
                minotaurAnimator.SetBool("isIdle",false);
                
                //ShootPattern(); is now being called in the 4th frame of the hittingfloor animation
                
                _attackingFloorHit = true;
                _timeBtwFloorHitAttack = timeBtwFloorHitAttack;
            }
            else
            {
                minotaurAnimator.SetBool("isIdle",true);
                _attackingFloorHit = false;
                setIdle(true);
            }
        }
        else
        {
            _timeBtwFloorHitAttack -= Time.deltaTime;
        }
    }

    private void ShootPattern()
    {
        projectile.SetActive(true);
        switch (_curAttack)
        {
            case MinotaurAttack.FloorHit:
                SpiralPattern(projectile.transform.position,pokePosition.position,24,0,1);
                Invoke("DeactivateWeapon",0+0.1f);
                break;
            case MinotaurAttack.Spin:
                SpiralPattern(transform.position,pokePosition.position,24,timeSpinning,1);
                Invoke("DeactivateWeapon",timeSpinning+0.1f);
                break;
            case MinotaurAttack.Dash:
                Debug.Log("SE CHAMADO ALGO TA ERRADO");
                break;
        }
        
    }

    private void DeactivateWeapon()
    {
        projectile.SetActive(false);
        minotaurAnimator.SetBool("isHittingFloor",false);
    }

    private void SpiralPattern(Vector3 origin,Vector3 target,int numberOfShotsPerLoop,float timeToSpiralOnce, int loops)
    {
        var weaponComponent = this.gameObject.GetComponent<WeaponComponent>();
        weaponComponent.firePoint.position = origin;
        var vec3 =  target - origin;
        var vec2 = new Vector2(vec3.x, vec3.y);
        weaponComponent.Spiral(vec2,numberOfShotsPerLoop,timeToSpiralOnce,loops, weaponComponent.speed);
        Debug.Log("Spiral");
    }


    public override void Idle()
    {
        base.Idle();
        
        if (!GetWaiting() && _attacking)
        {
            SetWaiting(true);
            
            if (_coroutine == null)
            {
                _coroutine = StartCoroutine(WaitingToAttack(idleTime));
            }
        }
        minotaurAnimator.SetBool("isIdle",true);

    }

    public IEnumerator WaitingToAttack(float sec)
    {
        yield return new WaitForSeconds(sec);
        
        Debug.Log("waited: "+sec+", minourState: "+currState);

            ChooseHitFloorOrSpin();
            
            SetWaiting(false);
            setIdle(false);
            _coroutine = null;
            _waitingDashCooldown = false;
            _timeToCallDashChanceLottery = timeToCallDashChanceLottery; 
    }

    private void ChooseHitFloorOrSpin()
    {
        switch (minotaurState)
        {
            case MinotaurState.Normal:
                ChooseHitFloor();
                break;
            case MinotaurState.Stressed:
                ChooseSpin();
                break;
            case MinotaurState.Rage:
                ChooseSpin();
                break;
        }
        
    }

    private void ChooseHitFloor()
    {
        if (!_floorHit)
        {
            _floorHit = true;
            _curAttack = MinotaurAttack.FloorHit;
        }
        else
        {
            _attacking = false;
            _floorHit = false;
        }
    }

    private void ChooseSpin()
    {
        
        if (!_spin)
        {
            _spin = true;
            _curAttack = MinotaurAttack.Spin;
        }
        else
        {
            _attacking = false;
            _spin = false;
        }
    }

    public void OnHit()
    {
        UpdateRage();
        minotaurAnimator.SetTrigger("isTakingDamage");
    }

    public void OnDeath()
    {
        minotaurAnimator.SetBool("isDead",true);
    }
    
    public void IdlingAfterAttack()
    {
       // Debug.Log("idling after attack: "+minotaurAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        minotaurAnimator.SetBool("isIdle",true);
    }

    public void OnFlip()
    {
        Debug.Log("Flipped");

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.localPosition = new Vector3(-child.localPosition.x,child.localPosition.y,child.localPosition.z);
        }
    }

    public override void MoveEnemy(Vector3 dir, float speed)
    {
        base.MoveEnemy(dir, speed);
        minotaurAnimator.SetBool("isMoving",true);
        minotaurAnimator.SetBool("isIdle", false);
        
    }

    public override void StopMovement()
    {
        base.StopMovement();
        minotaurAnimator.SetBool("isMoving",false);
        minotaurAnimator.SetBool("isIdle",true);
    }

    public override void Attack()
    {
        if (stateHasChanged)
        {
            StopMovement();  // so the player cant be in moving animation, without shooting when enemystate = atack
        }

        base.Attack();

    }
    
    public void UpdateRage()
    {
        var previousMinotaurState = minotaurState;
        var aux = (GetCurrentHealth() / getMaximumHealth()) * 100;
        _rage = 100 - aux;
        
        if (_rage >= 70)
        {
            minotaurState = MinotaurState.Rage;
            
        }else if (_rage >= 35)
        {
            minotaurState = MinotaurState.Stressed;
        }
        /*
        else
        {
            minotaurState = MinotaurState.Normal;
        }*/

        if (previousMinotaurState != minotaurState)
        {
            Debug.Log("Rage in ("+_rage+") Updated to: "+minotaurState+" mode, with life(%): "+aux);
            if (minotaurState == MinotaurState.Rage)
            {
                speed = speed + 0.25f;
                
                idleTime = idleTime - 0.5f;

                var weapon = GetComponent<WeaponComponent>();
                weapon.speed = weapon.speed + 1f;

            }else if (minotaurState == MinotaurState.Stressed)
            {
                speed = speed + 0.25f;
                idleTime = idleTime - 0.25f;
                
                var weapon = GetComponent<WeaponComponent>();
                weapon.speed = weapon.speed + 2f;
                
            }
            
            
        }
        
    }
}

