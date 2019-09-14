using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using GameAnalyticsSDK.Setup;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public String targetTag;

    private GameObject _instantiator;


    public void SetInstantiator(GameObject instantiator)
    {
        _instantiator = instantiator;
    }

    public GameObject GetInstantiator()
    {
        return _instantiator;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( other.CompareTag(targetTag) || other.CompareTag("Wall") || other.CompareTag("Door"))
        {
            if (other.CompareTag(targetTag)) 
            {
                other.gameObject.GetComponent<StatusComponent>().TakeDamage(10);
            } 
            Destroy(gameObject);
        }
    }
}