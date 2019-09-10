﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private MovementComponent m_MovementComponent;
    private StatusComponent m_StatusComponent;
    private SpellCastingComponent m_SpellComponent;
    private WeaponComponent m_WeaponComponent;
    [SerializeField] private SpriteRenderer m_PlayerHand;
    [SerializeField] private Joystick m_Joystick;
    [SerializeField] private Transform m_FirePoint;
    void Start()
    {
        if (!m_MovementComponent)
        {
            m_MovementComponent = GetComponent<MovementComponent>();
            if (!m_MovementComponent)
                Debug.LogWarning("Actor MovementComponent wasn't successfully set or found. Actor won't be able to benefit from this component");
            else
            {
                m_MovementComponent.m_OnFlip = new UnityEngine.Events.UnityEvent();
                m_MovementComponent.m_OnFlip.AddListener( FlipHand );
            }
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
        
        if (!m_WeaponComponent)
        {
            m_WeaponComponent = GetComponent<WeaponComponent>();
            if (!m_WeaponComponent)
                Debug.LogWarning("Actor WeaponComponent wasn't successfully set or found. Actor won't be able to benefit from this component");
        }
        
        m_WeaponComponent.SetTargetingFunction(() =>
        {
            var vec3 =  m_FirePoint.position - m_PlayerHand.transform.position;
            return new Vector2(vec3.x, vec3.y);
        });
    }

    public void PlayerDeath() { Debug.Log("Player Has Died.."); }
    public void FlipHand()
    {
        if (m_MovementComponent.GetSprite().flipX)
        {
            m_PlayerHand.transform.localPosition = new Vector3(0.66f, -1.303f, 0.0f);
            m_PlayerHand.transform.rotation = Quaternion.Euler(0, 0, 90.0f);
        }
        else
        {
            m_PlayerHand.transform.localPosition = new Vector3(-0.66f, -1.303f, 0.0f);
            m_PlayerHand.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void AutoAim()
    {
        Vector3 pos = this.gameObject.transform.position;
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(pos, 55.0f);

        float min_dist = Mathf.Infinity;
        GameObject last_obj = null;
        foreach(Collider2D overlap in overlaps)
        {
            if (overlap.gameObject.CompareTag("Enemy"))
            {
                Vector2 hit_pos = GameplayStatics.GetTriggerContactPoint(gameObject);
                Vector2 sub = new Vector2(pos.x - hit_pos.x, pos.y - hit_pos.y);
                if (min_dist > sub.magnitude)
                {
                    last_obj = overlap.gameObject;
                    min_dist = sub.magnitude;
                }
            }
        }

        if(last_obj != null)
        {
            Vector2 dir = (last_obj.transform.position - pos).normalized;
            m_MovementComponent.FlipSprite(dir.x);
            Quaternion rot = GameplayStatics.GetRotationFromDir(dir);
            m_PlayerHand.GetComponent<SpriteRenderer>().transform.rotation = rot;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //m_MovementComponent.Move(Input.GetAxis("Horizontal")*Time.deltaTime, Input.GetAxis("Vertical")*Time.deltaTime);
        m_MovementComponent.Move(m_Joystick.Horizontal * Time.deltaTime, m_Joystick.Vertical * Time.deltaTime);


        if (m_Joystick.Horizontal != 0.0f && m_Joystick.Vertical != 0.0f)
        {
            float angle = Mathf.Atan2(m_Joystick.Vertical, m_Joystick.Horizontal) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
            m_PlayerHand.GetComponent<SpriteRenderer>().transform.rotation = rot;
        }

        AutoAim();

        if (Input.GetButton("Fire1"))
        {
            m_WeaponComponent.AttemptToShoot();
            var vec3 =  m_FirePoint.position - m_PlayerHand.transform.position;
            var vec2 = new Vector2(vec3.x, vec3.y);
            m_WeaponComponent.Spiral(vec2, 36, 2, 1);
        }
    }
}
