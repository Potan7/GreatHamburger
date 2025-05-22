using UnityEngine;

public class PlateTable : MonoBehaviour, IInteractable
{
    private float offset = 0f;

    public void Interact(GameObject interactor)
    {
        Debug.Log("Robot°ú PlateTable Interact ½Ãµµ");

        RobotController controller = interactor.GetComponent<RobotController>();
        Transform hand = interactor.transform.Find("Hand");

        if (hand == null)
        {
            Debug.LogWarning("Hand transform not found on interactor.");
            return;
        }

        if (hand.childCount == 0)
        {
            Debug.LogWarning("Robot has no item.");
            return;
        }

        GameObject item = hand.GetChild(0).gameObject;
        Transform plate = transform.Find("plate");
        item.transform.parent = plate;
        item.transform.localPosition = new Vector3(0, offset, 0);
        offset += item.GetComponent<Ingredient>().GetHeight();
        item.transform.localRotation = Quaternion.identity;

        controller.SetBusy(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 localOffset = new Vector3(0f, 0f, 1f);
        Vector3 worldOffset = transform.rotation * localOffset;
        Vector3 pos = transform.position + worldOffset + new Vector3(0, 1f, 0);
        Gizmos.DrawSphere(pos, 0.1f);
    }
}
