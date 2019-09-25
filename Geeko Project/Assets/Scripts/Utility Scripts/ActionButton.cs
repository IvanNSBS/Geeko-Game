using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{

    private bool isActionButtonPressed = false;
    [SerializeField] private WeaponComponent playerWeaponComponent;

    // Update is called once per frame
    void Update()
    {
        if (isActionButtonPressed)
        {
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
}
