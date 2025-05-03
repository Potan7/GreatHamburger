using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] grid;

    // 로봇 시작 위치와 목적지 위치 지정
    Vector2Int RobotPoint = new Vector2Int(0, 9);
    Vector2Int DestPoint = new Vector2Int(8, 1);
    private List<Vector2Int> pathList;

    public TextAsset stageJson; // Resources 폴더에 넣고 할당

    private RobotMovement robotMovement;

    void Start()
    {
        // 그리드 맵 불러오기
        LoadStageGridData();

        // 이동 컴포넌트 참조
        robotMovement = GetComponent<RobotMovement>();
        robotMovement.Initialize(width, cellSize);

        // 경로 탐색
        pathList = AStarPathfinder.GetPathList(grid, width, height, RobotPoint, DestPoint);

        // 이동 시작
        StartCoroutine(robotMovement.MoveRobotToNode(pathList, DestPoint));
    }

    void LoadStageGridData()
    {
        // Json 파일로부터 값을 불러와 저장
        StageGridData data = JsonUtility.FromJson<StageGridData>(stageJson.text);
        width = data.width;
        height = data.height;
        cellSize = data.cellSize;
        grid = data.To2DArray();
    }
}