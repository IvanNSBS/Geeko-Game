using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


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
}

public class Enemy : MonoBehaviour
{
    public EnemyState currState = EnemyState.Wander;
    public EnemyType enemyType;
    
    public float range; //sight range
    public float attackRange;
    public float speed;
    public float dashSpeed;
    public float startDashTime;
    
    public float stoppingDistance;
    public float retreatDistance;

    public float coolDown;
    private bool coolDownAttack = false;
    public float startTimeBtwShots;

    
    private float timeBtwShots;
    private bool chooseDir;
    private Vector3 randomDir;
    private float dashTime;
    private bool holding;
    private bool dashing;
    private bool waiting = false;
    private bool iddle= false;
    
    public GameObject projectile;
    private Transform player;
    private Vector3 lastPlayerPosition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        dashTime = startDashTime;
        timeBtwShots = startTimeBtwShots;
        holding = false;
        dashing = false;
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

        if (iddle)
        {
            currState = EnemyState.Iddle;
        }
        else if (IsEnemyHoldingToDash())
        {
            currState = EnemyState.Hold;
        }else if (dashing)
        {
            currState = EnemyState.Dash;
        }
        else if (IsPlayerInAttackRange(attackRange)&&(!dashing))
        {
            currState = EnemyState.Attack;
        }
        /*
        if (IsTimeToRetreat(retreatDistance))
        {
            currState = EnemyState.Retreat;
        }
        */
    }

    public void Iddle()
    {
        if (!waiting)
        {
            StartCoroutine(waitingSecs(2));
            waiting = true;
            
        }
    }

    public IEnumerator waitingSecs(float sec)
    {
        yield return new WaitForSeconds(sec);
        Debug.Log("wander after iddle");
        currState = EnemyState.Wander;
        iddle = false;
        waiting = false;
    }
    
    public void Holding()
    {
        if (!dashing)
        {
            lastPlayerPosition = player.position;
            dashing = true;
            StartCoroutine(CoolDown());
        }
        else if(!coolDownAttack)
        {
            holding = false;
        }
        
    }
    
    public void Dash()
    {
        if (dashTime <= 0)
        {
            dashTime = startDashTime;
            dashing = false;
            iddle = true;
        }
        else
        {
            dashTime -= Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, lastPlayerPosition, dashSpeed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        //not implemented yet
        if (!coolDownAttack)
        {
            switch (enemyType)
            {
                case(EnemyType.Melee):
                    //now only dashing, so he's holding to dash
                    holding = true;
                    Debug.Log("ATTACK MELEE!!");
                    //StartCoroutine(CoolDown());
                    break;
                case(EnemyType.Ranged):
                    Shooting();
                    StartCoroutine(CoolDown());
                    break;
            }
        }
    }

    public bool IsEnemyHoldingToDash()
    {
        return holding;
    }
    
    public bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.position) <= range;
    }
    
    public bool IsPlayerInAttackRange(float attackRange)
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }

    private IEnumerator CoolDown()
    {
        coolDownAttack = true;
        yield return new WaitForSeconds(coolDown);
        coolDownAttack = false;
    }
    
    public void Wander()
    {
        if (!chooseDir)
        {
            StartCoroutine(ChooseDirection());
        }

        transform.position += -transform.right * speed * Time.deltaTime;
        if (IsPlayerInRange(range))
        {
            currState = EnemyState.Follow;
        }
    }

    private IEnumerator ChooseDirection()
    {
        chooseDir = true;
        yield return new WaitForSeconds(Random.Range(2f,8f));
        randomDir = new Vector3(0,0,Random.Range(0,360));
        Quaternion nextRotation = Quaternion.Euler(randomDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Random.Range(0.5f, 2.5f));
        chooseDir = false;
    }
    
    public void Shooting()
    {
        if(timeBtwShots <= 0)
        {
            Instantiate(projectile, transform.position, transform.rotation);
            timeBtwShots = startTimeBtwShots;
        }
        else
        {
           timeBtwShots -= Time.deltaTime;
        }
    }

    public void Follow()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    public bool IsTimeToRetreat(float retreatDistance)
    {
        if(Vector2.Distance(transform.position, player.position) < retreatDistance)
        {
            return true;
        }

        return false;
    }
    public void Retreat()
    {
        this.transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
    }

    public void Death()
    {
        Destroy(gameObject);
    }
    
}
