using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonManager : MonoBehaviour
{
    List<Door> doors = new List<Door>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            doors = FindObjectsOfType<Door>().ToList();
            foreach (Door d in doors)
            {
                d.OpenDoor();
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach(Door d in doors)
            {
                d.CloseDoor();
            }
        }
    }
}
