using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] grid;

    // 로봇 시작 위치와 목적지 위치 지정
    Vector2Int RobotPoint = new Vector2Int(4, 9);
    Vector2Int DestPoint = new Vector2Int(9, 4);
    private List<Vector2Int> pathList;

    public TextAsset stageJson; // Resources 폴더에 넣고 할당

    private RobotMovement robotMovement;
    private RobotInteraction robotInteraction;

    [SerializeField] private InteractableRegistry interactableRegistry;

    void Start()
    {
        // 그리드 맵 불러오기
        LoadStageGridData();

        // 이동 컴포넌트 참조
        robotMovement = GetComponent<RobotMovement>();
        robotInteraction = GetComponent<RobotInteraction>();
        interactableRegistry = GetComponent<InteractableRegistry>();

        robotMovement.Initialize(width, cellSize);

        // 작동 시작
        StartCoroutine(RunRobotProgram());
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

    private IEnumerator RunRobotProgram()
    {
        // 이동 함수
        yield return StartCoroutine(Move("Red"));

        //DestPoint = new Vector2Int();

        // 경로 탐색
        //pathList = AStarPathfinder.GetPathList(grid, width, height, RobotPoint, DestPoint);

        // 이동 시작
        //yield return StartCoroutine(robotMovement.MoveRobotToNode(pathList, DestPoint));

        robotInteraction.RobotInteract();

        // 도착 위치로 변경
        RobotPoint = new Vector2Int((int)(transform.position.x / cellSize), (int)(((height - 1) * cellSize - transform.position.z) / cellSize));

        // 목적지 변경
        DestPoint = new Vector2Int(0, 2);

        // 경로 탐색
        pathList = AStarPathfinder.GetPathList(grid, width, height, RobotPoint, DestPoint);

        // 이동 시작
        yield return StartCoroutine(robotMovement.MoveRobotToNode(pathList, DestPoint));

        robotInteraction.RobotInteract();
    }

    private IEnumerator Move(string name)
    {
        Vector3 vector3 = interactableRegistry.GetPosition(name);
        Debug.Log(vector3);

        // 도착 위치로 변경
        //RobotPoint = new Vector2Int((int)(transform.position.x / cellSize), (int)(((height - 1) * cellSize - transform.position.z) / cellSize));
        RobotPoint = ChangePosToPoint(transform.position);

        // 목적지 변경
        //DestPoint = new Vector2Int((int)(transform.position.x / cellSize), (int)(((height - 1) * cellSize - transform.position.z) / cellSize));
        DestPoint = ChangePosToPoint(vector3);

        // 경로 탐색
        pathList = AStarPathfinder.GetPathList(grid, width, height, RobotPoint, DestPoint);

        // 이동 시작
        yield return StartCoroutine(robotMovement.MoveRobotToNode(pathList, DestPoint));
    }

    private Vector2Int ChangePosToPoint(Vector3 position)
    {
        Vector2Int vector2Int = new Vector2Int((int)(position.x / cellSize), (int)(((height - 1) * cellSize - position.z) / cellSize));
        return vector2Int;
    }
}