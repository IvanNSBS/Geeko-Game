using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    public GameObject mapRoot;
    public GameObject mapCamera;
    
    void Start()
    {
        float posX = mapRoot.transform.position.x;
        float posY = mapRoot.transform.position.y;
        AdjustCameraPosition(posX, posY);
    }

    void AdjustCameraPosition(float x, float y)
    {
        mapCamera.transform.Translate(x, y, -10);
    }
}
