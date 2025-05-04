using UnityEngine;

public class RobotInteraction : MonoBehaviour
{
    [SerializeField] private GameObject detector;

    public void RobotInteract()
    {
        ObjectDetector objDetector = detector.GetComponent<ObjectDetector>();

        if (objDetector.IsInteractable())
        {
            Debug.Log("상호작용 가능");

            GameObject obj = objDetector.GetInteractableObject();
            IInteractable s = obj.GetComponent<IInteractable>();
            s.Interact(gameObject);
        }
    }

}
