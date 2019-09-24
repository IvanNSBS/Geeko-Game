using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;
public class DungeonManager : MonoBehaviour
{
    public GameObject player;
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
    }
}
