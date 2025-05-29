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
        animator.SetBool("NeedTime", true);

        switch(interactable)
        {
            case "Crate_Buns":
                animator.SetTrigger("PickUp");
                break;
            case "Crate_Lettuce":
                animator.SetTrigger("PickUp");
                break;
            case "Crate_Tomatoes":
                animator.SetTrigger("PickUp");
                break;
            case "Crate_Cheese":
                animator.SetTrigger("PickUp");
                break;
            case "Crate_Burgers":
                animator.SetTrigger("PickUp");
                break;
            case "Crate_Random":
                animator.SetTrigger("PickUp");
                break;
            case "PlateTable":
                animator.SetInteger("NextAction", 0);
                animator.SetTrigger("PutDown");
                break;
            case "CuttingBoard":
                animator.SetInteger("NextAction", 1);
                animator.SetTrigger("PutDown");
                break;
            case "Oven":
                animator.SetInteger("NextAction", 2);
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

            StartCoroutine(interactable.Interact());
        }
    }
}