using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUIComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private StatusComponent m_StatusComponent;
    [SerializeField] private Image m_HealthSlider;
    void Start()
    {
        if (!m_StatusComponent)
            Debug.LogWarning("Couldn't get Boss Status Component. UI won't work properly");
        else
            m_StatusComponent.AddOnTakeDamage(UpdateHealth);
    }

    void UpdateHealth(float f, GameplayStatics.DamageType type)
    {
        if (m_HealthSlider && m_StatusComponent)
            m_HealthSlider.fillAmount = m_StatusComponent.GetCurrentHealth() / m_StatusComponent.GetMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
