using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    [HideInInspector] public float m_Damage;
    [HideInInspector] public float m_ProjectileSpeed;

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(m_ProjectileSpeed, m_ProjectileSpeed);
    }
}
