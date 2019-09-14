using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private MovementComponent m_MovementComponent;
    private StatusComponent m_StatusComponent;
    private SpellCastingComponent m_SpellComponent;
    private EffectManagerComponent m_EffectManager;
    private WeaponComponent m_WeaponComponent;
    [SerializeField] private SpriteRenderer m_PlayerHand;
    [SerializeField] private Joystick m_Joystick;
    [SerializeField] private Transform m_FirePoint;

    [HideInInspector] public GameObject target;

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

        if (!m_EffectManager)
        {
            m_EffectManager = GetComponent<EffectManagerComponent>();
            if (!m_EffectManager)
                Debug.LogWarning("Actor EffectManagerComponent wasn't successfully set or found. Actor won't be able to benefit from this component");
        }
    }

    public void PlayerDeath() { Debug.Log("Player Has Died.."); }
    public void FlipHand()
    {
        if (!m_MovementComponent.GetSprite().flipX)
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
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(pos, 7.0f);

        float min_dist = Mathf.Infinity;
        target = null;
        foreach(Collider2D overlap in overlaps)
        {
            if (overlap.gameObject.CompareTag("Enemy"))
            {
               // Debug.Log("Object is: " + overlap.gameObject);
                Vector2 hit_pos = new Vector2(overlap.gameObject.transform.position.x, overlap.gameObject.transform.position.y);
                Vector2 sub = new Vector2(pos.x - hit_pos.x, pos.y - hit_pos.y);
                if (min_dist > sub.magnitude)
                {
                    target = overlap.gameObject;
                    min_dist = sub.magnitude;
                }
            }
        }

        if(target != null)
        {
            Vector2 dir = (target.transform.position - pos).normalized;
            m_MovementComponent.FlipSprite(dir.x);
            Quaternion rot = GameplayStatics.GetRotationFromDir(dir);
            m_PlayerHand.GetComponent<SpriteRenderer>().transform.rotation = rot;
        }

        m_SpellComponent.SetTarget(target);
    }

    // Update is called once per frame
    void Update()
    {
        //m_MovementComponent.Move(Input.GetAxis("Horizontal")*Time.deltaTime, Input.GetAxis("Vertical")*Time.deltaTime);
        m_MovementComponent.Move(m_Joystick.Horizontal * Time.deltaTime * m_EffectManager.GetSpeedMult(), m_Joystick.Vertical * Time.deltaTime * m_EffectManager.GetSpeedMult());


        if (m_Joystick.Horizontal != 0.0f && m_Joystick.Vertical != 0.0f)
        {
            float angle = Mathf.Atan2(m_Joystick.Vertical, m_Joystick.Horizontal) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
            m_PlayerHand.GetComponent<SpriteRenderer>().transform.rotation = rot;
        }

        AutoAim();
/*
        if (Input.GetButton("Fire1"))
        {
            m_WeaponComponent.AttemptToShoot();
            var vec3 =  m_FirePoint.position - m_PlayerHand.transform.position;
            var vec2 = new Vector2(vec3.x, vec3.y);
             m_WeaponComponent.Spiral(vec2, 36, 2, 1, m_WeaponComponent.speed);
            var tm = new TargetingManager()
                .InitStartingDirection(vec2)
                .RandomizeGauss(5)
                .Spiral(2)
                .Sine(20, 20)
                .Offset(180);
            m_WeaponComponent.GenericStream(tm, 100, 0.5f, m_WeaponComponent.speed);
            
            
        }
         */
    }
}
