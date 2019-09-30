using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(StatusComponent))]
public class DestructibleObject : MonoBehaviour
{
    [Header("Stats")]
    public StatusComponent objectStatusComponent;
    public LootManager lootManager;
    [Header("Visuals")]
    [SerializeField] private Animator ObjectAnimator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        objectStatusComponent.AddOnDeath(DestroyObject);
        objectStatusComponent.AddOnTakeDamage(HitObject);
    }

    private void HitObject(float damage, GameplayStatics.DamageType type)
    {
        ObjectAnimator.SetTrigger("Hit");
    }

    private void DestroyObject()
    {
        ObjectAnimator.SetTrigger("Destroyed");
        Destroy(GetComponent<Collider2D>());
        StartCoroutine("FadingSequence");
        DoItBeforeDestroy();
    }

    IEnumerator FadingSequence()
    {
        Sequence fadingSequence = DOTween.Sequence();
        fadingSequence.Append(spriteRenderer.DOFade(1f, 1f));
        fadingSequence.Append(spriteRenderer.DOFade(0f, 2f));
        fadingSequence.Play();
        yield return fadingSequence.WaitForCompletion();
        fadingSequence.Kill();
        Destroy(this.gameObject);
    }

    public virtual void DoItBeforeDestroy()
    {
        if (lootManager)
        {
            lootManager.CalculateLoot();
        }
    }
}
