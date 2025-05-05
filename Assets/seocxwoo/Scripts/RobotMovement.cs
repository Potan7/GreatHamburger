using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    // ·Îº¿ÀÇ ¼Óµµ Á¤º¸
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float rotationSpeed = 180f;

    private int width;
    private float cellSize;

    public void Initialize(int width, float cellSize)
    {
        this.width = width;
        this.cellSize = cellSize;
    }

    public IEnumerator MoveRobotToNode(List<Vector2Int> path, Vector2Int destination, Quaternion lastRot)
    {
        foreach (Vector2Int point in path)
        {
            Vector3 targetPos = new Vector3(point.x * cellSize, transform.position.y, (width - 1 - point.y) * cellSize);

            Vector3 direction = (transform.position - targetPos).normalized;

            Quaternion startRot = transform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(direction);
            
            float angle = Quaternion.Angle(startRot, targetRot);
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * (rotationSpeed / angle);
                transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            transform.rotation = targetRot;

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;
            yield return new WaitForSeconds(0.1f);
        }
        ///////////
        Quaternion nowRot = transform.rotation;

        Vector3 rr = lastRot.eulerAngles;

        Quaternion finalRot = Quaternion.Euler(0, (rr.y + 180f) % 360f, 0);

        float bngle = Quaternion.Angle(nowRot, finalRot);
        float tw = 0f;

        while (tw < 1f)
        {
            tw += Time.deltaTime * (rotationSpeed / bngle);
            transform.rotation = Quaternion.Slerp(nowRot, finalRot, tw);
            yield return null;
        }

        transform.rotation = finalRot;
        ////////////// ¹­¾î¼­ ÇÔ¼ö·Î »©±â
        Debug.Log("Move Finished.");
        yield return new WaitForSeconds(1.0f);
    }
}
