using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public enum GhostKingMoves
{
    HandAttack,
    JumpAttack,
    BreathAttack,
    Heal,
    SwordAttack,
}

public class GhostKingController : EnemyController
{

    [Header("Ghost King Properties")]
    public Animator ghostKingAnimator;
    public BossState bossState;
    public GameObject particle;
    public GameObject healParticle;
    public float roomCenterOffSet;
    
    [Header("Camera Shake")]
    public float duration;
    public float strength;
    public int vibration;
    public float randomness;
    public bool fadeOut;

    [Header("Hand Attack")]
    public float timeHandAttack;
    public float distanceInvokeFromtheCenter;
    public GameObject[] prefabs;



    [Header("Sword Attack")] 
    public Transform swordPosition;
    public float timeSwordAttack;
    public int numberOfShotsSA;
    public float amplitudeSA;
    public float timeBetweenShotsSA;
    public float bulletSpeedSA;

    [Header("Sword Attack in Rage")] 
    public float timeDisappeared;
    public float delayTime;
    

    [Header("Jump Attack")] 
    public Transform jumpPosition;
    public int howManyJumps;
    public float timeJumpAttack;
    public int numberOfShotsRS;
    public float amplitudeDegressRS;
    public float minimumSpeedRS;
    public float maximumSpeedRS;

    [Header("Breath Attack")] 
    public Transform breathPosition;
    public float timeBreathAttack;
    public int numberOfShotsBA;
    public float timeBetweenShotsBA;
    public float bulletSpeedBA;
    public float brakingTime;
    public float destroyTime;

    [Header("Heal")] 
    public float timeHeal;
    public float healFrame;
    public float healValue;


    private bool _afterWander;
    private Collider2D _collider2D;
    private GameObject _explosionObject;
    private float _rage;
    private GhostKingMoves _currMove;
    private bool _attacking;
    private Vector3 _roomCenter;
    private WeaponComponent _weaponComponent;
    private WeaponComponent _weaponHoming;
    
    private float _timeHandAttack;
    private bool _attackingHand;
    private bool _invokeAllowed;

    private float _timeSwordAttack;
    private bool _attackingSword;
    private bool _runningSword;
    private bool _disappeared;
    private bool _teleportAttack;
    private bool _delay;
    private float _delayTime;

    private float _timeJumpAttack;
    private bool _attackingJump;
    private bool _jumpIt;
    private bool _jumpAllowed;
    private int _howManyJumps;

    private float _timeBreathAttack;
    private bool _attackingBreath;

    private float _timeHeal;
    private bool _healing;
    private bool _recoverLife;
    private int _healCte=0;
    
    
    public override void Start()
    {
        base.Start();
        _roomCenter = FindObjectOfType<DungeonManager>().GetActualRoom().transform.position;
        var wcs = GetComponents<WeaponComponent>();
        _weaponComponent = wcs[0];
        _weaponHoming = wcs[1];
        _collider2D = GetComponent<Collider2D>();
    }
    
    
    public override void CheckTransitions()
    {
        currState = EnemyState.Wander;

        if (getIdle())
        {
            currState = EnemyState.Idle;
        }else if (_afterWander || _attacking || _recoverLife)
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
        ghostKingAnimator.SetBool("isIdle",true);
    }
    
