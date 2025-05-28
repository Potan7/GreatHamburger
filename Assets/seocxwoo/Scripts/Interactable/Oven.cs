using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject cookedBurger;

    public void Interact(GameObject interactor)
    {
        Debug.Log("Robot°ú Oven Interact ½Ãµµ");

        RobotController controller = interactor.GetComponent<RobotController>();
        Transform hand = interactor.transform.Find("Hand");

        if (hand == null)
        {
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
            Destroy(item);
        }
        else
        {
            GameObject item = Instantiate(cookedBurger);
            item.transform.parent = hand;
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;

            Debug.Log($"{interactor.name} picked up {item.name}");
            controller.SetBusy(false);
        }
    }

    public void WaitForBurger(GameObject interactor)
    {
        Debug.Log("wait is working");

        Animator animator = interactor.GetComponent<Animator>();
        animator.SetBool("NeedTime", false);
    }
}
