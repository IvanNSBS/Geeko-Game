using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EffectManagerComponent : MonoBehaviour
{
    // Start is called before the first frame update
    private float m_MoveSpeedMult = 1.0f; // max actor speed

    public void SetSpeedMultiplier(float mult)
    {
        m_MoveSpeedMult = mult;
        m_MoveSpeedMult = Mathf.Clamp(m_MoveSpeedMult, 0.0f, mult);
    }

    public void AddToSpeedMult(float mult)
    {
        m_MoveSpeedMult += mult;
        m_MoveSpeedMult = Mathf.Clamp(m_MoveSpeedMult, 0.0f, 10);
        Debug.Log("MULT = " + m_MoveSpeedMult);
    }

    public void ResetSpeedMultiplier(float mult)
    {
        m_MoveSpeedMult = 1.0f;
    }

    public float GetSpeedMult() { return m_MoveSpeedMult; }
}
