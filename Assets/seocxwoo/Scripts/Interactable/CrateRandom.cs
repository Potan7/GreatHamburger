using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CrateRandom : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> ingredientPrefabList = new List<GameObject>();
    private int count = 0;
    private int prevIndex = -1;

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
        
        int index = Random.Range(0, ingredientPrefabList.Count);
        if (count == 4 && index == prevIndex)
        {
            index = (index + 1) % 2;
            count = 0;
        }
        else if (index == prevIndex)
        {
            count++;
        }
        else
        {
            count = 0;
        }
        prevIndex = index;

        GameObject item = Instantiate(ingredientPrefabList[index], hand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Debug.Log($"{interactor.name} picked up {item.name}");
        controller.SetBusy(false);
    }
}
