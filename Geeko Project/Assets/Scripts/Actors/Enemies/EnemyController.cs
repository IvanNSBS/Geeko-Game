using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Experimental.PlayerLoop;
using Random = UnityEngine.Random;


public enum EnemyState{

    Wander,
    
    Follow,
    
    Die,
    
    Attack,
    
    Retreat,
    
    Dash,
    
    Hold,
    
    Iddle,
};

public enum EnemyType
{
    Melee,
    Ranged,
};

public class EnemyController : MonoBehaviour
{
    public EnemyState currState = EnemyState.Wander;
    public EnemyType enemyType;
    
    public float range; //sight range
    public float attackRange;
    public float speed;
    public float dashSpeed;
    public float startDashTime;
    public float iddleTime;
    public float holdingTime;
    public float stoppingDistance;
    public float retreatDistance;

    public float coolDown;
    
    private bool _coolDownAttack = false;
    public float startTimeBtwShots;
    public bool explodeWhenDie;
    public bool moveWhileShoot;
    public bool stopShootToReload;
    public float timeToReload;
    public float timeAmmo;
    
    private float _timeBtwShots;
    private bool _wandering;
    private Vector3 _randomDir;
    private float _dashTime;
    private bool _holding = false;
    private bool _dashing = false;
    private bool _waiting = false;
    private bool _iddle= false;
    
    
    public GameObject projectile;
    private Transform _player;
    private Vector3 _lastPlayerPosition;
    
    private MovementComponent _movementComponent;
    private StatusComponent _statusComponent;
    private bool _dashed = false;
    private bool _reloading = false;
    private bool _outOfAmmo = false;
    
    /* TO-DO
    BOSS
    Walks and shootings
    */
    private void Awake()
    {
        _movementComponent = GetComponent<MovementComponent>();
        _statusComponent = GetComponent<StatusComponent>();
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        projectile.transform.localScale = Vector3.one / 2;
        _dashTime = startDashTime;
        _timeBtwShots = startTimeBtwShots;
    }

    
    void Update()
    {
        switch (currState)
        {
            case(EnemyState.Wander):
                Wander();
                break;
            case(EnemyState.Follow):
                Follow();
                break;
            case(EnemyState.Die):
                Death();
                break;
            case(EnemyState.Dash):
                Dash();
                break;
            case(EnemyState.Hold):
                Holding();
                break;
            case(EnemyState.Attack):
                Attack();
                break;
            case(EnemyState.Retreat):
                Retreat();
                break;
            case(EnemyState.Iddle):
                Iddle();
                break;
        }
        if (IsPlayerInRange(range) && currState != EnemyState.Die)
        {
            currState = EnemyState.Follow;
        }else if (!IsPlayerInRange(range) && currState != EnemyState.Die)
        {
            currState = EnemyState.Wander;
        }
        
        
        if ( (_iddle)) 
        {
            currState = EnemyState.Iddle;
        }
        else if (IsEnemyHoldingToDash())
        {
            currState = EnemyState.Hold;
        }else if (_dashing)
        {
            currState = EnemyState.Dash;
        }
        else if (IsPlayerInAttackRange(attackRange)&&(!_dashing))
        {
            currState = EnemyState.Attack;
        }

        /*
        if (IsTimeToRetreat(retreatDistance))
        {
            currState = EnemyState.Retreat;
        }
        */
        if (_statusComponent.GetCurrentHealth() <= 0)
        {
            currState = EnemyState.Die;
        }
    }

    public void Iddle()
    {
       // Debug.Log("iddleling");
        
        if ( !_waiting && _dashed ) //_dashed == cooldown in concept for while, to give some time to wander again
        {
            _waiting = true;
           
            StartCoroutine(WaitingIddleTime(iddleTime));
        }
        else if ( !_waiting && _wandering)
        {
            _waiting = true;
            StartCoroutine(RandomlyWanderingIn(Random.Range(1.0f,2.0f)));
        }
        else if (_wandering)
        {
           // Debug.Log("iddle ~wandering cancelled, to attack");
            if (IsPlayerInAttackRange(range))
            {
                _wandering = false;
                _iddle = false;
                _waiting = false;
                currState = EnemyState.Attack;
            }
        }
        
    }

