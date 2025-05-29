using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour, IInteractable
{
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
