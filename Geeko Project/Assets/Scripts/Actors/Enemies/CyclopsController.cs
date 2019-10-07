using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
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
    
    [Range(1,100)]
    public float chanceToThrowNormalState;
    [Range(1,100)]
    public float chanceToThrowEnrageState;
    [Range(1,100)]
    public float chanceToThrowRageState;

    [Header("Stomp Attack")]
    public Transform stompPosition;
    public float timeStompAttack;
    public int numberOfShotsPerWay7W;
    public float amplitude7W;
    public float timeBtwShots7W;
    public float bulletSpeed7W;
    public float stompRadius;
    
    [Header("Camera Shake Stomp")]
    public float durationStomp;
    public float strengthStomp;
    public int vibrationStomp;
    public float randomnessStomp;
    public bool fadeOutStomp;

    [Header("Throw Attack")]
    public GameObject[] stones;
    public Transform throwPosition;
    public float wallOffSetExplosion;
    public float throwRadius;
    public float throwSpeed;
    public float timeThrowAttack;
    public bool useNewStonePatterns;
    
    [Header("Red Stone Explosion")]
    public int numberOfShotsPerDiagonalRS;
    public float amplitudeRS;
    public float timeBtwShotsRS;
    public float bulletSpeedRS;

    [Header("Purple Stone Explosion")] 
    public int numberOfShotsPerWayPS;
    public float amplitudePS;
    public float timeBetweenShotsPS;
    public float bulletSpeedPS;

    [Header("Grey Stone Explosion")] 
    public int howManySinWaves;
    public int numberOfShotsGS;
    public float amplitudeGS;
    public float waveLengthGS;
    public float timeBetweenShotsGS;
    public float bulletSpeedGS;

    [Header("Camera Shake Throw")]
    public float durationThrow;
    public float strengthThrow;
    public int vibrationThrow;
    public float randomnessThrow;
    public bool fadeOutThrow;

    [Header("Laser Attack")] 
    public LayerMask layerMask;
    public Transform laserEyePosition;
    public Vector2 laserRange;
    public float timeLaserAttack;
    public int laserDamage;
    public float timeFrame;
    public float anglePerFrame;
    public float angleAfterHitted;
    public int howManyFramesSlowedAfterHit;
    
    
    private CyclopsAttack _curAttack;
    private bool _attacking;
    private float _rage;
    private bool _stompAttacking;
    private bool _stompIt;
    private float _timeStompAttack;
    private WeaponComponent _weaponComponentRed;
    private WeaponComponent _weaponComponentPurple;
    private bool _attackingThrow;
    private float _timeThrowAttack;
    private float _timeLaserAttack;
    private bool _attackingLaser;
    private GameObject _explosionObject;
    private TypeOfStone _currStone;
    private bool _laserCharged;
    private bool _afterWander;
    private bool _throwRangeAllowed;
    private bool _stompRangeAllowed;
    private LineRenderer _lineRenderer;
    private int _laserDirectionX=1;
    private Animator _chargeLaserAnimation;
    private float _angle;
    private int _timeCte=0;
    private Vector2 _dir=Vector2.right;
    private Vector3 _roomCenter;
    private int _laserCount;
    private int _laserRepeat;
    private bool _hitted = false;
    private int _waitHit = 0;
    private GameObject _choosedStone;
    
    /*to-do
    
    parameters adjust
    more stones prefabs to explode
    adjust stomp pattern
    
    */
    public override void Start()
    {
        base.Start();
        var wcs = GetComponents<WeaponComponent>();
        _weaponComponentRed = wcs[0];
        _weaponComponentPurple = wcs[1];
        _lineRenderer = laserEyePosition.GetComponent<LineRenderer>();
        _chargeLaserAnimation = laserEyePosition.GetComponent<Animator>();
        try
        {
            _roomCenter = FindObjectOfType<DungeonManager>().GetActualRoom().transform.position;
            print("[try] rooms position in the cyclop: "+_roomCenter);
        }
        catch
        {
            _roomCenter = Vector3.zero;
            print("[catch] rooms position in the cyclops: "+_roomCenter);
        }
        
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
        
        print("waited: "+sec+", cyclopsState: "+currState);

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
        float _throw = 0;
        //var _laser = 0;
        var random = Random.Range(0, 100);
        switch (bossState)
        {
            case BossState.Normal:
                _throw = chanceToThrowNormalState-1;
                break;
            case BossState.Enrage:
                _throw = chanceToThrowEnrageState-1;
                break;
            case BossState.Rage:
                _throw = chanceToThrowRageState-1;
                break;
        }

        if (random < _throw)
        {
            return CyclopsAttack.Throw;
        }
        else
        {
            return CyclopsAttack.Laser;
        }
        
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

    public void LaserLoopAnimation()
    {
        cyclopsAnimator.SetBool("Laser", false);
        cyclopsAnimator.SetBool("laserLoop",true);

    }
    
    public void LaserAnimation()
    {
        cyclopsAnimator.SetBool("Laser",true);
        flipStaticEnemy();
    }

    public void SavePlayerPosition()
    {
        _dir = PlayerDirection(laserEyePosition.position);
    }

    public void ChargeLaserAnimation()
    {
        _chargeLaserAnimation.SetBool("isChargingLaser",true);
    }
    
    public void ResetLaser()
    {
        print(_timeCte);
        _hitted = false;
        _waitHit = 0;
        _laserCharged = false;
        _lineRenderer.enabled = false;
        _timeCte = 0;
        _attackingLaser = false;
        cyclopsAnimator.SetBool("laserLoop",false);
        cyclopsAnimator.SetBool("isIdle",true);
        _chargeLaserAnimation.SetBool("isChargingLaser",false);
    }
    public void ChargeLaser()
    {
        _laserCharged = true;
        _angle = 0;
        flipStaticEnemy();
        LaserLoopAnimation();
    }

    private void HitBoxLaser()
    {
        // 1 coord y equivale a 18 angle.
        // x ----- 9 angle 
        if (_laserCharged)
        {
            //var angleLine = _angle/_angleCte; 
            var position = laserEyePosition.position;
            var positionTop = new Vector3(position.x,position.y+(laserRange.y/2),0);
            var positionBottom = new Vector3(position.x,position.y-(laserRange.y/2),0);
            
            _dir = _dir.Rotate(_angle);

            RaycastHit2D centerLineHit = Physics2D.Raycast(position, _dir, laserRange.x, layerMask); 
            RaycastHit2D topLineHit = Physics2D.Raycast(positionTop, _dir, laserRange.x, layerMask); 
            RaycastHit2D bottomLineHit = Physics2D.Raycast(positionBottom, _dir, laserRange.x, layerMask);

            if (centerLineHit)
            {
                centerLineHit.collider.GetComponent<StatusComponent>().TakeDamage(laserDamage);
                _hitted = true;
            }else if (topLineHit)
            {
                topLineHit.collider.GetComponent<StatusComponent>().TakeDamage(laserDamage);
                _hitted = true;
            }else if (bottomLineHit)
            {
                bottomLineHit.collider.GetComponent<StatusComponent>().TakeDamage(laserDamage);
                _hitted = true;
            }
            
            Vector3[] positions = new[] {new Vector3(0,0,0), new Vector3((laserRange.x*_dir.x), (_dir.y)*laserRange.x, 0f)};
            _lineRenderer.SetPositions(positions);
            _lineRenderer.enabled = true;
            
            if (((timeLaserAttack-_timeLaserAttack)/timeFrame) > _timeCte )
            {
                _timeCte++;
                var playerDir = PlayerDirection(position);
                var angle = Vector2.SignedAngle(new Vector2(playerDir.x,playerDir.y), _dir);
                var actualAngle = Mathf.Min(Mathf.Abs(angle),anglePerFrame);
                
                if ((angle < 0))
                {
                    if (!_hitted && _waitHit <_timeCte)
                    {
                        _angle = actualAngle;
                    }
                    
                    if (_hitted && _waitHit < _timeCte)
                    {
                        _angle = angleAfterHitted;
                        _waitHit = _timeCte+howManyFramesSlowedAfterHit;
                    }else if (_hitted)
                    {
                        _angle = angleAfterHitted;
                    }
                }else if ((angle > 0))
                {
                    if (!_hitted && _waitHit <_timeCte)
                    {
                         _angle = -actualAngle;
                    }
                    
                    if (_hitted && _waitHit < _timeCte)
                    {
                        _angle = -angleAfterHitted;
                        _waitHit = _timeCte+howManyFramesSlowedAfterHit;
                    } else if (_hitted)
                    {
                        _angle = -angleAfterHitted;
                    }
                }
                else
                {
                    
                    if (_hitted && _waitHit < _timeCte)
                    {
                        _waitHit = _timeCte+howManyFramesSlowedAfterHit;
                    }
                    
                    _angle = 0;
                }
                
                
                if (_waitHit <= _timeCte && _hitted)
                {
                    _hitted = false;
                    print("finish it");
                }

                
                flipStaticEnemy(_dir);

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(throwPosition.position,throwRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(stompPosition.position,stompRadius);
    }

    private void Spread7WayPattern()
    {
        var dir = PlayerDirection(stompPosition.position);
        if (_explosionObject)
        {
            Destroy(_explosionObject);
        }
        _explosionObject = new GameObject();
        _explosionObject.transform.position = stompPosition.position;
        _weaponComponentRed.firePoint = _explosionObject.transform;
        _weaponComponentRed.SpreadSevenWay(dir,numberOfShotsPerWay7W,amplitude7W,timeBtwShots7W,bulletSpeed7W);
        CameraShake();
        
    }
    
    public override bool flipStaticEnemy()
    {
        var flipChildren = base.flipStaticEnemy();
        return FlipChildrenIf(flipChildren);
    }

    private bool FlipChildrenIf(bool flipChildren)
    {
        if (flipChildren)
        {
            FlipChildren();
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool flipStaticEnemy(Vector3 dir)
    {
        var flipChildren = base.flipStaticEnemy(dir);
        return FlipChildrenIf(flipChildren);
    }

    private void Throw()
    {
        if (!_throwRangeAllowed)
        {
            _throwRangeAllowed = DontColliderWithWall(throwPosition, throwRadius);
        }
        else
        {
            if (_timeThrowAttack <= 0)
            {
                if (!_attackingThrow)
                {
                    _choosedStone = ChooseStone();
                    ThrowAnimation(_choosedStone);
                   // thrown in the cyclop's throw frame 

                   _attackingThrow = true;
                    _timeThrowAttack = timeThrowAttack;
                }
                else
                {
                  cyclopsAnimator.SetBool("isThrowing",false);
                  cyclopsAnimator.SetBool("isIdle",true);
                  _attackingThrow = false;
                  _throwRangeAllowed = false;
                    setIdle(true);
                }
            }
            else
            {
                _timeThrowAttack -= Time.deltaTime;
            }
        }
    }

    private void ThrowAnimation(GameObject choosedStone)
    {
        var stone = choosedStone.GetComponent<CyclopsThrow>().stone;
        cyclopsAnimator.SetBool("isThrowing",true);
        cyclopsAnimator.SetBool("isIdle",false);
        switch (stone)
        {
            case TypeOfStone.Red:
                cyclopsAnimator.SetTrigger("Red");
                break;
            case TypeOfStone.Purple:
                cyclopsAnimator.SetTrigger("Purple");
                break;
            case TypeOfStone.Grey:
                cyclopsAnimator.SetTrigger("Grey");
                break;
        }
    }

    public void CameraShake()
    {
        switch (_curAttack)
        {
            case CyclopsAttack.Stomp:
                Camera.main.DOShakePosition(durationStomp, strengthStomp, vibrationStomp, randomnessStomp,
                fadeOutStomp);
                break;
            case CyclopsAttack.Throw:
                Camera.main.DOShakePosition(durationThrow, strengthThrow, vibrationThrow, randomnessThrow,
                    fadeOutThrow);
                break;
            case CyclopsAttack.Laser:
                print("camera shake in laser state???");
                break;
        }
        
        print("camera shake :"+_curAttack);
    }

    public void StoneCollision(TypeOfStone stone,Vector3 position)
    {
        _currStone = stone;
        if (_explosionObject)
        {
            Destroy(_explosionObject);
        }
        _explosionObject = new GameObject();
        _explosionObject.transform.position = position;
        StoneExplosion();
        // ShootPattern();
    }

    public void ThrowStone()
    {
        var tp = throwPosition.position;
        var stone = Instantiate(_choosedStone, tp, Quaternion.identity);
        var stoneThrown = stone.GetComponent<CyclopsThrow>();
        stoneThrown.ThrowStone(this,PlayerDirection(tp),throwSpeed);
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
                if (useNewStonePatterns)
                {
                    ThreeWay();
                }
                else
                {
                    FourDiagonals();
                }
                print("red");
                break;
            case TypeOfStone.Purple:
                if (useNewStonePatterns)
                {
                    FourWayPurple();
                }
                else
                {
                    FourDiagonals();
                }
                print("purple");
                break;
            case TypeOfStone.Grey:
                if (useNewStonePatterns)
                {
                    ThreeWaySineWave();
                }
                else
                {
                    FourDiagonals();
                }
                print("grey");
                break;
        }
    }

    private void ThreeWaySineWave()
    {
        var point = Vector2.MoveTowards(_explosionObject.transform.position, _roomCenter, wallOffSetExplosion*2);
        _explosionObject.transform.position = point;
        Vector2 dir = DirectionNormalized(point,_roomCenter);
        _weaponComponentRed.firePoint = _explosionObject.transform;
        var angleCte = 150 / howManySinWaves;
        dir = dir.Rotate(-angleCte);
        for(int i=0; i<howManySinWaves;i++)
        {
            _weaponComponentRed.SineWave(dir.Rotate(angleCte*i),amplitudeGS,numberOfShotsGS,waveLengthGS,timeBetweenShotsGS,bulletSpeedGS);
        }
      
    }

    private void FourWayPurple()
    {
        var point = Vector2.MoveTowards(_explosionObject.transform.position, _roomCenter, wallOffSetExplosion*2);
        _explosionObject.transform.position = point;
        var dir = DirectionNormalized(point,_roomCenter);
        _weaponComponentPurple.firePoint = _explosionObject.transform;
        _weaponComponentPurple.SpreadSevenWay(dir,numberOfShotsPerWayPS, amplitudePS, timeBetweenShotsPS, bulletSpeedPS);
    }

    private void ThreeWay()
    {
        var point = Vector2.MoveTowards(_explosionObject.transform.position, _roomCenter, wallOffSetExplosion*2);
        _explosionObject.transform.position = point;
        var dir = DirectionNormalized(point,_roomCenter);
        _weaponComponentRed.firePoint = _explosionObject.transform;
        _weaponComponentRed.SpreadThreeWay(dir, numberOfShotsPerDiagonalRS, amplitudeRS,timeBtwShotsRS, bulletSpeedRS);
    }
    
    private void FourDiagonals()
    {
        var point = Vector2.MoveTowards(_explosionObject.transform.position, _roomCenter, wallOffSetExplosion);
        _explosionObject.transform.position = point;
        var dir = PlayerDirection(point);
        _weaponComponentRed.firePoint = _explosionObject.transform;
        _weaponComponentRed.FourDiagonals(dir, numberOfShotsPerDiagonalRS,timeBtwShotsRS, bulletSpeedRS);
    }

    private void Stomp()
    {
        if (!_stompRangeAllowed)
        {
            _stompRangeAllowed = DontColliderWithWall(stompPosition, stompRadius);
        }
        else
        {
            if (_timeStompAttack <= 0)
            {
                if (!_stompAttacking)
                {
                    cyclopsAnimator.SetBool("isStomping",true);
                    cyclopsAnimator.SetBool("isIdle",false);
                
                    //ShootPattern(); is now being called in the 4th frame of the hittingfloor animation
                
                    _stompAttacking = true;
                    _timeStompAttack = timeStompAttack;
                }
                else
                {
                    cyclopsAnimator.SetBool("isIdle",true);
                    cyclopsAnimator.SetBool("isStomping",false);
                    _stompAttacking = false;
                    _stompRangeAllowed = false;
                    setIdle(true);
                }
            }
            else
            {
                _timeStompAttack -= Time.deltaTime;
            }
        }
    }

    private bool DontColliderWithWall(Transform firepoint, float radius)
    {
        int layer = LayerMask.GetMask("Default");
        Collider2D[] hit = Physics2D.OverlapCircleAll(firepoint.position, radius, layer);
        if (hit.Length > 1)
        {
            var dir = DirectionNormalized(firepoint.position, _roomCenter);
            MoveEnemy(dir, speed);
            return false;
        }
        else
        {
            StopMovement();
            return true;
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
                _laserCount++;
                ResetLaser();
                RepeatLaserOrTurnOff();
            }
        }
        else
        {
           ShootPattern();
            _timeLaserAttack -= Time.deltaTime;
        }
    }

    private void RepeatLaserOrTurnOff()
    {
        _laserRepeat = LaserRepeat();
        if (_laserCount >= _laserRepeat)
        {
            print("Turned off laser");
            TurnOffLaser();
        }
        
    }

    private int LaserRepeat()
    {
        switch (bossState)
        {
            case BossState.Normal:
                return 1;
            case BossState.Enrage:
                return 2;
            case BossState.Rage:
                return 3;
        }

        return 0;
    }

    private void TurnOffLaser()
    {
        _laserCount = 0;
        setIdle(true);
    }


    public void OnHit()
    {
        UpdateRage();
        //cyclopsAnimator.SetTrigger("isTakingDamage");
    }
    
    public void OnDeath()
    {
        cyclopsAnimator.SetBool("isDead",true);
        particle.SetActive(false);
        laserEyePosition.gameObject.SetActive(false);
    }
    
    public void IdlingAfterAttack()
    {
        cyclopsAnimator.SetBool("isIdle",true);
        cyclopsAnimator.SetBool("isStomping",false);
        cyclopsAnimator.SetBool("isThrowing",false);
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
        
        flipStaticEnemy();
    }
    
    public void OnFlip()
    {
        FlipChildren();
    }

    private void FlipChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.localPosition = new Vector3(-child.localPosition.x, child.localPosition.y, child.localPosition.z);
        }

        _laserDirectionX = -_laserDirectionX;
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
           
            if (bossState == BossState.Rage)
            {
                speed = speed + 0.25f;
                
                idleTime = idleTime - 0.5f;

               // var weapon = GetComponent<WeaponComponent>();
               // weapon.speed = weapon.speed + 1f;

                particle.SetActive(true);
            }else if (bossState == BossState.Enrage)
            {
                speed = speed + 0.25f;
                idleTime = idleTime - 0.25f;
                
               // var weapon = GetComponent<WeaponComponent>();
              //  weapon.speed = weapon.speed + 2f;
                
            }
            
            
        }
        
    }
    
}
