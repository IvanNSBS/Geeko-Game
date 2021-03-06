﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheetAssigner : MonoBehaviour
{
    [SerializeField]
    Texture2D[] sheetsNormal;
    [SerializeField]
    GameObject RoomObj;
    public Vector2 roomDimensions = new Vector2(16 * 17, 16 * 9);
    public float gutterSizeX = 18;
    public float gutterSizeY = 10;
    public bool finished;
    public void Assign(Room[,] rooms, bool floorHasBoss)
    {
        finished = false;
        foreach (Room room in rooms)
        {
            //skip point where there is no room
            if (room == null)
            {
                continue;
            }
            //pick a random index for the array
            int index = Mathf.RoundToInt(Random.value * (sheetsNormal.Length - 1));
            //find position to place room
            Vector3 pos = new Vector3(room.gridPos.x * gutterSizeX, room.gridPos.y * gutterSizeY, 0);
            RoomInstance myRoom = Instantiate(RoomObj, pos, Quaternion.identity).GetComponent<RoomInstance>();
            myRoom.Setup(sheetsNormal[index], room.gridPos, room.type, room.doorTop, room.doorBot, room.doorLeft, room.doorRight, floorHasBoss, room.minimapSprite);
        }
        finished = true;
    }
}
