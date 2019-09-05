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
            if (GetComponent<Animator>()) { // if theres an animation, check if can use it
                float h = m_Animation.animationClips[0].length; // get animation duration
                GetComponent<Animator>().runtimeAnimatorController = m_Animation;
                StartCoroutine(WaitAnimationFinish(h));
            }
    }


    //Wait the animation to finish and destroy the object right after
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
