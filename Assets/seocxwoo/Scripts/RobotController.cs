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

        robotInteraction.RobotInteract();

        // 이동 함수
        yield return StartCoroutine(Move("Blue"));

        robotInteraction.RobotInteract();
    }

    private IEnumerator Move(string name)
    {
        // 목적지 정보 저장
        Transform destination = interactableRegistry.GetTransform(name);

        // 로봇 출발 위치(그리드) 지정
        RobotPoint = ChangePosToPoint(transform.position);

        Vector2Int point = ChangePosToPoint(destination.position);
        int rot = (int)(destination.rotation.eulerAngles.y);
        Vector2Int newPoint = ChangeDestPoint(point, rot);
        Debug.Log(rot);
        DestPoint = newPoint;
        // 목적지 위치(그리드) 지정
        //DestPoint = ChangePosToPoint(destination.position);

        // 경로 탐색
        pathList = AStarPathfinder.GetPathList(grid, width, height, RobotPoint, DestPoint);


        // 이동 시작
        yield return StartCoroutine(robotMovement.MoveRobotToNode(pathList, DestPoint, destination.rotation));
    }

    private Vector2Int ChangePosToPoint(Vector3 position)
    {
        // 오브젝트의 position 좌표를 그리드값으로 변경
        Vector2Int vector2Int = new Vector2Int((int)(position.x / cellSize), (int)(((height - 1) * cellSize - position.z) / cellSize));
        return vector2Int;
    }

    private Vector2Int ChangeDestPoint(Vector2Int point, int rot)
    {
        int dx = point.x;
        int dy = point.y;

        if (rot == 90) dx -= 1;
        else if (rot == 270) dx += 1;
        else if (rot == 0) dy += 1;
        else if (rot == 180) dy -= 1;

        return new Vector2Int(dx, dy);
    }
}