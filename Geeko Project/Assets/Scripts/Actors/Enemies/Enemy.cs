using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float stoppingDistance;
    public float retreatDistance;

    private float timeBtwShots;
    public float startTimeBtwShots;

    public GameObject projectile;
    private Transform player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        timeBtwShots = startTimeBtwShots;
    }

    
    void Update()
    {
        FollowAndRetreat(transform, player, stoppingDistance, retreatDistance);
        
        Shooting(timeBtwShots,startTimeBtwShots);
    }


    public void Shooting(float timeBtwShots, float startTimeBtwShots)
    {
        if(timeBtwShots <= 0)
        {
            Instantiate(projectile, transform.position, transform.rotation);
            this.timeBtwShots = startTimeBtwShots;
        }
        else
        {
           this.timeBtwShots -= Time.deltaTime;
        }
    }
    
    public void FollowAndRetreat(Transform enemy, Transform player, float stoppingDistance, float retreatDistance)
    {
        if (Vector2.Distance(enemy.position, player.position) > stoppingDistance) //following
        {
           transform.position = Vector2.MoveTowards(enemy.position, player.position, speed * Time.deltaTime);
            
        } else if (Vector2.Distance(enemy.position, player.position) < stoppingDistance &&
                   Vector2.Distance(enemy.position, player.position) > retreatDistance)
        { //where to stop
            this.transform.position = enemy.position;
        }
        else if (Vector2.Distance(enemy.position, player.position) < retreatDistance){ //retreat
            this.transform.position = Vector2.MoveTowards(enemy.position, player.position, -speed * Time.deltaTime);

        }
    }
    
}
