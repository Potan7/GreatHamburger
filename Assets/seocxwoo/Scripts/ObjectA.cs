using UnityEngine;

public class ObjectA : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject itemPrefab;

    public void Interact(GameObject interactor)
    {
        Debug.Log(interactor.name);
        Transform hand = interactor.transform.Find("Hand");

        if (hand == null)
        {
            Debug.LogWarning("Hand transform not found on interactor.");
            return;
        }

        // 기존 아이템이 있으면 줍지 않음 (옵션)
        if (hand.childCount > 0)
        {
            Debug.Log("Already holding an item.");
            return;
        }

        GameObject item = Instantiate(itemPrefab, hand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Debug.Log($"{interactor.name} picked up {item.name}");
    }
}
