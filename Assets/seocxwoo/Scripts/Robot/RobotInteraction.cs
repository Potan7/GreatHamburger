using UnityEngine;

public class RobotInteraction : MonoBehaviour
{
    [SerializeField] private GameObject interactableDetector;
    private RobotController controller;
    private ObjectDetector detector;

    void Awake()
    {
        controller = GetComponent<RobotController>();
        detector = interactableDetector.GetComponent<ObjectDetector>();
    }

    public void Interact()
    {
        // detector가 감지한 오브젝트의 Interact() 실행
        if (detector.IsInteractable())
        {
            GameObject obj = detector.GetInteractable();
            IInteractable interactable = obj.GetComponent<IInteractable>();

            StartCoroutine(interactable.Interact());
        }
    }
}