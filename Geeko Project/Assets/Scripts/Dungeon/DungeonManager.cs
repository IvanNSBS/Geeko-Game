using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;
public class DungeonManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy; //temporary, delete
    private List<Door> doors = new List<Door>();
    private MiniMapCamera miniMapCamera;

    private void Start()
    {
        miniMapCamera = FindObjectOfType<MiniMapCamera>();
    }

    public void SpawnPlayer()
    {
        GameObject m_player = Instantiate(player, Vector3.zero, Quaternion.identity);
        miniMapCamera.SetMinimapImageRef(m_player.GetComponentInChildren<RawImage>());
    }

    public void SpawnEnemy() //temporary, delete
    {
        if (enemy)
        {
            //GameObject e = Instantiate(enemy, Vector3.one, Quaternion.identity);
            //e.GetComponent<StatusComponent>().Die = new UnityEvent();
            //e.GetComponent<StatusComponent>().Die.AddListener(OpenAllDoors);
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
            miniMapCamera.ShowMinimap();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            CloseAllDoors();
            miniMapCamera.HideMinimap();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnEnemy();
        }
    }
}
