using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    private bool isInteractable = false;
    private GameObject interactable;

    public bool IsInteractable()
    {
        return isInteractable;
    }

    public GameObject GetInteractable()
    {
        return interactable;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            isInteractable = true;
            interactable = other.gameObject;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            isInteractable = true;
            interactable = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            isInteractable = false;
            interactable = null;
        }
    }
}
