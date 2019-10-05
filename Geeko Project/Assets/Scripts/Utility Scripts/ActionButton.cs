using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private GameObject crosshair, pointer;
    [SerializeField] private Button button;
    private bool shoot = true;
    private bool isActionButtonPressed = false;
    [SerializeField] private WeaponComponent playerWeaponComponent;
    [SerializeField] private AudioClip m_ShootSound;

    // Update is called once per frame
    void Update()
    {
        if (isActionButtonPressed)
        {
            if (shoot) 
                playerWeaponComponent.AttemptToShoot();
        }
    }

    public void onPointerDownActionButton()
    {
        isActionButtonPressed = true;
    }

    public void onPointerUpActionButton()
    {
        isActionButtonPressed = false;
    }

    private void SwitchToPointer()
    {
        shoot = false;
        crosshair.SetActive(false);
        pointer.SetActive(true);
    }

    public void SwitchToCrossHair()
    {
        button.onClick.RemoveAllListeners();
        shoot = true;
        pointer.SetActive(false);
        crosshair.SetActive(true);
    }

    public void ChangeAction(UnityAction action)
    {
        isActionButtonPressed = false;
        SwitchToPointer();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
