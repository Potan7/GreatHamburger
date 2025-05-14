using System.Collections.Generic;
using UnityEngine;

namespace MapObject
{
    public class ItemSpawner : MonoBehaviour
    {
        public List<GameObject> itemList;

        public Transform mapObject;

        public int currentItemIndex = 0;

        void Start()
        {
            SpawnItem();
        }

        public bool SpawnItem()
        {
            if (currentItemIndex >= itemList.Count)
            {
                return false;
            }

            Instantiate(itemList[currentItemIndex], transform.position + Vector3.up * 0.3f, Quaternion.identity, mapObject);
            // item.transform.SetParent(transform);

            currentItemIndex++;
            return true;
        }
    }
}