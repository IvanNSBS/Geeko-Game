using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;


public enum EnemyState{

    Wander,
    
    Follow,
    
    Die,
    
    Attack,
    
    Retreat,
    
    Dash,
    
    Hold,
    
    Idle,
};

public enum EnemyType
{
    Melee,
    Ranged,
};

public class EnemyController : MonoBehaviour
{
    [Header("Enemy State")]
    [Tooltip("The enemy current state")]
    public EnemyState currState = EnemyState.Wander;

    [NonSerialized]
    public EnemyState previousState = EnemyState.Wander;
    [NonSerialized]
    public bool stateHasChanged = false;
    
    [Header("Enemy Type Attack")]
    [Tooltip("Choose one type of attack, be careful to choose the attributes accordingly")]
    public EnemyType enemyType;
    
    [Header("Bullet Prefab")]
    [Tooltip("If the enemy is ranged, choose the bullet prefab")]
    public GameObject projectile;

    [Header("Enemy Attributes")] [Tooltip("How long the enemy will start his actions when it spawns")]
    public float timeToStartActions;
    [Tooltip("The range that the enemy sees the opponent")]
    public float sightRange; 
    [Tooltip("The range of the attack of the enemy that triggers its attack")]
    public float attackRange;
    [Tooltip("The speed of the monster movement")]
    public float speed;
    [Tooltip("The speed of the Dash movement, if it dashes")]
    public float dashSpeed;
   
    [Header("Features")]
    [Tooltip("The enemy stop shooting to reload for sometime")]
    public bool stopShootToReload;
    [Tooltip("The enemy explode when dies and can call a function to handle this")]
    public bool explodeWhenDie;
    [Tooltip("The enemy shoots while moves")]
    public bool moveWhileShoot;
    [Tooltip("The enemy dashes in the direction of the player every (x) seconds")]
    public bool enemyMeleeDash;
    
    [Header("Type of Walk (when wandering or shooting)")]
    public bool zigZagWalkHorizontal;
    public bool zigZagWalkVertical;
    public bool horizontalWalk;
    public bool verticalWalk;
    public bool randomWalk;
    public bool followWalk;
    
    [Header("Time Attributes (seconds)")]
    [Tooltip("Time that the enemy will be idling to change state")]
    public float idleTime;
    [Tooltip("Time that the enemy will be wandering to change state")]
    public float wanderingTime;
    [Tooltip("Max time that the enemy will be chasing the player")]
    public float maxTimeFollowing;
    [Tooltip("Time that the enemy will be dashing")]
    public float dashTime;
    [Tooltip("Time that the enemy will be basic Attacking")]
    public float basicMeleeAttackTime;
    [Tooltip("Time of the cd of the basic attack melee")]
    public float cooldownBasicMeleeAttackTime;
    [Tooltip("Time that the enemy will be holding to dash")]
    public float holdingTime;
    [Tooltip("Time that the enemy will spend reloading to be able to shoot again")]
    public float timeToReload;
    [Tooltip("Time that the enemy spend shooting before reload")]
    public float timeAmmo;
    [Tooltip("Time between the enemy bullets")]
    public float timeBtwShots;
    [Tooltip("Time that the enemy walk in a direction when moving and shooting")]
    public float timeWalkingOneDirection;

    
    
    /*
    [Header("Deprecated ->Retreat <-")]
    public float stoppingDistance;
    public float retreatDistance;
    */
    
    
    //public float coolDown;
    
    
    private bool _coolDownAttack = false;
    private float _timeBtwShots;
    private bool _wandering;
    private Vector3 _randomDir;
    private float _dashTime;
    private bool _holding = false;
    private bool _dashing = false;
    private bool _waiting = false;
    private bool _idle= false;
    private bool _basicMeleeAttack = false;
    private bool _waitingHold;
    private SpriteRenderer _sprite;
    
    private Transform _player;
    private Vector3 _lastPlayerPosition;
    private Vector3 _lastEnemyPosition;
    
    private MovementComponent _movementComponent;
    private StatusComponent _statusComponent;
    
    private bool _dashed = false;
    private bool _reloading = false;
    private bool _outOfAmmo = false;
    private float _timeWalking = 0;
    private bool _zigZagHorizontal=false;
    private bool _zigZagVertical = false;
    private bool _dead=false;
    private float _basicMeleeAttackTime=0;
    private float _timeFollowing=0;
    private bool _followingTimeIsOver;
    private bool _saveLastPos;
    private bool _stopShootToReload;
    private bool _firstIdle;