    public IEnumerator RandomlyWanderingIn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (currState == EnemyState.Iddle)
        {
            _iddle = false;
            
        }
        _wandering = false;
        _waiting = false;
        
    }

    public IEnumerator WaitingIddleTime(float sec)
    {
        yield return new WaitForSeconds(sec);
       // Debug.Log("wander after iddle: "+sec+" seconds");
        _coolDownAttack = false; //reseted 
        _iddle = false;
        _waiting = false;
        _dashed = false;
        currState = EnemyState.Wander;
        
    }
    
    public void Holding()
    {
        //Debug.Log("Holding");
        
        if (!_dashing)
        {
            _lastPlayerPosition = _player.position;
            _dashing = true;
            
           StartCoroutine(StopHolding());
        }
        
    }

    public IEnumerator StopHolding()
    {
        yield return new WaitForSeconds(holdingTime);
        _holding = false;
    }
    
    public void Dash()
    {
      //  Debug.Log("Dashing");
      
        if (_dashTime <= 0)
        {
            _dashTime = startDashTime;
            _dashing = false;
            _iddle = true;
            _dashed = true;
        }
        else
        {
            _dashTime -= Time.deltaTime;
          MoveEnemy(DirectionNormalized(transform.position,_lastPlayerPosition),dashSpeed);
        }
    }

    private void Attack() //not implemented yet
    {
        _wandering = false;
        
        if (!_coolDownAttack)
        {
            switch (enemyType)
            {
                case(EnemyType.Melee):
                    //now only dashing, so he's holding to dash
                    //Debug.Log("ATTACK MELEE!! DASH");
                    _holding = true;
                    
                    break;
                case(EnemyType.Ranged):
                    //now only shooting temporary projectiles
                    
                    LoadOrShoot(); //if you not chose to stop bullets to reload, it will just shoot
                    
                    if (moveWhileShoot)
                    {
                        WalkShoot(); //shoot and move at same time;
                    }
                    
                    break;
            }
        }
    }

    public void LoadOrShoot()
    {
        if (!IsEnemyOutOfAmmo())
        {
            Shooting(); //temporary
            if (stopShootToReload)
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
        stopShootToReload = false;
        yield return new WaitForSeconds(timeAmmo);
        stopShootToReload = true;
        _outOfAmmo = true;
    }
    
    public bool IsEnemyOutOfAmmo()
    {
        return _outOfAmmo;
    }
    
    public void Reload()
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
        //RandomWalk();
        //ZigZagWalk();
        HorizontalWalk();
        //VerticalWalk();
        //CircleWalk();
        //FollowWalk();
        
        throw new NotImplementedException();
    }

    public void HorizontalWalk()
    {
        Vector3 actualPos = transform.position;
        Vector3 dir = (actualPos)-actualPos;
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

    private IEnumerator CoolDown()
    {
        _coolDownAttack = true;
        yield return new WaitForSeconds(coolDown);
        _coolDownAttack = false;
    }
    
    public void Wander()
    {
        if (!_wandering)
        {
            _wandering = true;
            ChooseDirectionRandomlyToWalk();
            StartCoroutine(RandomlyIddleIn(Random.Range(1.0f, 4.0f)));
            //walk for Random(1f,4f) seconds in a random direction
        }
        
        MoveEnemy(_randomDir,speed);
    }

    private IEnumerator RandomlyIddleIn( float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (currState == EnemyState.Wander) // to prevent a coroutine in the wrong time
        {
            _iddle = true;
        }
        else
        {
            _wandering = false;
        }
    }
    
    
    public void ChooseDirectionRandomlyToWalk()
    {
        Vector3 randomPoint = transform.position;
        /*float aux = Random.Range(-1.0f, 1.0f);
        while (aux == 0)
        {
            aux = Random.Range(-1.0f, 1.0f);
            Debug.Log("re-lottery direction");
        }*/
        randomPoint = randomPoint + new Vector3(Random.Range(-1.0f, 1.0f),Random.Range(-1.0f, 1.0f),0);
        _randomDir = Vector3.Normalize(randomPoint - transform.position);
    }
    
   
    
    public void Shooting()
    {
        
        if(_timeBtwShots <= 0)
        {
            Instantiate(projectile, transform.position, transform.rotation);
            _timeBtwShots = startTimeBtwShots;
        }
        else
        {
           _timeBtwShots -= Time.deltaTime;
        }
        
    }

    public Vector3 DirectionNormalized(Vector3 current, Vector3 target)
    {
        return Vector3.Normalize(target - current);
    }
    
    public void Follow()
    {
        MoveEnemy(DirectionNormalized(transform.position,_player.position),speed);
    }

    public void MoveEnemy(Vector3 dir,float speed)
    {
        // _movementComponent.Move(dir.x * speed * Time.deltaTime,dir.y * speed * Time.deltaTime);
       transform.position += dir * speed * Time.deltaTime;
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

    public void Death()
    {
        if (explodeWhenDie)
        {
           projectile.transform.localScale = Vector3.one*2;
            Instantiate(projectile, transform.position, transform.rotation);
        }
        Destroy(gameObject);
        Debug.Log("enemy killed");
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            other.gameObject.GetComponent<StatusComponent>().TakeDamage(1);
            Debug.Log("collision hit, player taking damage.");
        }
        
    }
}
