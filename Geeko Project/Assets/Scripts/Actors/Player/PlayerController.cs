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
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private float AutoAimRange = 12f;
    [HideInInspector] public GameObject target;
    public Progress ProgressPanel;

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
            else
            {
                m_StatusComponent.AddOnDeath(PlayerDeath);
                m_StatusComponent.AddOnTakeDamage(MakePlayerInvulnerable);
            }
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
            if (!target)
            {
                var vec3 = m_FirePoint.position - m_PlayerHand.transform.position;
                return new Vector2(vec3.x, vec3.y);
            }
            else
                return target.transform.position - m_FirePoint.position;

        });

        if (!m_EffectManager)
        {
            m_EffectManager = GetComponent<EffectManagerComponent>();
            if (!m_EffectManager)
                Debug.LogWarning("Actor EffectManagerComponent wasn't successfully set or found. Actor won't be able to benefit from this component");
        }
    }

    public void PlayerDeath()
    {
        Debug.Log("Player Has Died..");
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        foreach (SpriteRenderer sp  in this.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            sp.enabled = false;
        }
        Time.timeScale = 0f;
        GameOverPanel.SetActive(true);
        ProgressPanel.gameObject.SetActive(true);
    }

    public void FlipHand()
    {
        if (!m_MovementComponent.GetSprite().flipX)
        {
            m_PlayerHand.transform.localPosition = new Vector3(0.66f, -1.303f, 0.0f);
            m_PlayerHand.transform.rotation = Quaternion.Euler(0, 0, 180.0f);
        }
        else
        {
            m_PlayerHand.transform.localPosition = new Vector3(-0.66f, -1.303f, 0.0f);
            m_PlayerHand.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void MakePlayerInvulnerable(float useless, GameplayStatics.DamageType type)
    {
        StartCoroutine(GameplayStatics.Delay(m_StatusComponent.m_IFrameTime, () => this.gameObject.layer = LayerMask.NameToLayer("PlayerInvulnerable"), () => this.gameObject.layer = LayerMask.NameToLayer("Player")));
    }

    public void AutoAim()
    {
        Vector3 pos = this.gameObject.transform.position;
        float thresh = 0.7f;

        Collider2D[] overlaps = Physics2D.OverlapCircleAll(pos, AutoAimRange);

        float min_dist = Mathf.Infinity;
        GameObject old_target = target;
        target = null;
        foreach(Collider2D overlap in overlaps)
        {
            if (overlap.gameObject.CompareTag("Enemy"))
            {
                Vector2 hit_pos = new Vector2(overlap.gameObject.transform.position.x, overlap.gameObject.transform.position.y);
                Vector2 sub = new Vector2(pos.x - hit_pos.x, pos.y - hit_pos.y);
                if (min_dist > sub.magnitude + thresh)
                {
                    target = overlap.gameObject;
                    min_dist = sub.magnitude;
                }
            }
        }

        if(target != null)
        {
            // Vector3 target_center = target.GetComponent<Collider2D>().bounds.center;
            Vector3 target_center = target.transform.position;
            Vector2 dir = (target_center - gameObject.transform.position).normalized;
            //Vector2 dir = (target_center - m_FirePoint.transform.position).normalized;
            m_MovementComponent.FlipSprite(dir.x);
            Quaternion rot = GameplayStatics.GetRotationFromDir(dir);
            m_PlayerHand.GetComponent<SpriteRenderer>().transform.rotation = rot;

            target.GetComponentInChildren<TargetHighlightComponent>().ToggleHighlight(true);
            if (old_target)
                if (old_target != target) {
                    var shadow = old_target.GetComponentInChildren<TargetHighlightComponent>();
                    if(shadow)
                        shadow.ToggleHighlight(false);
                }
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
        else
            m_MovementComponent.Move(Input.GetAxis("Horizontal")*Time.deltaTime * m_EffectManager.GetSpeedMult(), Input.GetAxis("Vertical")*Time.deltaTime * m_EffectManager.GetSpeedMult());

        AutoAim();
        
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //    m_WeaponComponent.AttemptToShoot();
        if (Input.GetKeyDown(KeyCode.Alpha1))
            m_SpellComponent.CastSpell(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            m_SpellComponent.CastSpell(1);
    }
}
