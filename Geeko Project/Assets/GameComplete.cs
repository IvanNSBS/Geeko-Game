using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComplete : MonoBehaviour
{
    public GameObject Progress;
  
    public void SetGameCompleteActive(bool b)
    {
        Progress.SetActive(b);
        this.gameObject.SetActive(b);
    }
}
