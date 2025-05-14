using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public List<GameObject> itemList;

    public int currentItemIndex = 0;

    void Start()
    {
        SpawnItem();
    }

    public void SpawnItem()
    {
        if (currentItemIndex >= itemList.Count)
        {
            return;
        }

        Instantiate(itemList[currentItemIndex], transform.position, Quaternion.identity);
        // item.transform.SetParent(transform);

        currentItemIndex++;
    }
}
