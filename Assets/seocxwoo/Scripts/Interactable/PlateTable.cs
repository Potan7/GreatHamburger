using System.Collections;
using UnityEngine;

public class PlateTable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject robotPrefab;

    private RobotController robot;
    private Animator animator;
    private Transform hand;

    private float offset = 0f;

    void Awake()
    {
        robot = robotPrefab.GetComponent<RobotController>();
        animator = robotPrefab.GetComponent<Animator>();
        hand = robotPrefab.transform.Find("Hand");
    }

    public IEnumerator Interact()
    {
        Debug.Log("Robot Interact with PlateTable.");

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

        // PlateTable 위 접시에 아이템 내려놓기(쌓기)
        robot.SetBusy(true);
        GameObject item = hand.GetChild(0).gameObject;
        Transform plate = transform.Find("plate");
        item.transform.parent = plate;
        item.transform.localPosition = new Vector3(0, offset, 0);
        offset += item.GetComponent<Ingredient>().GetHeight();
        item.transform.localRotation = Quaternion.identity;
        animator.SetInteger("NextAction", 0);
        animator.SetTrigger("PutDown");
        yield return new WaitForSeconds(1.0f);
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
    public void ResetPlateOffset() => offset = 0;
}
