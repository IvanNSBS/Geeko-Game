using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public Vector3 moveJump = Vector2.zero;
    float horMove, vertMove;

    void Start()
    {
        SheetAssigner SA = FindObjectOfType<SheetAssigner>();
        moveJump = new Vector3(SA.gutterSizeX, SA.gutterSizeY, 0);
        Debug.Log(moveJump.x + ", " + moveJump.y);
    }

    void Update()
    {
        if (Input.GetKeyDown("w") || Input.GetKeyDown("s") ||
            Input.GetKeyDown("a") || Input.GetKeyDown("d")) //if any 'wasd' key is pressed
        {
            horMove = System.Math.Sign(Input.GetAxisRaw("Horizontal"));//capture input
            vertMove = System.Math.Sign(Input.GetAxisRaw("Vertical"));
            Vector3 targetPos = transform.position;
            targetPos += Vector3.right * horMove * moveJump.x; //jump bnetween rooms based opn input
            targetPos += Vector3.up * vertMove * moveJump.y;
            transform.position = targetPos;
        }
    }

}
