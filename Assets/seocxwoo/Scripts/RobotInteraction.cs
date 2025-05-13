using UnityEngine;

public class RobotInteraction : MonoBehaviour
{
    [SerializeField] private GameObject interactableDetector;

    public void Interact()
    {
        ObjectDetector detector = interactableDetector.GetComponent<ObjectDetector>();

        // detector가 감지한 오브젝트의 Interact() 실행
        if (detector.IsInteractable())
        {
            GameObject obj = detector.GetInteractable();
            IInteractable interactable = obj.GetComponent<IInteractable>();
            interactable.Interact(gameObject);
        }
    }

}