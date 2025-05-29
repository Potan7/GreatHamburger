using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MapObject.Ingredients;
using UnityEngine;

namespace MapObject
{
    public class ItemSpawner : MonoBehaviour
    {
        [Tooltip("스폰될 아이템 목록 (순서대로 스폰됨)")]
        public List<PlayerIngredient> itemList;

        public Transform mapObject;

        public int currentItemIndex = 0;
        public int spawnInterval = 3; // 스폰 간격 (초 단위)

        void Start()
        {
            SpawnItem().Forget();
        }

        protected void IngredientSelected(PlayerIngredient ingredient)
        {
            // Debug.Log("Selected: " + ingredient.name);
            ingredient.OnIngredientSelected -= IngredientSelected;
            SpawnItem().Forget();
        }

        public virtual async UniTaskVoid SpawnItem()
        {
            await UniTask.Delay(spawnInterval * 1000);

            var ingredient = Instantiate(itemList[currentItemIndex], transform.position + Vector3.up * 0.3f, Quaternion.identity, mapObject);
            ingredient.OnIngredientSelected += IngredientSelected;

            currentItemIndex++;
            currentItemIndex %= itemList.Count;
        }
    }
}