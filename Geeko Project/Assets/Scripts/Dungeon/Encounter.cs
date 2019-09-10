using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    public GameObject Enemies;

    public void HideEnemies()
    {
        Enemies.SetActive(false);
    }

    public void SpawnEnemies()
    {
        Enemies.SetActive(true);
    }
}
