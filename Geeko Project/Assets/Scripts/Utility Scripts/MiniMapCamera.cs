using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class MiniMapCamera : MonoBehaviour
{
    List<RoomInstance> rooms = new List<RoomInstance>();
    List<MapSpriteSelector> minimapSprites = new List<MapSpriteSelector>();
    RoomInstance actualRoom;
    private RawImage minimapImage;
   
    public void GetRoomsReferences()
    {
        rooms = FindObjectsOfType<RoomInstance>().ToList();
    }

    public void GetMinimapReferences()
    {
        minimapSprites = FindObjectsOfType<MapSpriteSelector>().ToList();
    }

    public RoomInstance GetActualRoom()
    {
        return actualRoom;
    }

    public void HideAllMinimapSprites()
    {
        foreach (MapSpriteSelector sprite in minimapSprites)
        {
            sprite.ColorMinimapRoom(0);
        }
    }

    public RoomInstance SearchMapSpritByGridPos(Vector2 gridPos)
    {
        foreach(RoomInstance room in rooms)
        {
            if(room.gridPos == gridPos)
            {
                return room;
            }
        }
        return null;
    }

    public void DiscoverNeighborRooms()
    {
        Vector2 searchGrid;
        if (actualRoom.doorTop)
        {
            searchGrid = new Vector2(actualRoom.gridPos.x, actualRoom.gridPos.y + 1);
            RoomInstance room = SearchMapSpritByGridPos(searchGrid);
            if (room != null && !room.visited)
            {
                room.minimapSprite.ColorMinimapRoom(1);
            }
        }
        if (actualRoom.doorBot)
        {
            searchGrid = new Vector2(actualRoom.gridPos.x, actualRoom.gridPos.y - 1);
            RoomInstance room = SearchMapSpritByGridPos(searchGrid);
            if (room != null && !room.visited)
            {
                room.minimapSprite.ColorMinimapRoom(1);
            }
        }
        if (actualRoom.doorRight)
        {
            searchGrid = new Vector2(actualRoom.gridPos.x + 1, actualRoom.gridPos.y);
            RoomInstance room = SearchMapSpritByGridPos(searchGrid);
            if (room != null && !room.visited)
            {
                room.minimapSprite.ColorMinimapRoom(1);
            }
        }
        if (actualRoom.doorLeft)
        {
            searchGrid = new Vector2(actualRoom.gridPos.x - 1, actualRoom.gridPos.y);
            RoomInstance room = SearchMapSpritByGridPos(searchGrid);
            if (room != null && !room.visited)
            {
                room.minimapSprite.ColorMinimapRoom(1);
            }
        }
    }

    public void UpdateActualRoom(RoomInstance room)
    {
        if(actualRoom != null)
        {
            actualRoom.minimapSprite.ColorMinimapRoom(2);
        }
        actualRoom = room;
        if(actualRoom.visited == false)
        {
            DiscoverNeighborRooms();
            actualRoom.visited = true;
        }
        UpdateCameraPosition();
        room.minimapSprite.ColorMinimapRoom(3);
    }

    void UpdateCameraPosition()
    {
        Vector3 targetPosition = new Vector3(actualRoom.gridPos.x * 16, actualRoom.gridPos.y * 8, -10);
        this.gameObject.transform.position = targetPosition;
    }

    public void SetMinimapImageRef(RawImage Image)
    {
        minimapImage = Image;
    }

    public void HideMinimap()
    {
        minimapImage.DOFade(0f, 0.5f);
    }

    public void ShowMinimap()
    {
        minimapImage.DOFade(1f, 0.5f);
    }
}   
