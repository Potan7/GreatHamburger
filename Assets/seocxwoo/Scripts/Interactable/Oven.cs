using System.Collections;
using UnityEngine;

public class Oven : MonoBehaviour, IInteractable
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
        Debug.Log("Robot Interact with Oven.");

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

        // Oven에 내려놓을 아이템이 적합하지 않음 (에러 발생)
        if (!IsSuitable(item.name))
        {
            Debug.LogWarning("Item is not suitable.");

            // 에러 애니메이션 실행 후 종료
            robot.SetBusy(true);
            animator.SetTrigger("Error");
            yield return new WaitForSeconds(1.0f);
            robot.SetBusy(false);

            yield break;
        }

        Destroy(item);

        // Oven에 패티 내려놓기->기다리기->줍기
        robot.SetBusy(true);
        animator.SetInteger("NextAction", 2);
        animator.SetTrigger("PutDown");
        yield return new WaitForSeconds(2.0f);
        animator.SetTrigger("PickUp");
        yield return new WaitForSeconds(1.0f);
        item = Instantiate(ingredientPrefab, hand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        robot.SetBusy(false);
    }

    private bool IsSuitable(string name)
    {
        if (name == "food_ingredient_burger_uncooked(Clone)")
        {
            return true;
        }
        else
        {
            return false;
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
