using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float speed;

    private Transform player;
    private Vector2 target;
    private Vector3 direction;
    private GameObject instantiator;

    public void SetInstantiator(GameObject instantiator)
    {
        this.instantiator = instantiator;
    }

    public GameObject GetInstantiator()
    {
        return instantiator;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 center = player.TransformPoint(player.GetComponent<CircleCollider2D>().offset);
        direction = (center - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
    }

    void Update()
    {
        
        transform.position += direction * speed * Time.deltaTime;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player")) || (other.CompareTag("Door")) || (other.CompareTag("Wall")))
        {
            if(other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<StatusComponent>().TakeDamage(10);
            }
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
