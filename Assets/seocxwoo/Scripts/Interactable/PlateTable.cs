using System.Collections;
using UnityEngine;

public class PlateTable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject robotPrefab;

    private RobotController robot;
    private Animator animator;
    private Transform hand;

    private float offset = 0f;

    void Start()
    {
        robot = robotPrefab.GetComponent<RobotController>();
        animator = robotPrefab.GetComponent<Animator>();
        hand = robotPrefab.transform.Find("Hand");
    }

    public IEnumerator Interact()
    {
        Debug.Log("Robot Interact with PlateTable.");

        if (hand == null)
        {
            Debug.LogWarning("Hand transform not found on interactor.");
            yield return null;
        }

        if (hand.childCount == 0)
        {
            Debug.LogWarning("Robot has no item.");
            animator.SetTrigger("Error");
            yield return null;
        }

        GameObject item = hand.GetChild(0).gameObject;
        Transform plate = transform.Find("plate");
        item.transform.parent = plate;
        item.transform.localPosition = new Vector3(0, offset, 0);
        offset += item.GetComponent<Ingredient>().GetHeight();
        item.transform.localRotation = Quaternion.identity;

        animator.SetTrigger("PutDown");
        yield return WaitForAnimation(animator, "PutDown");

        Debug.Log("put down");
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
