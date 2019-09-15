using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [Header("Animation")]
    public Animator spikeAnimator;
    [Header("Stats")]
    public float Damage;
    public float TimeToActivate;
    public float TimeActive;
    private bool isActive;
    private float TimeSinceLastActivation, ActiveTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        spikeAnimator.SetBool("isActive", false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isActive)
        {
            ActiveTimer += Time.deltaTime;
            if (ActiveTimer >= TimeActive)
            {
                DeactivateSpikes();
            }
        } else
        {
            TimeSinceLastActivation += Time.deltaTime;
            if(TimeSinceLastActivation >= TimeToActivate)
            {
                ActivateSpikes();
            }
        }
    }

    private void ActivateSpikes()
    {
        isActive = true;
        spikeAnimator.SetBool("isActive", isActive);
        TimeSinceLastActivation = 0f;
    }

    private void DeactivateSpikes()
    {
        isActive = false;
        spikeAnimator.SetBool("isActive", isActive);
        ActiveTimer = 0f;
    }

    private void OnTriggerStay2D (Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isActive)
        {
            collision.gameObject.GetComponent<StatusComponent>().TakeDamage(Damage);
        }
    }
}
