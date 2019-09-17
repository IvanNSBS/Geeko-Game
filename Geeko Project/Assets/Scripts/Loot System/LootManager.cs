using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    [System.Serializable]
    public class Loot
    {
        public GameObject item;
        public int dropRate;
    }

    public List<Loot> lootTable = new List<Loot>();
    public int dropChance;
    
    public void calculateLoot()
    {
        int m_dropChance = Random.Range(0, 101);
        if(m_dropChance > dropChance)
        {
            //Debug.Log("No Loot for me");
            //Destroy(this.gameObject);
            return;
        }
        else
        {
            int itemWeight = 0;

            for (int i = 0; i < lootTable.Count; i++)
                itemWeight += lootTable[i].dropRate;

            int randomValue = Random.Range(0, itemWeight);

            for(int j = 0; j < lootTable.Count; j++)
            {
                if (randomValue <= lootTable[j].dropRate)
                {
                    Instantiate(lootTable[j].item, transform.position, Quaternion.identity);
                    //Destroy(this.gameObject);
                    return;
                }
                randomValue -= lootTable[j].dropRate;
            }
        }
    }
}
