using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    private bool isInteractable = false;
    private GameObject interactableObject;

    public bool IsInteractable()
    {
        return isInteractable;
    }

    public GameObject GetInteractableObject()
    {
        return interactableObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("°¨Áö");
            isInteractable = true;
            interactableObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isInteractable = false;
            interactableObject = null;
        }
    }
}
