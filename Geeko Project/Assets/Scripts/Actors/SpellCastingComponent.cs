using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCastingComponent : MonoBehaviour
{
    [SerializeField] public Spell m_Spell1;
    [SerializeField] private Spell m_Spell2;
    [SerializeField] private Spell m_Spell3;

    [SerializeField] private GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        if( obj )
            m_Spell1.gameObject = obj;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
