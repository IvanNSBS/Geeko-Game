using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField][Range(0,100)] private float m_MovementSpeed = 10.0f; // max actor speed
    [SerializeField][Range(0,2)] private float m_AccelerationTime = 0.0f; // time to reach max speed
    [SerializeField][Range(0,2)] private float m_DeaccelerationTime = 0.0f; // time to zero your speed
    public UnityEvent m_OnFlip;


    private Rigidbody2D m_ActorRigidBody = null;
    private SpriteRenderer m_ActorSprite = null;
    private Vector2 m_RefSpeed = Vector2.zero;
    private float m_Magnitude = 10.0f; // movement speed magnitude. Used only to not need crazy values on movespeed.

    public SpriteRenderer GetSprite() { return m_ActorSprite; }
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

    //Stop rigidbody acceleration so it can actually stop moving
    public void StopMovement()
    {
        if (m_ActorRigidBody)
        {
            m_ActorRigidBody.angularVelocity = 0;
            m_ActorRigidBody.velocity = Vector3.zero;
        }
    }

    public void FlipSprite(float x)
    {
        // If actor tried to move right, make sprite look to right
        if (m_ActorSprite && x > 0.0f)
        {
            if (m_OnFlip != null && !m_ActorSprite.flipX) // call auxiliary OnFlip Event
                m_OnFlip.Invoke();
            m_ActorSprite.flipX = false;
        }
        // If actor tried to move left, make sprite look to right
        else if (m_ActorSprite && x < 0.0f)
        {
            if (m_OnFlip != null && m_ActorSprite.flipX)// call auxiliary OnFlip Event
                m_OnFlip.Invoke();
            m_ActorSprite.flipX = true;
        }
    }
    public void Move(float speed_x, float speed_y)
    {
        if (m_ActorRigidBody) // if the actor has a rigidbody
        {
            //update actor speed
            Vector2 newspeed = new Vector2(speed_x * GetMoveSpeed(), speed_y*GetMoveSpeed());
            if (newspeed.magnitude >= GetVelocity().magnitude)
            {
                //interp current speed based on acceleration time
                //m_ActorRigidBody.velocity = Vector2.SmoothDamp(m_ActorRigidBody.velocity, newspeed, ref m_RefSpeed, m_AccelerationTime);
                m_ActorRigidBody.velocity = newspeed;
            }
            else {
                //interp current speed to idle based on deacceleration time
                //m_ActorRigidBody.velocity = Vector2.SmoothDamp(m_ActorRigidBody.velocity, newspeed, ref m_RefSpeed, m_DeaccelerationTime);
                m_ActorRigidBody.velocity = newspeed;
            }

        }

        FlipSprite(speed_x);
    }
}
