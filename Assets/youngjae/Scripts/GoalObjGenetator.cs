using System;
using System.Collections.Generic;
using System.Text;
using MapObject.Ingredients;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[ExecuteInEditMode]
public class GoalObjGenetator : MonoBehaviour
{
    [Header("자식 오브젝트 제거 기능 주의")]
    public PlayerMapManager playerMapManager;
    [Header("playerMapManager에 할당 시 작동 시작")]
    public TextMeshPro goalText;

    Dictionary<string, string> ingredientNames = new Dictionary<string, string>
    {
        { "bun_bottom", "빵" },
        { "bun_top", "빵"},
        { "lettuce_slice", "양상추" },
        { "tomato_slice", "토마토" },
        { "cheese", "치즈" },
        { "patty_cooked", "패티" }
    };

    void Start()
    {
        if (playerMapManager != null && goalText != null)
        {
            goalText.text = GetRecipeByTexts(playerMapManager.answerList);
        }
    }

    void Update()
    {
        if (!Application.isEditor || playerMapManager == null) return;

        var answerList = playerMapManager.answerList;
        if (answerList == null) return;

        bool needsUpdate = false;
        if (answerList.Count != transform.childCount)
        {
            needsUpdate = true;
        }
        else
        {
            for (int i = 0; i < answerList.Count; i++)
            {
                var child = transform.GetChild(i);
                if (answerList[i] == null || child.name != answerList[i].gameObject.name)
                {
                    needsUpdate = true;
                    break;
                }
            }
        }

        if (!needsUpdate)
        {
            return; // 모든 자식의 이름과 수가 일치하면 아무것도 하지 않음
        }

        ResetChildren();

        SetBurger(answerList);
    }

    private void SetBurger(List<GameObject> answerList)
    {
        // answerList의 각 요소를 GameObject로 변환해 생성한 뒤 자식으로 쌓아넣기
        float currentStackHeight = 0f; // 현재까지 쌓인 로컬 Y 높이

        for (int i = 0; i < answerList.Count; i++)
        {
            GameObject prefabIngredient = answerList[i];
            if (prefabIngredient == null || prefabIngredient.gameObject == null)
            {
                Debug.LogWarning($"AnswerList item at index {i} is null or its GameObject is null. Skipping.");
                continue;
            }

            GameObject instance = Instantiate(prefabIngredient, transform);
            instance.name = prefabIngredient.name; // 이름 일치
            instance.layer = LayerMask.NameToLayer("UI");

            // 디스플레이용이므로 물리 및 상호작용 컴포넌트 비활성화/수정
            if (instance.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }

            if (instance.TryGetComponent<XRGrabInteractable>(out var grabInteractable))
            {
                grabInteractable.enabled = false;
            }

            // PlayerIngredient 스크립트 비활성화
            if (instance.TryGetComponent<PlayerIngredient>(out var piScript))
            {
                piScript.enabled = false;
            }

            if (instance.TryGetComponent<Collider>(out var col))
            {
                col.isTrigger = true; // 물리적 충돌 방지
            }

            instance.isStatic = true;

            // 로컬 회전 초기화
            instance.transform.localRotation = Quaternion.identity;

            // 위치 계산 (Plate.cs 방식과 유사하게)
            Renderer itemRenderer = instance.GetComponentInChildren<Renderer>();
            if (itemRenderer != null)
            {
                float itemWorldHeight = itemRenderer.bounds.size.y;
                float parentWorldScaleY = transform.lossyScale.y;

                // 부모 스케일이 0에 가까운 경우 오류 방지
                if (Mathf.Approximately(parentWorldScaleY, 0f))
                {
                    parentWorldScaleY = 1f;
                }

                float heightOfCurrentItemInLocalSpace = itemWorldHeight / parentWorldScaleY;

                instance.transform.localPosition = new Vector3(0, currentStackHeight, 0);
                currentStackHeight += heightOfCurrentItemInLocalSpace;
            }
            else
            {
                // 렌더러가 없는 경우 기본 높이로 쌓거나 경고 로그
                instance.transform.localPosition = new Vector3(0, currentStackHeight, 0);
                currentStackHeight += 0.1f; // 임의의 작은 높이
                Debug.LogWarning($"GameObject {instance.name} in GoalObjGenerator has no renderer, using default stacking height.");
            }
        }

        if (goalText != null)
        {
            goalText.text = GetRecipeByTexts(playerMapManager.answerList);
        }
    }

    private void ResetChildren()
    {
        Debug.Log("Resetting children of GoalObjGenetator...");
        // 자식 오브젝트를 안전하게 제거하기 위해 역순으로 반복하거나 리스트 복사 후 반복
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        goalText.text = string.Empty;
    }

    private string GetRecipeByTexts(List<GameObject> answerList)
    {
        if (answerList == null || answerList.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder recipe = new StringBuilder();

        for (int i = 0; i < answerList.Count; i++)
        {
            if (i == 0)
            {
                recipe.Append(ingredientNames.TryGetValue(answerList[i].name, out var name) ? name : answerList[i].name);
            }
            else
            {
                recipe.Append("-");
                recipe.Append(ingredientNames.TryGetValue(answerList[i].name, out var name) ? name : answerList[i].name);
            }
        }

        return recipe.ToString();
    }

}
