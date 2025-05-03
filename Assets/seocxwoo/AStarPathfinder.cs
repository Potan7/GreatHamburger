using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    private class GridPoint
    {
        public int x, y;
        public int fn, gn, hn;
        public GridPoint parentPoint;
    };

    public static List<Vector2Int> GetPathList(int[,] grid, int width, int height, Vector2Int start, Vector2Int destination)
    {
        List<GridPoint> openList = new List<GridPoint>();
        List<GridPoint> closeList = new List<GridPoint>();
        List<Vector2Int> pathList = new List<Vector2Int>();

        // 최초 로봇의 위치 생성
        GridPoint curPoint = new GridPoint();
        curPoint.x = start.x;
        curPoint.y = start.y;
        curPoint.gn = 0;
        openList.Add(curPoint);

        while (true)
        {
            // 상하좌우 4방향에 대한 연산 진행
            AddNewPointToOpenList(openList, closeList, curPoint, grid, width, height, destination, -1, 0);
            AddNewPointToOpenList(openList, closeList, curPoint, grid, width, height, destination, 1, 0);
            AddNewPointToOpenList(openList, closeList, curPoint, grid, width, height, destination, 0, -1);
            AddNewPointToOpenList(openList, closeList, curPoint, grid, width, height, destination, 0, 1);

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
            if (curPoint.x == destination.x && curPoint.y == destination.y)
            {
                break;
            }
        }

        // parentPoint를 이용해 pathList에 경로 저장
        GridPoint pathfindPoint = curPoint;
        while (pathfindPoint != null)
        {
            Vector2Int vector2Int = new Vector2Int(pathfindPoint.x, pathfindPoint.y);
            pathList.Add(vector2Int);
            pathfindPoint = pathfindPoint.parentPoint;
        }

        pathList.Reverse();

        return pathList;
    }

    private static void AddNewPointToOpenList(List<GridPoint> openList, List<GridPoint> closeList, GridPoint curPoint, int[,] grid, int width, int height, Vector2Int destination, int offsetX, int offsetY)
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
        newPoint.hn = Math.Abs(newPoint.x - destination.x) + Math.Abs(newPoint.y - destination.y);
        newPoint.fn = newPoint.gn + newPoint.hn;
        newPoint.parentPoint = curPoint;

        openList.Add(newPoint);
    }
}
