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
        if (other.gameObject != owner && !other.CompareTag("Untagged"))
        {
            Debug.Log(owner);
            Debug.Log(other.gameObject);
            Destroy(gameObject);
        }
    }
}