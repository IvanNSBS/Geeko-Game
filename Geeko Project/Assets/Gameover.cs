using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameover : MonoBehaviour
{
    private void DestroyPlayerAndDungeonManager()
    {
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Destroy(FindObjectOfType<DungeonManager>().gameObject);
    }

    public void Retry()
    {
        FindObjectOfType<LoadTargetScene>().LoadSceneNum(2);
        Time.timeScale = 1f;
        DestroyPlayerAndDungeonManager();
    }

    public void BackToMainMenu()
    {
        FindObjectOfType<LoadTargetScene>().LoadSceneNum(0);
        Time.timeScale = 1f;
        DestroyPlayerAndDungeonManager();
    }
}
