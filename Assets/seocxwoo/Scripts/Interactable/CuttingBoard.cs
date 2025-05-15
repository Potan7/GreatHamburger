using UnityEngine;

public class CuttingBoard : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        Debug.Log("is working");

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
        //Transform plate = transform.Find("plate");
        item.transform.parent = transform;
        item.transform.localPosition = new Vector3(0, 1.15f, 0);
        item.transform.localRotation = Quaternion.identity;
    }
}