    /* TO-DO
    BOSS
    Walks and shootings
    */
    private void Awake()
    {
        _movementComponent = GetComponent<MovementComponent>();
        _statusComponent = GetComponent<StatusComponent>();
    }

    public virtual void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
      //  projectile.transform.localScale = Vector3.one / 2;
        _dashTime = dashTime;
        _timeBtwShots = timeBtwShots;
        _stopShootToReload = stopShootToReload;
        this.gameObject.layer = GameplayStatics.idxLayerEnemy;
        ResetFollowingTime();
        _firstIdle = true;
    }

    public SpriteRenderer GetSprite()
    {
        return _sprite;
    }
    
    public virtual void Update()
    {
        StateMachine();

        previousState = currState;
        stateHasChanged = false;
        
        CheckTransitions();

        if (previousState != currState)
        {
            stateHasChanged = true;
        }
        
    }

    public virtual void StateMachine()
    {
        switch (currState)
        {
            case (EnemyState.Wander):
                Wander();
                break;
            case (EnemyState.Follow):
                Follow();
                break;
            case (EnemyState.Die):
                Death();
                break;
            case (EnemyState.Dash):
                Dash();
                break;
            case (EnemyState.Hold):
                Holding();
                break;
            case (EnemyState.Attack):
                Attack();
                break;
            case (EnemyState.Retreat):
                Retreat();
                break;
            case (EnemyState.Idle):
                Idle();
                break;
        }
    }

    public virtual void CheckTransitions() // the order can change the preference about the state.
                                            // Becareful.
    {
        if (IsPlayerInRange(sightRange) && currState != EnemyState.Die)
        {
            currState = EnemyState.Follow;
        }
        
        else if (!IsPlayerInRange(sightRange) && currState != EnemyState.Die)
        {
            currState = EnemyState.Wander;
        }

        if ((_idle))
        {
            currState = EnemyState.Idle;
        }
        else if (IsEnemyHoldingToDash())
        {
            currState = EnemyState.Hold;
        }
        else if (_dashing)
        {
            currState = EnemyState.Dash;
        }
        else if (IsPlayerInAttackRange(attackRange))
        {
            currState = EnemyState.Attack;
        }

        /*
        if (IsTimeToRetreat(retreatDistance))
        {
            currState = EnemyState.Retreat;
        }
        */
        if (GetCurrentHealth() <= 0)
        {
            currState = EnemyState.Die;
        }
    }

    public float GetCurrentHealth()
    {
        return _statusComponent.GetCurrentHealth();
    }

    public void setIdle(bool idle)
    {
        _idle = idle;
    }

    public bool getIdle()
    {
        return _idle;
    }
    
    public virtual void StopMovement()
    {
        _movementComponent.StopMovement();
    }

    public IEnumerator FirstIdle()
    {
        yield return new WaitForSeconds(timeToStartActions);
        _firstIdle = false;
        _waiting = false;
        _idle = false;
    }
    public virtual void Idle()
    {
        StopMovement();
       // Debug.Log("iddleling");

       if (!_waiting && _firstIdle)
       {
           _waiting = true;
           _idle = true;
           StartCoroutine(FirstIdle());
       }
       
        else if ( !_waiting && _dashed ) //_dashed == cooldown in concept for while, to give some time to wander again
        {
            _waiting = true;
           
            StartCoroutine(WaitingIdleTime(idleTime)); //can be random
        }
        else if ( !_waiting && _wandering)
        {
            _waiting = true;
            StartCoroutine(RandomlyWanderingIn(idleTime)); //can be random
        }
        else if (_wandering)
        {
           // Debug.Log("iddle ~wandering cancelled, to attack");
            if (IsPlayerInAttackRange(sightRange))
            {
                _wandering = false;
                _idle = false;
                _waiting = false;
                currState = EnemyState.Attack;
            }
        }else if (!_waiting && _followingTimeIsOver)
        {
            print("idling after followed");
            _waiting = true;
            StartCoroutine(WaitingIdleTimeAfterFollowing(idleTime));
        }
        
    }

    public bool GetWaiting()
    {
        return _waiting;
    }

    public void SetWaiting(bool aux)
    {
        _waiting = aux;
    }

    public IEnumerator WaitingIdleTimeAfterFollowing(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _idle = false;
        _followingTimeIsOver = false;
        _waiting = false;
    }
    
    public virtual IEnumerator RandomlyWanderingIn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (currState == EnemyState.Idle)
        {
            _idle = false;
            
        }
        _wandering = false;
        _waiting = false;
        
    }

    public IEnumerator WaitingIdleTime(float sec)
    {
        yield return new WaitForSeconds(sec);
       // Debug.Log("wander after iddle: "+sec+" seconds");
        _coolDownAttack = false; //reseted 
        _waiting = false;
        _dashed = false;
        _idle = false;
        currState = EnemyState.Wander;
        
    }
    
    public virtual void Holding()
    {
        StopMovement();
        
        if (!_waitingHold)
        {
            print("Holding:");
            
           
            _waitingHold = true;
            
           StartCoroutine(StopHolding((holdingTime/3)*2,holdingTime/3));
        }
        
        flipStaticEnemy();
    }

    public virtual IEnumerator StopHolding(float timeToGetEnemyPos, float timeAfterGotEnemyPos)
    {
        yield return new WaitForSeconds(timeToGetEnemyPos);
        
        _lastPlayerPosition = _player.position;
        _lastEnemyPosition = transform.position;
        print("Saving player position: "+_lastPlayerPosition);
        print("Saving enemy position: " + _lastEnemyPosition);
        
        yield return new WaitForSeconds(timeAfterGotEnemyPos);
        
        StartDashing();
        _holding = false;

    }

    private void StartDashing()
    {
        _dashing = true;
    }

    public virtual void Dash()
    {
      //  Debug.Log("Dashing");
      
        if (_dashTime <= 0)
        {
            ResetDash();
        }
        else
        {
            _dashTime -= Time.deltaTime;
          MoveEnemy(DirectionNormalized(_lastEnemyPosition,_lastPlayerPosition),dashSpeed);
        }
    }

    public virtual void ResetDash()
    {
        _dashTime = dashTime;
        _dashing = false;
        _idle = true;
        _dashed = true;
        _waitingHold = false;
    }

    public virtual void Attack() //not implemented yet
    {
        _wandering = false;
        
        if (!_coolDownAttack)
        {
            switch (enemyType)
            {
                case(EnemyType.Melee):
                    
                    BasicAttacks();
                    
                    break;
                case(EnemyType.Ranged):
                    //now only shooting temporary projectiles
                    
                    LoadOrShoot(); //if you not chose to stop bullets to reload, it will just shoot
                    
                    if (moveWhileShoot)
                    {
                       // Debug.Log("what");
                        WalkShoot(); //shoot and move at same time;
                    }
                    else
                    {
                        StopMovement();
                    }
                    
                    break;
            }
        }
    }

    public virtual void BasicAttacks()
    {
        if (enemyMeleeDash)
        {
            StartHoldToDash();
        }
        else
        {
            BasicAttack();
        }
    }

    public void StartHoldToDash()
    {
        _holding = true;
        Debug.Log("Start Dashing...");
    }

    public bool getBasicMeleeAttack()
    {
        return _basicMeleeAttack;
    }

    public virtual void BasicAttack()
    {
        if (!_basicMeleeAttack) // if not used yet
        {
            if (!_saveLastPos)
            {
                _saveLastPos = true;
                _randomDir = DirectionNormalized(transform.position, _player.position);
            }
            
            if(_basicMeleeAttackTime <= 0)
            {
                _basicMeleeAttackTime = basicMeleeAttackTime;
                _saveLastPos = false;
                StartCoroutine(BasicAttackCooldown());
            }
            else
            {
                MoveEnemy(_randomDir,speed*70*Time.deltaTime);
                _basicMeleeAttackTime -= Time.deltaTime;
            }
        }
    }

    public IEnumerator BasicAttackCooldown()
    {
        _basicMeleeAttack = true;
        yield return new WaitForSeconds(cooldownBasicMeleeAttackTime);
        _basicMeleeAttack = false;
    }

    public void LoadOrShoot()
    {
        if (!IsEnemyOutOfAmmo())
        {
            Shooting(); //temporary
            if (_stopShootToReload)
            {
                StartCoroutine(WasteBullets());
            }
        }
        else
        {
            Reload();
        }
    }

    public IEnumerator WasteBullets()
    {
        _stopShootToReload = false;
        yield return new WaitForSeconds(timeAmmo);
        _stopShootToReload = true;
        _outOfAmmo = true;
    }
    
    public bool IsEnemyOutOfAmmo()
    {
        return _outOfAmmo;
    }
    
    public virtual void Reload()
    {
        if (!_reloading)
        {
            StartCoroutine(LoadingAmmo()); //one to load
        }
    }
    
    public IEnumerator LoadingAmmo()
    {
        _reloading = true;
        yield return new WaitForSeconds(timeToReload);
        _reloading = false;
        _outOfAmmo = false;
    }
    
    private void WalkShoot()
    {
        //Debug.Log("walkshoot");
        if (timeWalkingOneDirection > 0) // can be random the time...
        {
            if (_timeWalking <= 0)
            {
                _timeWalking = timeWalkingOneDirection;
                _randomDir = ChooseTypeOfWalk();
                Debug.Log("reset Direction");
            }
            else
            {
               // Debug.Log("time: "+_timeWalking);
                _timeWalking -= Time.deltaTime;
                MoveEnemy(_randomDir,speed);
            }
        }
    }

    public Vector3 ChooseTypeOfWalk() // return a direction to walk
    {
        //to-do circle
        
        Vector3 newPoint = transform.position;

        if (followWalk) //ignore the others
        {
            newPoint = FollowWalk(newPoint);
        }
        
        if (zigZagWalkHorizontal)
        {
            newPoint = ZigZagWalkHorizontal(newPoint);
        }

        if (zigZagWalkVertical)
        {
            newPoint = ZigZagWalkVertical(newPoint);
        }

        if (horizontalWalk)
        {
            newPoint = HorizontalWalk(newPoint);
        }

        if (verticalWalk)
        {
            newPoint = VerticalWalk(newPoint);
        }

        if (randomWalk)
        {
            newPoint =  ChoosePointRandomlyToWalk(newPoint);
        }
       

        Vector3 dirToWalk = DirectionNormalized(transform.position, newPoint);
        return dirToWalk;
    }

    public Vector3 FollowWalk(Vector3 pos)
    {
        Vector3 dirToWalk = DirectionNormalized(transform.position, _player.position);
        Vector3 newPoint = dirToWalk + pos;
        
        return newPoint;
    }

    public Vector3 ZigZagWalkVertical(Vector3 pos)
    {

        int[] lottery = new int[2] {- 1, 1 };

        int aux = Random.Range(0, 2); 
        
        if (!_zigZagVertical)  // 
        {
            pos = new Vector3(pos.x-1,pos.y+lottery[aux],pos.z);
        }
        else if(_zigZagVertical) 
        {
            pos = new Vector3(pos.x+1,pos.y+lottery[aux],pos.z);
        }

        _zigZagVertical = !_zigZagVertical;
        return pos;
    }
    
    public Vector3 ZigZagWalkHorizontal(Vector3 pos)
    {
        int[] lottery = new int[2] {- 1, 1 };
        int aux = Random.Range(0, 2); 
        
        if (!_zigZagHorizontal)  // 
        {
            pos = new Vector3(pos.x+lottery[aux],pos.y+1,pos.z);
        }
        else if(_zigZagHorizontal) 
        {
            pos = new Vector3(pos.x+ lottery[aux],pos.y-1,pos.z);
        }
        _zigZagHorizontal = !_zigZagHorizontal;
        return pos;
    }

    public Vector3 HorizontalWalk(Vector3 pos)
    {
        int[] lottery = new int[2] {- 1, 1 };
        int aux = Random.Range(0, 2);
        pos = new Vector3(pos.x+lottery[aux],pos.y,pos.z);
        return pos;
    }
    
    public Vector3 VerticalWalk(Vector3 pos)
    {
        int[] lottery = new int[2] {- 1, 1 };
        int aux = Random.Range(0, 2); 
        pos = new Vector3(pos.x,pos.y+lottery[aux],pos.z);
        return pos;
    }

    public bool IsEnemyHoldingToDash()
    {
        return _holding;
    }
    
    public bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, _player.position) <= range;
    }
    
    public bool IsPlayerInAttackRange(float attackRange)
    {
        return Vector3.Distance(transform.position, _player.position) <= attackRange;
    }

    /* Deprecated
    private IEnumerator CoolDown()
    {
        _coolDownAttack = true;
        yield return new WaitForSeconds(coolDown);
        _coolDownAttack = false;
    }
    */
    public virtual void Wander()
    {
        if (!_wandering)
        {
            _wandering = true;
            _randomDir = ChooseTypeOfWalk();
            StartCoroutine(RandomlyIdleIn(wanderingTime)); //can be random
            if (wanderingTime > timeWalkingOneDirection)
            {
                StartCoroutine(timeWalkingOneDirectionWandering());
            }
        }
        
        MoveEnemy(_randomDir,speed);
    }

    public bool GetWandering()
    {
        return _wandering;
    }

    public void SetWandering(bool aux)
    {
        _wandering = aux;
    }
    
    public IEnumerator timeWalkingOneDirectionWandering()
    {
        yield return new WaitForSeconds(timeWalkingOneDirection);
        if (_wandering && EnemyState.Wander == currState)
        {
            _randomDir = ChooseTypeOfWalk();
            StartCoroutine(timeWalkingOneDirectionWandering());
        }
    }

    private IEnumerator RandomlyIdleIn( float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (currState == EnemyState.Wander) // to prevent a coroutine in the wrong time
        {
            _idle = true;
        }
        else
        {
            _wandering = false;
        }
    }
    
    
    public Vector3 ChoosePointRandomlyToWalk(Vector3 pos)
    {
        pos = pos + new Vector3(Random.Range(-1.0f, 1.0f),Random.Range(-1.0f, 1.0f),0);
        return pos;
    }
    
   
    
    public virtual GameObject Shooting()
    {
        GameObject aux = null;
        if(_timeBtwShots <= 0)
        {
            Vector3 centerBox = GetComponent<Collider2D>().offset;
            aux = Instantiate(projectile,transform.TransformPoint(centerBox), transform.rotation);
            aux.GetComponent<Projectile>().SetInstantiator(this.gameObject);
            _timeBtwShots = timeBtwShots;
        }
        else
        {
           _timeBtwShots -= Time.deltaTime;
        }

        return aux;
    }

    public virtual bool flipStaticEnemy()
    {
        var aux = _sprite.flipX;
        Vector3 dir = DirectionNormalized(transform.position, _player.position);
        if (dir.x < 0)
        {
            _sprite.flipX = true;
        }
        else
        {
            _sprite.flipX = false;
        }

        if (aux != _sprite.flipX)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector3 PlayerDirection()
    {
        Vector3 dir = DirectionNormalized(transform.position, _player.position);
        return dir;
    }
    
    public Vector3 DirectionNormalized(Vector3 current, Vector3 target)
    {
        return Vector3.Normalize(target - current);
    }
    
    public virtual void Follow()
    {
        if (stateHasChanged) //reset timer
        {
            ResetFollowingTime();
        }
        
        if (_timeFollowing <= 0)
        {
            _idle = true;
            _followingTimeIsOver = true;
        }
        else
        {
            _timeFollowing -= Time.deltaTime;
            MoveEnemy(DirectionNormalized(transform.position,_player.position),speed);
        }
    }

    public void SetTimeFollowing(float dale)
    {
        _timeFollowing = dale;
    }
    
    public void ResetFollowingTime()
    {
        _timeFollowing = maxTimeFollowing;
    }

    public float GetTimeFollowing()
    {
        return _timeFollowing;
    }
    
    public virtual void MoveEnemy(Vector3 dir,float speed)
    {
        _movementComponent.Move(dir.x * speed * Time.deltaTime,dir.y * speed * Time.deltaTime);
       //transform.position += dir * speed * Time.deltaTime;
    }

    public bool IsTimeToRetreat(float retreatDistance)
    {
        if(Vector2.Distance(transform.position, _player.position) < retreatDistance)
        {
            return true;
        }

        return false;
    }
    public void Retreat()
    {
        MoveEnemy(DirectionNormalized(transform.position,_player.position), -speed);
    }

    public bool IsEnemyDashing()
    {
        return _dashing;
    }

    public virtual void Death()
    {
        print("Death function called");
        if (!_dead)
        {
            StopMovement();
            
            if (explodeWhenDie)
            {
                ExplodeWhenDie();
            }

            DeathRoutine();

            print(this.gameObject.name+" killed");
            _dead = true;
        }
    }

    public virtual void DeathRoutine()
    {
        Destroy(this.GetComponent<Rigidbody2D>());
        Destroy(GetComponent<Collider2D>());


        StartCoroutine(FadingSequence());

        var loot = GetComponent<LootManager>();
        if (loot)
        {
            loot.CalculateLoot();
        }
    }
    
    public virtual IEnumerator FadingSequence()
    {
        Sequence fadingSequence = DOTween.Sequence();
        fadingSequence.Append(_sprite.DOFade(1f, 1f));
        fadingSequence.Append(_sprite.DOFade(0f, 2f));
        fadingSequence.Play();
        yield return fadingSequence.WaitForCompletion();
        fadingSequence.Kill();
        DestroyEnemy();
    }

    private void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }

    public virtual void ExplodeWhenDie() //override this function to make something better
    {
        //projectile.transform.localScale = Vector3.one*2; //demonstration
        //do something bigger
        var foo = Instantiate(projectile, transform.position, transform.rotation);
        foo.GetComponent<Projectile>().SetInstantiator(gameObject);
    }

    public float getMaximumHealth()
    {
        return _statusComponent.GetMaxHealth();
    }
    
    public bool isDead()
    {
        return _dead;
    }
    

    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            other.gameObject.GetComponent<StatusComponent>().TakeDamage(10);
            Debug.Log("collision hit, player taking damage.");
        }
        
    }

}
