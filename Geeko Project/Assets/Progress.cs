using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    public Image[] playerPositions;
    public Image Slime;
    private int currentPlayerPosition;
    void Start()
    {
        //currentPlayerPosition = 0;
        Slime.transform.position = playerPositions[currentPlayerPosition].transform.position;
    }

    public void ActivateProgress()
    {
        this.gameObject.SetActive(true);
    }

    public void MoveSlime()
    {
        currentPlayerPosition++;
        Slime.transform.position = playerPositions[currentPlayerPosition].transform.position;
    }
}
