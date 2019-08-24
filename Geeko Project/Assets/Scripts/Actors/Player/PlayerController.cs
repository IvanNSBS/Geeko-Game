using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MovementComponent mMovementComponent;

    void Start()
    {
        if (!mMovementComponent)
        {
            mMovementComponent = GetComponent<MovementComponent>();
            if (!mMovementComponent)
                Debug.LogWarning("Actor MovementComponent wasn't successfully set. Actor won't be able to use this component");
        }
    }

    // Update is called once per frame
    void Update()
    {
        mMovementComponent.Move(Input.GetAxis("Horizontal")*Time.deltaTime, Input.GetAxis("Vertical")*Time.deltaTime);
    }
}
