using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFXManager : MonoBehaviour
{

    [SerializeField] private RuntimeAnimatorController m_Animation;
    // Start is called before the first frame update
    void Start()
    {
        if (m_Animation)
            if (GetComponent<Animator>()) {
                float h = m_Animation.animationClips[0].length;
                GetComponent<Animator>().runtimeAnimatorController = m_Animation;
                StartCoroutine(WaitAnimationFinish(h));
            }
    }

    private IEnumerator WaitAnimationFinish(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
