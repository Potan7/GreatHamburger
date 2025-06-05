using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBuns : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private List<GameObject> ingredientPrefabList = new List<GameObject>();

    private RobotController robot;
    private Animator animator;
    private Transform hand;

    private int count = 0;

    void Awake()
    {
        robot = robotPrefab.GetComponent<RobotController>();
        animator = robotPrefab.GetComponent<Animator>();
        hand = robotPrefab.transform.Find("Hand");
    }

    public IEnumerator Interact()
    {
        Debug.Log("Robot Interact with Crate_Buns.");

        // Hand에 이미 아이템이 있으면 줍지 않음 (에러 발생)
        if (hand.childCount > 0)
        {
            Debug.LogWarning("Robot Already holding an item.");

            // 에러 애니메이션 실행 후 종료
            robot.SetBusy(true);
            animator.SetTrigger("Error");
            yield return new WaitForSeconds(1.0f);
            robot.SetBusy(false);

            yield break;
        }

        // Crate_Buns에서 아이템 줍기 (빵 위/아래가 번갈아가며 등장)
        robot.SetBusy(true);
        animator.SetTrigger("PickUp");
        yield return new WaitForSeconds(1.0f);
        GameObject item = Instantiate(ingredientPrefabList[count++ % 2], hand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
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
