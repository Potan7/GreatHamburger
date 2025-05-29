using System.Collections;
using UnityEngine;

public class Crate : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private GameObject ingredientPrefab;

    private RobotController robot;
    private Animator animator;
    private Transform hand;

    void Start()
    {
        robot = robotPrefab.GetComponent<RobotController>();
        animator = robotPrefab.GetComponent<Animator>();
        hand = robotPrefab.transform.Find("Hand");
    }

    public IEnumerator Interact()
    {
        Debug.Log("Robot Interact with Crate.");

        if (hand == null)
        {
            Debug.LogWarning("Hand not found on Robot.");
            yield return null;
        }

        // 기존 아이템이 있으면 줍지 않음 (옵션)
        if (hand.childCount > 0)
        {
            Debug.LogWarning("Robot Already holding an item.");
            // 여기서 다른 처리 필요
            animator.SetTrigger("Error");
            yield return null;
        }

        animator.SetTrigger("PickUp");
        yield return WaitForAnimation(animator, "PickUp");

        GameObject item = Instantiate(ingredientPrefab, hand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Debug.Log("picked up");
        robot.SetBusy(false);
    }

    private IEnumerator WaitForAnimation(Animator animator, string stateName)
    {
        // 현재 상태가 원하는 상태가 될 때까지 대기
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }

        // 애니메이션이 끝날 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 localOffset = new Vector3(0f, 0f, 1f);
        Vector3 worldOffset = transform.rotation * localOffset;
        Vector3 pos = transform.position + worldOffset + new Vector3(0, 1f, 0);
        Gizmos.DrawSphere(pos, 0.1f);
    }
}
