using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    public GameObject Enemies;
    public GameObject Enviroment;

    private void Start()
    {
        if(Enviroment)
            Enviroment.SetActive(false);    
    }

    public void HideEnemies()
    {
        Enemies.SetActive(false);
    }

    public void SpawnEnemies()
    {
        Enemies.SetActive(true);
    }

    public void ActivateEnviroment()
    {
        if(Enviroment)
            Enviroment.SetActive(true);
    }
}
