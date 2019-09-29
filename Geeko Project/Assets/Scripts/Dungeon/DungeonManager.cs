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

    void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
    }
    
    public void SpawnPlayer()
    {
        miniMapCamera = FindObjectOfType<MiniMapCamera>();
        GameObject m_player = Instantiate(player, Vector3.zero, Quaternion.identity);
        GameObject.DontDestroyOnLoad(m_player);
        miniMapCamera.SetMinimapImageRef(m_player.GetComponentInChildren<RawImage>());
    }

    public void RepositionPlayer()
    {
        player.transform.position = Vector3.zero;
    }

    public void UpdatePlayerReference()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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

    public RoomInstance GetActualRoom()
    {
        return miniMapCamera.GetActualRoom();
    }
}
