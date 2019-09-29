using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomInstance : MonoBehaviour
{
    public Texture2D tex;
    public Vector2 gridPos;
    public int type; //0: initial room, 1: normal room, 2: miniboss, 3: boss
    public bool visited;
    public MapSpriteSelector minimapSprite;

    private float tileSize = 1;
    private Vector2 roomSizeInTiles = new Vector2(9, 17);
    private MiniMapCamera minimapCam;
    private DungeonManager dungeonManager;
    private int enemiesInThisRoom;
    private bool hasEnemyInThisRoom, floorHasBoss;
    private Encounter encounter;
    [HideInInspector] public bool doorTop, doorBot, doorLeft, doorRight;
    [SerializeField] private ColorToGameObject[] mappings;
    [SerializeField] private GameObject doorU, doorD, doorL, doorR, wallU, wallD, wallL, wallR;
    [SerializeField] private List<Encounter> Encounters = new List<Encounter>();
    [SerializeField] private List<int> EncounterRate = new List<int>();
    [SerializeField] private List<Encounter> SpecialEncounters = new List<Encounter>();

    private void Start()
    {
        dungeonManager = FindObjectOfType<DungeonManager>();
        SetupEncounter();
    }

    public void Setup(Texture2D _tex, Vector2 _gridPos, int _type, bool _doorTop, bool _doorBot, bool _doorLeft, bool _doorRight, bool _floorHasBoss, MapSpriteSelector _minimapSprite)
    {
        tex = _tex;
        gridPos = _gridPos;
        type = _type;
        doorTop = _doorTop;
        doorBot = _doorBot;
        doorLeft = _doorLeft;
        doorRight = _doorRight;
        floorHasBoss = _floorHasBoss;
        minimapSprite = _minimapSprite;
        minimapCam = FindObjectOfType<MiniMapCamera>();
        enemiesInThisRoom = 0;
        MakeDoors();
        GenerateRoomTiles();
    }

    void MakeDoors()
    {
        //top door, get position then spawn
        Vector3 spawnPos = transform.position + Vector3.up * (roomSizeInTiles.y / 4 * tileSize) - Vector3.up * (tileSize / 4);
        PlaceDoor(spawnPos, doorTop, doorU, "U");
        //bottom door
        spawnPos = transform.position + Vector3.down * (roomSizeInTiles.y / 4 * tileSize) - Vector3.down * (tileSize / 4);
        PlaceDoor(spawnPos, doorBot, doorD, "D");
        //right door
        spawnPos = transform.position + Vector3.right * (roomSizeInTiles.x * tileSize) - Vector3.right * (tileSize);
        PlaceDoor(spawnPos, doorRight, doorR, "R");
        //left door
        spawnPos = transform.position + Vector3.left * (roomSizeInTiles.x * tileSize) - Vector3.left * (tileSize);
        PlaceDoor(spawnPos, doorLeft, doorL, "L");
    }

    void PlaceDoor(Vector3 spawnPos, bool door, GameObject doorSpawn, string direction)
    {
        // check whether its a door or wall, then spawn
        if (door)
        {
            Instantiate(doorSpawn, spawnPos, Quaternion.identity).transform.parent = transform;
        }
        else
        {
            switch (direction)
            {
                case "U":
                    Instantiate(wallU, spawnPos, Quaternion.identity).transform.parent = transform;
                    break;
                case "D":
                    Instantiate(wallD, spawnPos, Quaternion.identity).transform.parent = transform;
                    break;
                case "R":
                    Instantiate(wallR, spawnPos, Quaternion.identity).transform.parent = transform;
                    break;
                case "L":
                    Instantiate(wallL, spawnPos, Quaternion.identity).transform.parent = transform;
                    break;
            }
        }
    }

    void GenerateRoomTiles()
    {
        //loop through every pixel of the texture
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                GenerateTile(x, y);
            }
        }
    }
    void GenerateTile(int x, int y)
    {
        Color pixelColor = tex.GetPixel(x, y);
        //skip clear spaces in texture
        if (pixelColor.a == 0)
        {
            return;
        }
        //find the color to math the pixel
        foreach (ColorToGameObject mapping in mappings)
        {
            if (mapping.color.Equals(pixelColor))
            {
                Vector3 spawnPos = positionFromTileGrid(x, y);
                Instantiate(mapping.prefab, spawnPos, Quaternion.identity).transform.parent = this.transform;
            }
        }
    }
    Vector3 positionFromTileGrid(int x, int y)
    {
        Vector3 ret;
        //find difference between the corner of the texture and the center of this object
        Vector3 offset = new Vector3((-roomSizeInTiles.x + 1) * tileSize, (roomSizeInTiles.y / 4) * tileSize - (tileSize / 4), 0);
        //find scaled up position at the offset
        ret = new Vector3(tileSize * (float)x, -tileSize * (float)y, 0) + offset + transform.position;
        return ret;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            minimapCam.UpdateActualRoom(this);
            if (hasEnemyInThisRoom)
            {
                SpawnEnemies();
                minimapCam.HideMinimap();
            }
        }
    }
     /*
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            checkEnemies();
        }
    }
    */
    private void SetupEncounter()
    {
        switch(type){

            case 0:
                hasEnemyInThisRoom = false;
                break;
            case 1:
                hasEnemyInThisRoom = true;
                int random = Random.Range(1, 100);
                int spawn = -1;
                while(random >= 0)
                {
                    spawn++;
                    random -= EncounterRate[spawn];
                }
                encounter = Instantiate(Encounters[spawn], this.transform.position, Quaternion.identity).GetComponent<Encounter>();
                encounter.transform.parent = this.gameObject.transform;
                encounter.HideEnemies();
                break;
            case 2:
                hasEnemyInThisRoom = true;
                encounter = Instantiate(SpecialEncounters[0], this.transform.position, Quaternion.identity).GetComponent<Encounter>();
                encounter.HideEnemies();
                break;
        }
    }

    private void checkEnemies()
    {
        enemiesInThisRoom--;
        if (enemiesInThisRoom <= 0 && encounter)
        {
            hasEnemyInThisRoom = false;
            //Destroy(encounter.Enemies);
            dungeonManager.OpenAllDoors();
            minimapCam.ShowMinimap();
            if(type == 2 && !floorHasBoss)
            {
                encounter.ActivateEnviroment();
            }
        }
    }
    
    private void SpawnEnemies()
    {
        if (encounter && hasEnemyInThisRoom)
        {
            encounter.SpawnEnemies();
            enemiesInThisRoom = encounter.Enemies.GetComponentsInChildren<StatusComponent>().Length;
            foreach (StatusComponent status in encounter.Enemies.GetComponentsInChildren<StatusComponent>())
            {
                status.AddOnDeath(checkEnemies);
            }
            if(type != 2)
                encounter.ActivateEnviroment();
            dungeonManager.CloseAllDoors();
        }
    }
}
