using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private MovementComponent m_MovementComponent;
    private StatusComponent m_StatusComponent;
    private SpellCastingComponent m_SpellComponent;
    void Start()
    {
        if (!m_MovementComponent)
        {
            m_MovementComponent = GetComponent<MovementComponent>();
            if (!m_MovementComponent)
                Debug.LogWarning("Actor MovementComponent wasn't successfully set or found. Actor won't be able to benefit from this component");
        }

        if (!m_StatusComponent)
        {
            m_StatusComponent = GetComponent<StatusComponent>();
            if (!m_StatusComponent)
                Debug.LogWarning("Actor StatusComponent wasn't successfully set or found. Actor won't be able to benefit from this component");
        }

        if (!m_SpellComponent)
        {
            m_SpellComponent = GetComponent<SpellCastingComponent>();
            if (!m_SpellComponent)
                Debug.LogWarning("Actor SpellCastingComponent wasn't successfully set or found. Actor won't be able to benefit from this component");
        }
    }

    public void PlayerDeath() { Debug.Log("Player Has Died.."); }

    // Update is called once per frame
    void Update()
    {
        m_MovementComponent.Move(Input.GetAxis("Horizontal")*Time.deltaTime, Input.GetAxis("Vertical")*Time.deltaTime);
        if (Input.GetButtonDown("Fire1"))
            Instantiate(m_SpellComponent.m_Spell1.m_Prefab);
    }
}
