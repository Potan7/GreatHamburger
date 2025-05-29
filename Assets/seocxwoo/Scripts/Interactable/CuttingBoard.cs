using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private GameObject ingredientPrefab;

    private RobotController robot;
    private Animator animator;
    private Transform hand;

    void Start()
    {
        robot = robotPrefab.GetComponent<RobotController>();
        animator = robotPrefab.GetComponent<Animator>();
        hand = robotPrefab.transform.Find("Hand");
    }

    public IEnumerator Interact()
    {
        Debug.Log("Robot Interact with Crate.");

        if (hand == null)
        {
            Debug.LogWarning("Hand transform not found on interactor.");
            yield return null;
        }

        if (hand.childCount == 0)
        {
            Debug.LogWarning("Robot has no item.");
            animator.SetTrigger("Error");
            yield return null;
        }

        GameObject item = hand.GetChild(0).gameObject;
        Destroy(item);

        animator.SetTrigger("Wait");
        yield return WaitForAnimation(animator, "Wait");

        animator.SetTrigger("PickUp");
        yield return WaitForAnimation(animator, "PickUp");

        item = Instantiate(ingredientPrefab, hand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Debug.Log("picked up");
        robot.SetBusy(false);
    }

    private IEnumerator WaitForAnimation(Animator animator, string stateName)
    {
        // 현재 상태가 원하는 상태가 될 때까지 대기
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }

        // 애니메이션이 끝날 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 localOffset = new Vector3(0f, 0f, 1f);
        Vector3 worldOffset = transform.rotation * localOffset;
        Vector3 pos = transform.position + worldOffset + new Vector3(0, 1f, 0);
        Gizmos.DrawSphere(pos, 0.1f);
    }

    [SerializeField] private List<GameObject> cuttedIngredientList = new List<GameObject>();
    [SerializeField] private GameObject storage;

    public void Interact(GameObject interactor)
    {
        Debug.Log("Robot과 CuttingBoard Interact 시도");

        RobotController controller = interactor.GetComponent<RobotController>();
        Transform hand = interactor.transform.Find("Hand");

        if (hand == null)
        {

            //Animator animator = interactor.GetComponent<Animator>();
            //animator.SetTrigger("Error");

            Debug.LogWarning("Hand transform not found on interactor.");
            return;
        }

        if (hand.childCount == 0)
        {
            Debug.Log("Robot has no item. okay");
        }

        if (hand.childCount > 0)
        {
            GameObject item = hand.GetChild(0).gameObject;
            item.transform.parent = transform;
            item.transform.localPosition = new Vector3(0, 1.15f, 0);
            item.transform.localRotation = Quaternion.identity;
            storage = item;
        }
        else
        {
            GameObject item = storage;
            item.transform.parent = hand;
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            storage = null;

            Debug.Log($"{interactor.name} picked up {item.name}");
            controller.SetBusy(false);
        }
    }

    public void CutIngredient(GameObject interactor)
    {
        Debug.Log("cutIngredient is working");

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

            case "cheese":
                SpawnCuttedIngredient(2);
                break;
        }

        Animator animator = interactor.GetComponent<Animator>();
        animator.SetBool("NeedTime", false);
        // 애니메이터 꼭 필요한지 체크
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
}
