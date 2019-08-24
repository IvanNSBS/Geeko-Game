﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField][Range(0,100)] private float m_MovementSpeed = 10.0f; // max actor speed
    [SerializeField][Range(0,2)] private float m_AccelerationTime = 0.0f; // time to reach max speed
    [SerializeField][Range(0,2)] private float m_DeaccelerationTime = 0.0f; // time to zero your speed

    private Rigidbody2D m_ActorRigidBody = null;
    private SpriteRenderer m_ActorSprite = null;
    private Vector2 m_RefSpeed = Vector2.zero;
    private float m_Magnitude = 10.0f; // movement speed magnitude. Used only to not need crazy values on movespeed.
    public bool SetRigidBody()
    {
        m_ActorRigidBody = GetComponent<Rigidbody2D>();
        if (m_ActorRigidBody)
            return true;
        else
            return false;
    }
    public bool SetSprite()
    {
        m_ActorSprite = GetComponent<SpriteRenderer>();
        if (m_ActorSprite)
            return true;
        else
            return false;
    }
    Rigidbody2D GetRigidbody() { return m_ActorRigidBody; }

    private void Start()
    {
        if (!SetRigidBody())
            Debug.LogWarning("WARNING! MovementComponent RigidBody wasn't successfully set\n" +
                "Actor won't be able to move.");
        if (!SetSprite())
            Debug.LogWarning("WARNING! Sprite Render wasn't successfully set\n" +
                "Actor won't be able to update sprite from Movement Component.");
    }

    public void SetMoveSpeed(float nspeed) { m_MovementSpeed = nspeed;  }
    public float GetMoveSpeed() { return m_MovementSpeed * m_Magnitude; }
    public Vector2 GetVelocity() { return m_ActorRigidBody.velocity;  }
    public void Move(float speed_x, float speed_y)
    {
        if (m_ActorRigidBody)
        {
            Vector2 newspeed = new Vector2(speed_x * GetMoveSpeed(), speed_y*GetMoveSpeed());
            if(newspeed.magnitude >= GetVelocity().magnitude)
                m_ActorRigidBody.velocity = Vector2.SmoothDamp(m_ActorRigidBody.velocity, newspeed, ref m_RefSpeed, m_AccelerationTime);
            else
                m_ActorRigidBody.velocity = Vector2.SmoothDamp(m_ActorRigidBody.velocity, newspeed, ref m_RefSpeed, m_DeaccelerationTime);
        }
        if (m_ActorSprite && speed_x > 0.0f)
            m_ActorSprite.flipX = false;
        else if (m_ActorSprite && speed_x < 0.0f)
            m_ActorSprite.flipX = true;
    }
}