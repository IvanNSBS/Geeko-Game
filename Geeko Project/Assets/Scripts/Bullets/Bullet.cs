using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject owner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( other.CompareTag("Enemy") || other.CompareTag("Wall") || other.CompareTag("Door"))
        {
            if (other.CompareTag("Enemy")) 
            {
                other.gameObject.GetComponent<StatusComponent>().TakeDamage(10);
            } 
            Destroy(gameObject);
        }
    }
}