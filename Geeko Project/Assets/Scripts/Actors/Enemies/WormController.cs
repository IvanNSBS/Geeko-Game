using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class WormController : EnemyController
{
    [Header("Worm Properties")] 
    public Animator wormAnimator;
    public Animator dirtFront;
    public Animator dirtBehind;

    public float timeToAppear;
    public float timeToDisappear;
    public float minRandomWalk;
    public float maxRandomWalk;
    public int numberOfBullets;

    private bool _attack=false;
    private WeaponComponent _weaponComponent;
    private Vector3 _randomDirection;
    private float _timeToDisappear=0;
    private Collider2D _collider2D;
    private bool _shooting;

    public override void Start()
    {
        base.Start();
        _weaponComponent = GetComponent<WeaponComponent>();
        _collider2D = GetComponent<Collider2D>();
    }

    public override void StateMachine()
    {
        base.StateMachine();
    }

    public override void CheckTransitions()
    {
        if (currState != EnemyState.Die)
        {
            currState = EnemyState.Wander;
        }

        if (getIdle())
        {
            currState = EnemyState.Idle;
        }else if (_attack)
        {
            currState = EnemyState.Attack;
        }
        
        if (GetCurrentHealth() <= 0)
        {
            currState = EnemyState.Die;
        }

    }
    
    public override void Attack()
    {
        base.Attack();
        if (_timeToDisappear >= timeToDisappear)
        {
            _attack = false;
            _timeToDisappear = 0;
        }
        else
        {
            _timeToDisappear += Time.deltaTime;
        }
    }

    public override GameObject Shooting()
    {
        if (!_shooting)
        {
            print("shooting");
            wormAnimator.SetBool("isAttacking",true);
            wormAnimator.SetBool("isIdle",false);
            _shooting = true;
            _weaponComponent.Linear(-_randomDirection, numberOfBullets, _weaponComponent.cooldown, _weaponComponent.speed);
        }
        return null;
    }

    public override void Reload()
    {
        base.Reload();
        if (_shooting)
        {
            _shooting = false;
        }
    }

    public override void Idle()
    {
        base.Idle();
        if (!GetWaiting() && _attack)
        {
            SetWaiting(true);
            print("idling, attack after");
            wormAnimator.SetBool("isIdle",true);
            StartCoroutine(AttackAfterIdle());
        }
    }

    private IEnumerator AttackAfterIdle()
    {
        yield return new WaitForSeconds(idleTime);
        setIdle(false);
        SetWaiting(false);
    }

    public override void Wander()
    {
        if(!GetWandering())
        {
            SetWandering(true);
            _randomDirection = ChooseTypeOfWalk();
            StartCoroutine(MovingUnderEarth(timeToAppear));
        }
    }

    private IEnumerator MovingUnderEarth(float time)
    {
        print("entering in the ground");
        wormAnimator.SetBool("isIdle",false);
        wormAnimator.SetTrigger("isGoingDown"); //in the animation last frame will disable the components;

        yield return new WaitForSeconds(time);
        
        Move();

        dirtBehind.GetComponent<SpriteRenderer>().enabled = true;
        dirtBehind.SetBool("dirtIdle",false);
        dirtBehind.SetTrigger("dirtBuild");
    }

    public void IdlingAfterUnderEarth()
    {
        wormAnimator.SetBool("isIdle",true);
        setIdle(true); //idling when comeback
        _attack = true; //guarantee the attack after comeback
    }

    public void DisableComponentsWorm()
    {
        _collider2D.enabled = false;
        GetSprite().enabled = false;
        dirtBehind.GetComponent<SpriteRenderer>().enabled = false;
        dirtFront.SetBool("dirtIdle",false);
        dirtFront.SetTrigger("dirtDestroy");
    }

    public void DisableDirtFrontAfterDestroyed()
    {
        dirtFront.SetBool("dirtIdle",true);
        dirtFront.GetComponent<SpriteRenderer>().enabled = false;
        
    }

    public void EnableDirtFrontAfterBuilded()
    {
        dirtFront.GetComponent<SpriteRenderer>().enabled = true;
        dirtBehind.SetBool("dirtIdle",true);
        GetSprite().enabled = true;
        _collider2D.enabled = true;
        wormAnimator.SetTrigger("isGoingUp"); // in the animation last frame will enable idle animation;
    }
    
    private void Move()
    {
        var aux = Random.Range(minRandomWalk, maxRandomWalk);
        var clearDirection = false;
        var layerMask = ~LayerMask.GetMask("Player");
        var interactions = 0;
        while (!clearDirection)
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, _randomDirection, aux+0.5f,layerMask);
            if (!ray)
            {
                clearDirection = true;
            }
            else if (ray.collider.CompareTag("SpellUninteractive"))
            {
                clearDirection = true;
                aux = ray.distance;
            }
            else
            {
                print("[Recalculating] Collided in: "+ray.collider.name+", distance collider = "+ray.distance+", distance ray: "+aux);
                _randomDirection = ChooseTypeOfWalk();
                if (interactions < 5)
                {
                    aux = Random.Range(minRandomWalk, maxRandomWalk);
                }
                else
                {
                    aux = minRandomWalk;
                }
            }

            interactions++;
        }
        transform.position += aux * _randomDirection;
    }

    public void OnDeath()
    {
        wormAnimator.SetBool("isDead",true);
    }
    
}