    public IEnumerator WaitingToAttack(float sec)
    {
        yield return new WaitForSeconds(sec);
        
        print("waited: "+sec+", ghostKing: "+currState);

        JumpOrNot();

        _afterWander = false;
        SetWaiting(false);
        setIdle(false);
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
    
    private void JumpOrNot()
    {
        if (!_jumpIt)
        {
            _jumpIt = true;
            _currMove = GhostKingMoves.JumpAttack;
        }
        else
        {
            _attacking = false;
            _jumpIt = false;
        }
    }
    
    public GhostKingMoves ChooseAttack() //modified
    {
        float _sword = 0;
        float _hand = 0;
        float _breath = 0;
        
        var random = Random.Range(0, 100);
        
        switch (bossState)
        {
            case BossState.Normal:
                _sword = 100;
                _hand = 60;
                _breath = 100;
                break;
            case BossState.Enrage:
                _sword = 30;
                _hand = 65;
                _breath = 100;
                break;
            case BossState.Rage:
                 _sword = 50;
                 _hand = 80;
                 _breath = 100;
                break;
        }

        if (random < _sword)
        {
            return GhostKingMoves.SwordAttack;
        }
        else if(random < _hand)
        {
            return GhostKingMoves.HandAttack;
        }else if (random < _breath)
        {
            return GhostKingMoves.BreathAttack;
        }
        else
        {
            return GhostKingMoves.Heal;
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
            _currMove = ChooseAttack();
            _attacking = true;
        }
        else
        {
            switch (_currMove)
            {
                case GhostKingMoves.HandAttack:
                    HandAttack();
                    break;
                case GhostKingMoves.JumpAttack:
                    JumpAttack();
                    break;
                case GhostKingMoves.SwordAttack:
                    SwordAttack();
                    break;
                case GhostKingMoves.BreathAttack:
                    BreathAttack();
                    break;
                case GhostKingMoves.Heal:
                    Heal();
                    break;
            }
        }
    }
    
    private void ShootPattern()
    {
        switch (_currMove)
        {
            case GhostKingMoves.HandAttack:
                print("ERROR!!");
                break;
            case GhostKingMoves.JumpAttack:
                RandomSpeedAllRange();
                break;
            case GhostKingMoves.SwordAttack:
                NineWaySpread();
                break;
            case GhostKingMoves.BreathAttack:
                HomingBullets();
                break;
            case GhostKingMoves.Heal:
                print("ERROR!!");
                break;
        }
    }

    private void NineWaySpread()
    {
        var dir = PlayerDirection(swordPosition.position);

        if (_explosionObject)
        {
            Destroy(_explosionObject);
        }
        
        _explosionObject = new GameObject();
        _explosionObject.transform.position = swordPosition.position;
        _weaponComponent.firePoint = _explosionObject.transform;
        _weaponComponent.SpreadNineWay(dir,numberOfShotsSA,amplitudeSA,timeBetweenShotsSA,bulletSpeedSA);
    }

    private void HomingBullets()
    {
        var dir = PlayerDirection(breathPosition.position);

        if (_explosionObject)
        {
            Destroy(_explosionObject);
        }
        
        _explosionObject = new GameObject();
        _explosionObject.transform.position = breathPosition.position;
        _weaponHoming.firePoint = _explosionObject.transform;
        _weaponHoming.FourDiagonals(dir,numberOfShotsBA,timeBetweenShotsBA,bulletSpeedBA);
        _weaponHoming.SetHomingDirectional(GetPlayer(), brakingTime).SetDisappearAfter(destroyTime);

    }

    private void RandomSpeedAllRange()
    {
        if (_explosionObject)
        {
            Destroy(_explosionObject);
        }
        _explosionObject = new GameObject();
        _explosionObject.transform.position = jumpPosition.position;
        _weaponComponent.firePoint = _explosionObject.transform;
        _weaponComponent.RandomSpeedAndSpread(PlayerDirection(),numberOfShotsRS,amplitudeDegressRS,minimumSpeedRS,maximumSpeedRS);
        CameraShake(duration,strength,vibration,randomness,fadeOut);
        
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

    private void HandAttack()
    {
        if (!_invokeAllowed)
        {
            _invokeAllowed = MoveToCenterRoom(transform, roomCenterOffSet);
        }
        else
        {
            if (_timeHandAttack <= 0)
            {
                if (!_attackingHand)
                {
                    //animations things;
                    ghostKingAnimator.SetBool("isInvoking", true);
                    ghostKingAnimator.SetBool("isIdle", false);
                    
                    // thrown in the cyclop's throw frame 

                    InvokeMonsters();
                    
                    _attackingHand = true;
                    _timeHandAttack = timeHandAttack;
                }
                else
                {
                    ghostKingAnimator.SetBool("isInvoking", false);
                    ghostKingAnimator.SetBool("isIdle", true);
                    _attackingHand = false;
                    _invokeAllowed = false;
                    _recoverLife = true;
                    _currMove = GhostKingMoves.Heal;
                    setIdle(true);
                }
            }
            else
            {
                _timeHandAttack -= Time.deltaTime;
            }
        }

    }

    private void InvokeMonsters() //eu sei que poderia tá melhor
    { 
        var prefabChoosed1 = ChoosePrefab();
        var position = new Vector3(_roomCenter.x+distanceInvokeFromtheCenter,_roomCenter.y,_roomCenter.z);
        Instantiate(prefabChoosed1, position, Quaternion.identity);
        
        var prefabChoosed2 = ChoosePrefab();
        var position2 = new Vector3(_roomCenter.x-distanceInvokeFromtheCenter,_roomCenter.y,_roomCenter.z);
        Instantiate(prefabChoosed2, position2, Quaternion.identity);
    }

    private GameObject ChoosePrefab()
    {
        var size = prefabs.Length;
        var random = Random.Range(0,size);
        return prefabs[random];
    }


    public void CameraShake(float duration, float strength, int vibration, float randomness, bool fadeOut)
    {
        Camera.main.DOShakePosition(duration, strength, vibration, randomness, fadeOut);
    }
    
    private void SwordAttack()
    {
        if (_runningSword || (bossState != BossState.Rage))
        {
            SimpleSwordAttack();
        }
        else
        {
            TeleportSwordAttack();
        }
    }

    private void TeleportSwordAttack()
    {
        if (!_disappeared)
        {
            _disappeared = true;
            StartCoroutine(WaitingToAppearBehindPlayer(timeDisappeared));
        }
        else
        {
            DelayToAttack();
        }

        if (_teleportAttack)
        {
            SimpleSwordAttack();
        }
    }

    private void DelayToAttack()
    {
        if (!_delay && !_teleportAttack)
        {
            _delay = true;
            StartCoroutine(WaitDelay(delayTime));
        }
    }

    private IEnumerator WaitDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _delay = false;
        _teleportAttack = true;
    }

    private IEnumerator WaitingToAppearBehindPlayer(float timeDisappeared)
    {
        print("disappering");
        //Disappear animation things here;
        ghostKingAnimator.SetBool("isIdle",false);
        ghostKingAnimator.SetBool("Disappear",true);

        yield return new WaitForSeconds(timeDisappeared);

        MoveToBehindPlayer();
        
        //appear animation things
        GetSprite().enabled = true;
        ghostKingAnimator.SetBool("Disappear",false);
        ghostKingAnimator.SetBool("Appear",true);
        
    }

    public void Disappear()
    {
        _collider2D.enabled = false;
        GetSprite().enabled = false;
        particle.SetActive(false);
        
        //shadow.SetActive(false);
    }

    public void Appear()
    {
        _collider2D.enabled = true;
        ghostKingAnimator.SetBool("isIdle",true);
        ghostKingAnimator.SetBool("Appear", false);
        particle.SetActive(true);
        //shadow.SetActive(true);
    }
    
    private void MoveToBehindPlayer()
    {
        var p = GetPlayer();
        var playerPosition = p.TransformPoint(p.GetComponent<Collider2D>().offset);
        transform.position = playerPosition;
    }

    private void SimpleSwordAttack()
    {
        if (_timeSwordAttack <= 0)
        {
            if (!_attackingSword)
            {
                ghostKingAnimator.SetBool("isSwordAttacking", true);
                ghostKingAnimator.SetBool("isIdle", false);

                _runningSword = true;
                _attackingSword = true;
                _timeSwordAttack = timeSwordAttack;
            }
            else
            {
                ghostKingAnimator.SetBool("isIdle", true);
                ghostKingAnimator.SetBool("isSwordAttacking", false);
                _attackingSword = false;
                _runningSword = false;
                _teleportAttack = false;
                setIdle(true);
            }
        }
        else
        {
            _timeSwordAttack -= Time.deltaTime;
        }
    }

    private void Heal()
    {
        if (_timeHeal <= 0)
        {
            if (!_healing)
            {
                ghostKingAnimator.SetBool("isIdle",false);
                ghostKingAnimator.SetTrigger("isHealInit");

                healParticle.SetActive(true);
                
                _healing = true;
                _timeHeal = timeHeal;
            }
            else
            {
                ghostKingAnimator.SetBool("isHealLoop",false);
                ghostKingAnimator.SetBool("isHealEnded",true);
                _healing = false;
                setIdle(true);
                _recoverLife = false;
                _healCte = 0;
                healParticle.SetActive(false);
            }
        }
        else
        {
            HealPerSecond();
            _timeHeal -= Time.deltaTime;
        }
    }

    public void HealLoopAnimation()
    {
        ghostKingAnimator.SetBool("isHealLoop",true);
    }

    public void HealFinishAnimation()
    {
        ghostKingAnimator.SetBool("isHealEnded",false);
        ghostKingAnimator.SetBool("isIdle",true);
    }
    
    private void HealPerSecond()
    {
        if(((timeHeal-_timeHeal)/healFrame) > _healCte)
        {
            _healCte++;
            
            GetStatusComponent().Heal(healValue);

        }
    }

    private void JumpAttack()
    {
        
        if (!_jumpAllowed)
        {
            _jumpAllowed = MoveToCenterRoom(jumpPosition, roomCenterOffSet);
        }
        else
        {
            if (_timeJumpAttack <= 0)
            {
                if (!_attackingJump)
                {
                    ghostKingAnimator.SetBool("isJumping",true);
                    ghostKingAnimator.SetBool("isIdle",false);
                
                    //ShootPattern(); // is now being called in the frame of the jump animation
                
                    _attackingJump = true;
                    _timeJumpAttack = timeJumpAttack;
                }
                else
                {
                    _howManyJumps++;

                    ResetJump();

                    RepeatOrNotJump();
                    
                }
            }
            else
            {
                _timeJumpAttack -= Time.deltaTime;
            }
        }
    }

    private void RepeatOrNotJump()
    {
        if (_howManyJumps >= howManyJumps)
        {
            print("ended jumping");
            StopJump();
        }
    }

    public void StopJump()
    {
        _howManyJumps = 0;
        setIdle(true);
    }

    private void ResetJump()
    {
        _attackingJump = false;
        _jumpAllowed = false;
        ghostKingAnimator.SetBool("isIdle",true);
        ghostKingAnimator.SetBool("isJumping",false);
    }


    private bool MoveToCenterRoom(Transform transform, float offSet)
    {
        //int layer = LayerMask.GetMask("Default");
        //Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius, layer);

        var positiveXbound = _roomCenter.x + offSet;
        var negativeXbound = _roomCenter.x - offSet;
        var positiveYbound = _roomCenter.y + offSet;
        var negativeYbound = _roomCenter.y - offSet;

        var current = transform.position;

        if (current.x <= positiveXbound && current.x >= negativeXbound)
        {
            if (current.y <= positiveYbound && current.y >= negativeYbound)
            {
                StopMovement();
                return true;
            }
        }
        
        var dir = DirectionNormalized(transform.position, _roomCenter);
        MoveEnemy(dir, speed);
        return false;
    }
    
    private void BreathAttack() //maybe looping breathing 
    {
        if (_timeBreathAttack <= 0)
        {
            if (!_attackingBreath)
            {
                //animations things;
                ghostKingAnimator.SetBool("Breath",true);
                ghostKingAnimator.SetBool("isIdle",false);

                ShootPattern(); //call in the right frame later!
                
                _attackingBreath = true;
                _timeBreathAttack = timeBreathAttack;
            }
            else
            {
                ghostKingAnimator.SetBool("isIdle",true);
                ghostKingAnimator.SetBool("Breath",false);
                _attackingBreath = false;
                setIdle(true);
            }
        }
        else
        {
            _timeBreathAttack -= Time.deltaTime;
        }
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

    }
    
    public void OnHit()
    {
        UpdateRage();
    }
    public void OnDeath()
    {
        ghostKingAnimator.SetBool("isDead",true);
        particle.SetActive(false);
        healParticle.SetActive(false);
    }
    
    public override void MoveEnemy(Vector3 dir, float speed)
    {
        base.MoveEnemy(dir, speed);
        ghostKingAnimator.SetBool("isMoving",true);
        ghostKingAnimator.SetBool("isIdle", false);
        
    }
    
    public override void StopMovement()
    {
        base.StopMovement();
        ghostKingAnimator.SetBool("isMoving",false);
        ghostKingAnimator.SetBool("isIdle",true);
        
        flipStaticEnemy();
    }
    
    public void IdlingAfterAttack()
    {
        ghostKingAnimator.SetBool("isIdle",true);
        ghostKingAnimator.SetBool("isSwordAttacking",false);
        ghostKingAnimator.SetBool("Breath",false);
        ghostKingAnimator.SetBool("isInvoking",false);
        ghostKingAnimator.SetBool("isJumping",false);
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
