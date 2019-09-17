using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(0, 500)] float m_TimeToLive;

    void Start()
    {
        if(m_TimeToLive > 0.0f)
            Destroy(this.gameObject, m_TimeToLive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
