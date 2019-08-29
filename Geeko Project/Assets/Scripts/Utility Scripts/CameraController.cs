using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public Vector3 moveJump = Vector2.zero;

    void Start()
    {
        SheetAssigner SA = FindObjectOfType<SheetAssigner>();
        moveJump = new Vector3(SA.gutterSizeX, SA.gutterSizeY, 0);
        Debug.Log(moveJump.x + ", " + moveJump.y);
    }

    public void MoveCameraUp()
    {
        Vector3 targetPos = transform.position;
        targetPos += Vector3.up * moveJump.y;
        transform.position = targetPos;
    }
    public void MoveCameraRight()
    {
        Vector3 targetPos = transform.position;
        targetPos += Vector3.right * moveJump.x;
        transform.position = targetPos;
    }

    public void MoveCameraLeft()
    {
        Vector3 targetPos = transform.position;
        targetPos += Vector3.left * moveJump.x;
        transform.position = targetPos;
    }

    public void MoveCameraDown()
    {
        Vector3 targetPos = transform.position;
        targetPos += Vector3.down * moveJump.y;
        transform.position = targetPos;
    }
}
