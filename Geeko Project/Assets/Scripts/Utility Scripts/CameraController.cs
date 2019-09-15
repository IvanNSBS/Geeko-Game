using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    Vector3 moveJump = Vector2.zero;
    [Range(0, 1)]public float speed;
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
        transform.DOMove(targetPos, speed);
        //transform.position = targetPos;
    }
    public void MoveCameraRight()
    {
        Vector3 targetPos = transform.position;
        targetPos += Vector3.right * moveJump.x;
        transform.DOMove(targetPos, speed);
        //transform.position = targetPos;
    }

    public void MoveCameraLeft()
    {
        Vector3 targetPos = transform.position;
        targetPos += Vector3.left * moveJump.x;
        transform.DOMove(targetPos, speed);
        //transform.position = targetPos;
    }

    public void MoveCameraDown()
    {
        Vector3 targetPos = transform.position;
        targetPos += Vector3.down * moveJump.y;
        transform.DOMove(targetPos, speed);
        //transform.position = targetPos;
    }
}
