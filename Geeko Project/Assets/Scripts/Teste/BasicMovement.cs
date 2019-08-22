using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [System.NonSerialized]
    public Transform player;
    public float speed;

    private void Start()
    {
        player = this.transform;
    }

    void Update()
    {
        this.Move();
    }

    public void Move()
    {

        Vector3 Movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

        player.position += Movement * speed * Time.deltaTime;

    }

}
