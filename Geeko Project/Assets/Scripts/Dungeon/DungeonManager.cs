using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy; //temporary, delete
    List<Door> doors = new List<Door>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SpawnPlayer()
    {
        Instantiate(player, Vector3.zero, Quaternion.identity);
    }

    public void SpawnEnemy() //temporary, delete
    {
        if (enemy)
        {
            Instantiate(enemy, Vector3.one, Quaternion.identity);
        }
    }
    
    public void GetDoorsReferences()
    {
        doors = FindObjectsOfType<Door>().ToList();
    }

    public void OpenAllDoors()
    {
        foreach (Door d in doors)
        {
            d.OpenDoor();
        }
    }

    public void CloseAllDoors()
    {
        foreach (Door d in doors)
        {
            d.CloseDoor();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenAllDoors();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            CloseAllDoors();
        }
    }
}
