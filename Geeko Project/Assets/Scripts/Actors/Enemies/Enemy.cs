using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyState{

    Wander,
    
    Follow,
    
    Die,
    
    Attack,
    
    Retreat
};

public class Enemy : MonoBehaviour
{
    public EnemyState currState = EnemyState.Wander;
    
    public float range;
    public float attackRange;
    public float speed;
    
    public float stoppingDistance;
    public float retreatDistance;

    public float startTimeBtwShots;

    
    private float timeBtwShots;
    private bool chooseDir;
    private Vector3 randomDir;
    
    public GameObject projectile;
    private Transform player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        timeBtwShots = startTimeBtwShots;
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
            case(EnemyState.Attack):
                RangeAttack();
                break;
            case(EnemyState.Retreat):
                Retreat();
                break;
        }

        if (IsPlayerInRange(range) && currState != EnemyState.Die)
        {
            currState = EnemyState.Follow;
        }else if (!IsPlayerInRange(range) && currState != EnemyState.Die)
        {
            currState = EnemyState.Wander;
        }

        if (IsPlayerInAttackRange(attackRange))
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

    private void RangeAttack()
    {
        //not implemented yet
        Shooting();
    }

    public bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.position) <= range;
    }
    
    public bool IsPlayerInAttackRange(float attackRange)
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange;
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
