using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private int targetSceneIndex;
    // Start is called before the first frame update
    void Start()
    {
        targetSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadTargetScene loadTargetScene = this.gameObject.AddComponent<LoadTargetScene>();
            loadTargetScene.LoadSceneNum(targetSceneIndex);
        }
    }
}
