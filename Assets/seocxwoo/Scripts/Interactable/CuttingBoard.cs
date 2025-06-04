using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private List<GameObject> cuttedIngredientList = new List<GameObject>();
    [SerializeField] private GameObject storage;

    private RobotController robot;
    private Animator animator;
    private Transform hand;

    void Awake()
    {
        robot = robotPrefab.GetComponent<RobotController>();
        animator = robotPrefab.GetComponent<Animator>();
        hand = robotPrefab.transform.Find("Hand");
    }

    public IEnumerator Interact()
    {
        Debug.Log("Robot Interact with CuttingBoard.");

        // Hand에 내려놓을 아이템이 없음 (에러 발생)
        if (hand.childCount == 0)
        {
            Debug.LogWarning("Robot has no item.");

            // 에러 애니메이션 실행 후 종료
            robot.SetBusy(true);
            animator.SetTrigger("Error");
            yield return new WaitForSeconds(1.0f);
            robot.SetBusy(false);

            yield break;
        }

        GameObject item = hand.GetChild(0).gameObject;

        // CuttingBoard에 내려놓을 아이템이 적합하지 않음 (에러 발생)
        if (!IsSuitable(item.name))
        {
            Debug.LogWarning("Item is not suitable.");

            // 에러 애니메이션 실행 후 종료
            robot.SetBusy(true);
            animator.SetTrigger("Error");
            yield return new WaitForSeconds(1.0f);
            robot.SetBusy(false);

            yield break;
        }

        // CuttingBoard 위에 아이템 놓기
        item = hand.GetChild(0).gameObject;
        item.transform.parent = transform;
        item.transform.localPosition = new Vector3(0, 1.15f, 0);
        item.transform.localRotation = Quaternion.identity;
        storage = item;

        // CuttingBoard에 재료 내려놓기->썰기->줍기
        robot.SetBusy(true);
        animator.SetInteger("NextAction", 1);
        animator.SetTrigger("PutDown");
        yield return new WaitForSeconds(2.0f);
        CutIngredient();
        animator.SetTrigger("PickUp");
        yield return new WaitForSeconds(1.0f);
        item = storage;
        item.transform.parent = hand;
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        storage = null;
        robot.SetBusy(false);
    }

    private bool IsSuitable(string name)
    {
        if (name == "food_ingredient_lettuce(Clone)" || name == "food_ingredient_tomato(Clone)")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CutIngredient()
    {
        Ingredient ingredient = storage.GetComponent<Ingredient>();
        string name = ingredient.GetName();
        
        switch(name)
        {
            case "lettuce":
                SpawnCuttedIngredient(0);
                break;

            case "tomato":
                SpawnCuttedIngredient(1);
                break;
        }
    }

    private void SpawnCuttedIngredient(int index)
    {
        GameObject cutted = Instantiate(cuttedIngredientList[index]);
        cutted.transform.parent = transform;
        cutted.transform.localPosition = new Vector3(0, 1.15f, 0);
        cutted.transform.localRotation = Quaternion.identity;
        Destroy(storage);
        storage = cutted;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 localOffset = new Vector3(0f, 0f, 1f);
        Vector3 worldOffset = transform.rotation * localOffset;
        Vector3 pos = transform.position + worldOffset + new Vector3(0, 1f, 0);
        Gizmos.DrawSphere(pos, 0.1f);
    }
}
