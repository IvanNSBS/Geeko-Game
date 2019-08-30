using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image m_HealthBar;
    [SerializeField] private Text m_HealthText;
    private StatusComponent m_StatusComponent;
    // Update is called once per frame
    private void Start()
    {
        if (!m_StatusComponent)
        {
            m_StatusComponent = GetComponent<StatusComponent>();
            if (!m_StatusComponent)
                Debug.LogWarning("Actor StatusComponent wasn't successfully set or found. Actor won't be able to benefit from this component");

            UpdateHealthBar();
        }
    }

    public void UpdateHealthBar()
    {
        if(m_HealthBar)
            m_HealthBar.fillAmount = m_StatusComponent.GetCurrentHealth() / m_StatusComponent.GetMaxHealth();
        if (m_HealthText)
            m_HealthText.text = ((int)m_StatusComponent.GetCurrentHealth()).ToString() + " / " + ((int)m_StatusComponent.GetMaxHealth()).ToString();
    }

}
