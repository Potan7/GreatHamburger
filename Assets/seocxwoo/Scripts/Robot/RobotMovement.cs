using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    // 로봇의 속도 정보
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
        // 경로(리스트)를 받아 로봇의 실제 이동 구현 
        foreach (Vector2Int point in path)
        {
            Vector3 destPos = new Vector3(point.x * cellSize, transform.position.y, (width - 1 - point.y) * cellSize);
            Vector3 direction = (transform.position - destPos).normalized;
            Quaternion endRot = Quaternion.LookRotation(direction);

            // 회전
            yield return StartCoroutine(RotateTo(endRot));
            // 이동
            yield return StartCoroutine(MoveForward(destPos));
        }

        // 마지막 회전(오브젝트를 바라보게)
        //Quaternion endRotation = Quaternion.Euler(0, (lastRot.eulerAngles.y + 180f) % 360f, 0);
        Quaternion endRotation = Quaternion.Euler(0, lastRot.eulerAngles.y % 360f, 0);
        yield return StartCoroutine(RotateTo(endRotation));

        Debug.Log("Move Finished.");
        //yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator RotateTo(Quaternion endRot)
    {
        Quaternion startRot = transform.rotation;

        float angle = Quaternion.Angle(startRot, endRot);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * (rotationSpeed / angle);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        transform.rotation = endRot;
        //yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator MoveForward(Vector3 destPos)
    {
        while (Vector3.Distance(transform.position, destPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = destPos;
        //yield return new WaitForSeconds(0.1f);
    }
}
