using UnityEngine;

public class RobotInteraction : MonoBehaviour
{
    [SerializeField] private GameObject interactableDetector;
    private RobotController controller;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<RobotController>();
        animator = GetComponent<Animator>();
    }

    public void PlayAnim(string interactable)
    {
        controller.SetBusy(true);

        switch(interactable)
        {
            case "Crate":
                animator.SetTrigger("PickUp");
                break;
            case "PlateTable":
                animator.SetBool("HasNextAction", false);
                animator.SetTrigger("PutDown");
                break;
            case "CuttingBoard":
                animator.SetBool("HasNextAction", true);
                animator.SetTrigger("PutDown");
                break;
        }
    }

    public void Interact()
    {
        ObjectDetector detector = interactableDetector.GetComponent<ObjectDetector>();

        // detector가 감지한 오브젝트의 Interact() 실행
        if (detector.IsInteractable())
        {
            GameObject obj = detector.GetInteractable();
            IInteractable interactable = obj.GetComponent<IInteractable>();
            Debug.Log(obj.name);
            interactable.Interact(gameObject);
        }

        controller.SetBusy(false);
    }
}