using System;
using System.Collections.Generic;
using Data;
using MapObject.Ingredients;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MapObject
{

    public class Plate : MonoBehaviour
    {
        public List<PlayerIngredient> stackedIngredients = new();
        // private BoxCollider plateCollider;

        [SerializeField]
        float plateHeight = 0f;  // 높이 조정

        void OnCollisionEnter(Collision collision)
        {
            // Debug.Log($"Plate collided with {collision.gameObject.name}");
            if (collision.gameObject.TryGetComponent<PlayerIngredient>(out var ingredient))
            {
                StackIngredient(ingredient);
            }
        }
        void StackIngredient(PlayerIngredient ingredient)
        {
            // 이미 쌓인 재료인지 확인 (중복 스택 방지)
            if (stackedIngredients.Contains(ingredient))
            {
                return;
            }
            Debug.Log("Stacking ingredient: " + ingredient.name);

            ingredient.rb.constraints = RigidbodyConstraints.FreezeAll; // 모든 움직임을 고정
            ingredient.interactable.enabled = false;
            ingredient.GetComponent<Collider>().isTrigger = true; // 충돌을 트리거로 설정하여 물리적 상호작용 방지

            ingredient.OnIngredientCollision += StackIngredient;

            ingredient.transform.SetParent(transform, true); // worldPositionStays = true로 설정하여 현재 월드 상태 유지 후 부모 지정

            // 재료의 위치 조정
            Quaternion rotation = Quaternion.Euler(0, 0, 0); // 필요에 따라 로컬 회전값 조정 가능
            ingredient.transform.localRotation = rotation;
            if (ingredient.TryGetComponent<Renderer>(out var ingredientRenderer))
            {
                // 1. 재료의 월드 공간에서의 실제 높이를 가져옵니다.
                // ingredientRenderer.bounds.size.y는 이미 월드 스케일을 포함한 높이입니다.
                float ingredientWorldHeight = ingredientRenderer.bounds.size.y;

                // 2. 부모 객체(this.transform)의 Y축 월드 스케일을 가져옵니다.
                float parentWorldScaleY = transform.lossyScale.y;
                if (Mathf.Approximately(parentWorldScaleY, 0)) // 부모의 Y스케일이 0이면 나누기 오류 방지
                {
                    Debug.LogError("Parent object has a Y scale of 0! Cannot stack correctly.");
                    return;
                }

                // 3. 재료의 월드 높이를 부모의 로컬 공간 높이로 변환합니다.
                // plateHeight는 부모의 로컬 공간을 기준으로 누적되므로, 더해줄 높이도 로컬 공간 기준이어야 합니다.
                float heightToAddInLocalSpace = ingredientWorldHeight / parentWorldScaleY;

                Vector3 newLocalPosition = new Vector3(
                    0,
                    plateHeight, // 현재 plateHeight가 새 재료의 바닥 Y 좌표가 됨
                    0
                );

                // 다음 재료가 쌓일 기준 높이를 업데이트합니다. (이 부분은 변경 없음)
                plateHeight += heightToAddInLocalSpace;
                Debug.Log($"  7. plateHeight (after this ingredient): {this.plateHeight}");

                ingredient.transform.localPosition = newLocalPosition;
                // ingredient.SetLocalPositionAndRotation(newLocalPosition, targetLocalRotation); // 이렇게 사용해도 동일

                FindAnyObjectByType<DescriptionPanel>().JobComplete(WaitJob.WaitForPlating);
            }

            stackedIngredients.Add(ingredient);
            PlayerMapManager.Instance.OnAddedItemToPlate(stackedIngredients);
            
            // bool trySpawnItem = FindFirstObjectByType<ItemSpawner>().SpawnItem();
            // if (!trySpawnItem)
            // {
            //     bool isCorrect = true;
            //     for (int i = 0; i < answer.Length; i++)
            //     {
            //         if (!stackedIngredients[i].name.Contains(answer[i]))
            //         {
            //             isCorrect = false;
            //             Debug.Log($"Incorrect ingredient: {stackedIngredients[i].name} does not contain {answer[i]}");
            //             break;
            //         }
            //     }

            //     if (isCorrect)
            //     {
            //         answerObject.SetActive(true);
            //         Debug.Log("Correct! All ingredients stacked.");
            //     }
            //     else
            //     {
            //         Debug.Log("Incorrect! Try again.");
            //     }

            // }
        }
    }
}