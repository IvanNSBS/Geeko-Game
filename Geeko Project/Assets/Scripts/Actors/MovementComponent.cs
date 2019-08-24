using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float m_MovementSpeed = 10.0f; // max actor speed
    [SerializeField] private float m_MaxAccelerationTime = 0.2f; // time to reach max speed
    private Rigidbody2D m_ActorRigidBody = null;
    private Vector2 m_RefSpeed = Vector2.zero;
    bool SetRigidBody()
    {
        m_ActorRigidBody = GetComponent<Rigidbody2D>();
        if (m_ActorRigidBody)
            return true;
        else
            return false;
    }

    private void Start()
    {
        bool success = SetRigidBody();
        if (!success)
            Debug.LogWarning("WARNING! MovementComponent RigidBody wasn't successfully set\n" +
                "Actor won't be able to move.");
    }

    public void SetMoveSpeed(float nspeed) { m_MovementSpeed = nspeed;  }
    public float GetMoveSpeed() { return m_MovementSpeed; }
    public void Move(float speed_x, float speed_y)
    {
        Vector2 newspeed = new Vector2(speed_x*m_MovementSpeed, speed_y*m_MovementSpeed);
        m_ActorRigidBody.velocity = Vector2.SmoothDamp(m_ActorRigidBody.velocity, newspeed, ref m_RefSpeed, m_MaxAccelerationTime); ;
    }
}
