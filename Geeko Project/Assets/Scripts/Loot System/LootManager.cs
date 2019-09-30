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

    public bool shouldDropSpell = false;
    public GameObject SpellDropPrefab;
    public List<Loot> lootTable = new List<Loot>();
    public int dropChance;
    
    public void CalculateLoot()
    {
        int m_dropChance = Random.Range(0, 101);
        if(m_dropChance > dropChance)
        {
            //Debug.Log("No Loot for me");
            //Destroy(this.gameObject);
            if (shouldDropSpell)
            {
                if (SpellDropPrefab)
                {
                    GameObject spell = Instantiate(SpellDropPrefab, transform.position, Quaternion.identity);
                    spell.transform.parent = this.transform.parent;
                }
            }
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
                    GameObject go = Instantiate(lootTable[j].item, transform.position, Quaternion.identity);
                    go.transform.parent = this.transform.parent;
                    if (shouldDropSpell)
                    {
                        if (SpellDropPrefab)
                        {
                            GameObject spell = Instantiate(SpellDropPrefab, transform.position, Quaternion.identity);
                            spell.transform.parent = this.transform.parent;
                        }
                    }
                    return;
                }
                randomValue -= lootTable[j].dropRate;
            }
        }
    }
}
