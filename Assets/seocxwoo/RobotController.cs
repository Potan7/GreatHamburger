using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Rendering;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    private class GridPoint
    {
        public int x;
        public int y;
        public int fn;
        public int gn;
        public int hn;
        public GridPoint parentPoint;
    };

    private int width;
    private int height;
    private int[,] grid;
    private float cellSize;

    float moveSpeed = 3.0f;
    float rotationSpeed = 180f;

    GridPoint RobotPoint;
    GridPoint DestPoint;

    List<GridPoint> openList = new List<GridPoint>();
    List<GridPoint> closeList = new List<GridPoint>();
    List<GridPoint> pathList = new List<GridPoint>();

    public TextAsset stageJson; // Resources 폴더에 넣고 할당

    void Start()
    {
        // 그리드 맵 불러오기
        LoadStageGridData();

        // 로봇 시작 위치 지정
        RobotPoint = new GridPoint();
        RobotPoint.x = 0;
        RobotPoint.y = 9;

        // 목적지 위치 지정
        DestPoint = new GridPoint();
        DestPoint.x = 8;
        DestPoint.y = 1;

        // 경로 탐색 후 이동
        AStarPathfinding();
    }

    void LoadStageGridData()
    {
        // Json 파일로부터 값을 불러와 저장
        StageGridData data = JsonUtility.FromJson<StageGridData>(stageJson.text);
        width = data.width;
        height = data.height;
        cellSize = data.cellSize;

        int index = 0;
        grid = new int[width, height];

        // grid값 불러와 저장
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid[j, i] = data.grid[index++];
            }
        }
    }

    void AStarPathfinding()
    {
        // 최초 로봇의 위치 생성
        GridPoint curPoint = new GridPoint();
        curPoint.x = RobotPoint.x;
        curPoint.y = RobotPoint.y;
        curPoint.gn = 0;
        openList.Add(curPoint);

        while (true)
        {
            // 상하좌우 4방향에 대한 연산 진행
            AddNewPointToOpenList(curPoint, -1, 0);
            AddNewPointToOpenList(curPoint, 1, 0);
            AddNewPointToOpenList(curPoint, 0, -1);
            AddNewPointToOpenList(curPoint, 0, 1);
             
            openList.Remove(curPoint);
            closeList.Add(curPoint);

            // 임시 GridPoint 생성
            GridPoint nextPoint = new GridPoint();
            nextPoint.fn = 1000;

            // for문을 통해 다음으로 이동할 위치 정하기
            foreach (GridPoint point in openList)
            {
                if (point.fn < nextPoint.fn)
                {
                    nextPoint = point;
                }
            }

            curPoint = nextPoint;

            // 현재 위치가 목적지라면 STOP
            if (curPoint.x == DestPoint.x && curPoint.y == DestPoint.y)
            {
                break;
            }
        }

        // parentPoint를 이용해 pathList에 경로 저장
        GridPoint pathfindPoint = curPoint;
        while (pathfindPoint != null)
        {
            pathList.Add(pathfindPoint);
            pathfindPoint = pathfindPoint.parentPoint;
        }

        pathList.Reverse();

        // 코루틴 실행 (로봇 움직임)
        StartCoroutine("MoveRobotToNode");
    }

    void AddNewPointToOpenList(GridPoint curPoint, int offsetX, int offsetY)
    {
        // 상하좌우의 값들을 계산하기 위한 함수
        GridPoint newPoint = new GridPoint();
        newPoint.x = curPoint.x + offsetX;
        newPoint.y = curPoint.y + offsetY;

        // 그리드 위를 벗어나면 return
        if (newPoint.x < 0 || newPoint.x >= width) return;
        if (newPoint.y < 0 || newPoint.y >= height) return;
        // 이미 리스트에 들어있는 경우 return
        if (closeList.Exists(p => p.x == newPoint.x && p.y == newPoint.y)) return;
        if (openList.Exists(p => p.x == newPoint.x && p.y == newPoint.y)) return;
        // 장애물(그리드의 1값)인 경우 return
        if (grid[newPoint.x, newPoint.y] != 0) return;

        // 값 계산 후 열린 리스트에 저장
        newPoint.gn = curPoint.gn + 1;
        newPoint.hn = Math.Abs(newPoint.x - DestPoint.x) + Math.Abs(newPoint.y - DestPoint.y);
        newPoint.fn = newPoint.gn + newPoint.hn;
        newPoint.parentPoint = curPoint;

        openList.Add(newPoint);
    }

    IEnumerator MoveRobotToNode()
    {
        // 저장된 경로(리스트)를 따라 순차적으로 이동
        foreach (GridPoint point in pathList)
        {
            //Debug.Log("Next Point: (" + point.x + ", " + point.y + ")");

            // 이동할 다음 실제 위치
            Vector3 targetPos = new Vector3(point.x * cellSize, transform.position.y, (width - 1 - point.y) * cellSize);

            // 다음 이동 방향 구하기
            Vector2 direction = new Vector2(transform.position.x - targetPos.x, transform.position.z - targetPos.z).normalized;

            // 시작 각도와 목표 각도 설정
            Quaternion startRot = transform.rotation;
            Quaternion targetRot = Quaternion.Euler(0, GetRotateAngle(direction), 0);
            float angle = Quaternion.Angle(startRot, targetRot);

            // 목표 각도까지 회전
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * (rotationSpeed / angle);
                transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            // 정확한 회전값 대입
            transform.rotation = targetRot;

            // 목표 위치까지 이동
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // 정확한 위치값 대입
            transform.position = targetPos;

            // 이동 사이에 대기 시간 추가
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Move Finished.");
    }

    float GetRotateAngle(Vector2 dir)
    {
        // 이동할 방향에 따라 목표 각도 설정 후 반환
        float angle = 0f;

        if (dir.x > 0) angle = 90f;
        else if (dir.x < 0) angle = 270f;
        else if (dir.y > 0) angle = 0f;
        else if (dir.y < 0) angle = 180f;

        return angle;
    }
}