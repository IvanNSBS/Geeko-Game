using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHighlightComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] SpriteRenderer m_OutlineHighlight;

    public void ToggleHighlight(bool value)
    {
        m_OutlineHighlight.enabled = value;
    }

    private void Start()
    {
        ToggleHighlight(false);
    }
}
