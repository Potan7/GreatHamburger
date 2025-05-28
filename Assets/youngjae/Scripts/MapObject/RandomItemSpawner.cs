
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MapObject
{
    public class RandomItemSpawner : ItemSpawner
    {
        int lastSpawnedIndex = -1;
        int repeatCount = 0;
        public int maxRepeatCount = 5; // 최대 연속 반복 횟수
        public override async UniTaskVoid SpawnItem()
        {
            await UniTask.Delay(spawnInterval * 1000);

            int ran = Random.Range(0, itemList.Count);
            if (lastSpawnedIndex == ran)
            {
                repeatCount++;
                if (repeatCount >= maxRepeatCount)
                {
                    repeatCount = 0;
                    ran = (ran + 1) % itemList.Count; // 다른 아이템으로 변경
                }
            }
            else
            {
                lastSpawnedIndex = ran; // 새로운 아이템이므로 인덱스 갱신
                repeatCount = 0; // 다른 아이템이므로 반복 횟수 초기화
            }

            var ingredient = Instantiate(itemList[ran], transform.position + Vector3.up * 0.3f, Quaternion.identity, mapObject);
            ingredient.OnIngredientSelected += IngredientSelected;
        }
    }
}
