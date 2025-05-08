using UnityEngine;

public class ObjectB : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        Transform hand = interactor.transform.Find("Hand");

        if (hand == null)
        {
            Debug.LogWarning("Hand transform not found on interactor.");
            return;
        }

        if (hand.childCount == 0)
        {
            Debug.Log("No item.");
            return;
        }

        GameObject item = hand.GetChild(0).gameObject;
        Transform slot = transform.Find("Slot");
        item.transform.parent = slot;
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        //Debug.Log($"{interactor.name} picked up {item.name}");
    }
}
