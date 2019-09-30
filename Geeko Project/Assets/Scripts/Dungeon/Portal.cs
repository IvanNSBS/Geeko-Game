using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private int targetSceneIndex;
    private DungeonManager dungeonManager;
    // Start is called before the first frame update
    void Start()
    {
        targetSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        dungeonManager = FindObjectOfType<DungeonManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInChildren<ActionButton>().ChangeAction(EnterPortal);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInChildren<ActionButton>().SwitchToCrossHair();
        }
    }

    public void EnterPortal()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().ProgressPanel.MoveSlime();
        dungeonManager.UpdatePlayerReference();
        FindObjectOfType<ActionButton>().SwitchToCrossHair();
        LoadTargetScene loadTargetScene = this.gameObject.AddComponent<LoadTargetScene>();
        loadTargetScene.LoadSceneNum(targetSceneIndex);
        
    }
}
