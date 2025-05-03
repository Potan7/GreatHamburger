using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float rotationSpeed = 180f;

    private int width;
    private float cellSize;

    public void Initialize(int width, float cellSize)
    {
        this.width = width;
        this.cellSize = cellSize;
    }

    public IEnumerator MoveRobotToNode(List<Vector2Int> path, Vector2Int destination)
    {
        foreach (Vector2Int point in path)
        {
            Vector3 targetPos = new Vector3(point.x * cellSize, transform.position.y, (width - 1 - point.y) * cellSize);
            Vector2 direction = new Vector2(transform.position.x - targetPos.x, transform.position.z - targetPos.z).normalized;

            Quaternion startRot = transform.rotation;
            Quaternion targetRot = Quaternion.Euler(0, GetRotateAngle(direction), 0);
            float angle = Quaternion.Angle(startRot, targetRot);
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * (rotationSpeed / angle);
                transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            transform.rotation = targetRot;

            if (destination.x * cellSize == targetPos.x && (width - 1 - destination.y) * cellSize == targetPos.z)
            {
                Debug.Log("Move Finished.");
                yield break;
            }

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Move Finished.");
    }

    private float GetRotateAngle(Vector2 dir)
    {
        if (dir.x > 0) return 90f;
        if (dir.x < 0) return 270f;
        if (dir.y > 0) return 0f;
        if (dir.y < 0) return 180f;
        return 0f;
    }
}
