using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{

    public bool GameIsPaused = false;
    public GameObject PauseMenu;
    public GameObject Progress;
    public void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        PauseMenu.SetActive(true);
        Progress.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        PauseMenu.SetActive(false);
        Progress.SetActive(false);
    }

    public void BackToMainMenu()
    {
        FindObjectOfType<LoadTargetScene>().LoadSceneNum(0);
        Time.timeScale = 1f;
        DestroyPlayerAndDungeonManager();
    }

    private void DestroyPlayerAndDungeonManager()
    {
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Destroy(FindObjectOfType<DungeonManager>().gameObject);
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Pause();
        }
    }
}
