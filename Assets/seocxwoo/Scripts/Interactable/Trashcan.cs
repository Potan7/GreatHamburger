using System.Collections;
using UnityEngine;

public class Trashcan : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject robotPrefab;

    private RobotController robot;
    private Animator animator;
    private Transform hand;

    void Awake()
    {
        robot = robotPrefab.GetComponent<RobotController>();
        animator = robotPrefab.GetComponent<Animator>();
        hand = robotPrefab.transform.Find("Hand");
    }

    public IEnumerator Interact()
    {
        Debug.Log("Robot Interact with Trashcan.");

        // Hand에 내려놓을 아이템이 없음 (에러 발생)
        if (hand.childCount == 0)
        {
            Debug.LogWarning("Robot has no item.");

            // 에러 애니메이션 실행 후 종료
            robot.SetBusy(true);
            animator.SetTrigger("Error");
            yield return new WaitForSeconds(1.0f);
            robot.SetBusy(false);

            yield break;
        }

        GameObject item = hand.GetChild(0).gameObject;
        Destroy(item);

        // Trashcan에 아이템 버리기
        robot.SetBusy(true);
        animator.SetInteger("NextAction", 0);
        animator.SetTrigger("PutDown");
        yield return new WaitForSeconds(2.0f);
        robot.SetBusy(false);
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
