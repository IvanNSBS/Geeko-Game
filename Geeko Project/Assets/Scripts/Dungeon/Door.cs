using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    CameraController cm;
    public GameObject gate;
    public int doorType;
    void Start()
    {
        cm = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (doorType)
            {
                case 0:
                    collision.gameObject.transform.position += new Vector3(0f, 3.5f, 0f);
                    cm.MoveCameraUp();
                    break;
                case 1:
                    collision.gameObject.transform.position += new Vector3(0f, -3.5f, 0f);
                    cm.MoveCameraDown();
                    break;
                case 2:
                    collision.gameObject.transform.position += new Vector3(3.6f, 0f, 0f);
                    cm.MoveCameraRight();
                    break;
                case 3:
                    collision.gameObject.transform.position += new Vector3(-3.6f, 0f, 0f);
                    cm.MoveCameraLeft();
                    break;
            }
        }
    }

    public void OpenDoor()
    {
        gate.SetActive(false);
    }

    public void CloseDoor()
    {
        gate.SetActive(true);
    }
}
