using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirt : MonoBehaviour
{
    private WormController worm;
    void Start()
    {
        worm = transform.parent.GetComponent<WormController>();
    }
    
    public void DisableDirtFrontAfterDestroyed()
    {
        worm.DisableDirtFrontAfterDestroyed();
    }

    public void EnableDirtFrontAfterBuilded()
    {
        worm.EnableDirtFrontAfterBuilded();
    }
    
    
}
