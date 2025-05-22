using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> outputList = new List<GameObject>();

    public void Interact(GameObject interactor)
    {
        Debug.Log("Robot과 Crate Interact 시도");

        RobotController controller = interactor.GetComponent<RobotController>();
        Transform hand = interactor.transform.Find("Hand");

        if (hand == null)
        {
            Debug.LogWarning("Hand not found on Robot.");
            return;
        }

        // 기존 아이템이 있으면 줍지 않음 (옵션)
        if (hand.childCount > 0)
        {
            Debug.LogWarning("Robot Already holding an item.");
            // 여기서 다른 처리 필요
            return;
        }
        
        GameObject item = Instantiate(outputList[0], hand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        outputList.RemoveAt(0);

        Debug.Log($"{interactor.name} picked up {item.name}");
        controller.SetBusy(false);
    }
}
