using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    // 이동 시작 위치, 목적지 위치
    Vector2Int startPoint = new Vector2Int();
    Vector2Int destPoint = new Vector2Int();

    // 맵(그리드) 정보가 담긴 Json파일
    public TextAsset stageJson;

    // Json파일에서 추출한 그리드 정보
    private int width;
    private int height;
    private float cellSize;
    private int[,] grid;

    // A* 알고리즘을 이용한 경로(리스트)
    private List<Vector2Int> pathList;

    // Scripts
    private RobotMovement robotMovement;
    private RobotInteraction robotInteraction;
    [SerializeField] private InteractableRegistry interactableRegistry;

    private bool isBusy = false;

    void Start()
    {
        // 맵(그리드) 정보 불러오기
        LoadStageGridData();

        // Initialization
        robotMovement = GetComponent<RobotMovement>();
        robotMovement.Initialize(width, cellSize);
        robotInteraction = GetComponent<RobotInteraction>();

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
        // 로봇 실제 움직임이 입력될 함수

        yield return StartCoroutine(MoveToNodeAndInteract("CuttingBoard"));

        yield return StartCoroutine(MoveToNodeAndInteract("Crate_Buns"));
        yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        yield return StartCoroutine(MoveToNodeAndInteract("Crate_Lettuce"));
        yield return StartCoroutine(MoveToNodeAndInteract("CuttingBoard"));
        yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        yield return StartCoroutine(MoveToNodeAndInteract("Crate_Tomatoes"));
        yield return StartCoroutine(MoveToNodeAndInteract("CuttingBoard"));
        yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        yield return StartCoroutine(MoveToNodeAndInteract("Crate_Burgers"));
        yield return StartCoroutine(MoveToNodeAndInteract("Oven"));
        yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        yield return StartCoroutine(MoveToNodeAndInteract("Crate_Cheese"));
        yield return StartCoroutine(MoveToNodeAndInteract("CuttingBoard"));
        yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        yield return StartCoroutine(MoveToNodeAndInteract("Crate_Buns"));
        yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));
    }

    private IEnumerator MoveToNodeAndInteract(string name)
    {
        // 로봇이 바쁘면 대기(애니메이션 등으로 인해)
        while (isBusy)
        {
            yield return null;
        }

        // 로봇이 이동 후 상호작용하도록 구성
        yield return StartCoroutine(MoveToNode(name));
        //robotInteraction.Interact();
        robotInteraction.PlayAnim(name);
    }

    private IEnumerator MoveToNode(string name)
    {
        // 목적지 정보 저장
        Transform destination = interactableRegistry.GetTransform(name);

        // 로봇 출발 위치(그리드) 지정
        startPoint = ChangePosToPoint(transform.position);
        Debug.Log(startPoint);

        // 목적지 위치(그리드) 지정
        destPoint = ChangeDestPoint(destination);
        Debug.Log(destPoint);

        // 경로 탐색
        pathList = AStarPathfinder.GetPathList(grid, width, height, startPoint, destPoint);

        // 이동 시작
        yield return StartCoroutine(robotMovement.MoveRobotToNode(pathList, destPoint, destination.rotation));
    }

    private Vector2Int ChangePosToPoint(Vector3 position)
    {
        // 오브젝트의 position 좌표를 그리드값으로 변경
        Vector2Int vector2Int = new Vector2Int(Mathf.RoundToInt(position.x / cellSize), Mathf.RoundToInt(((height - 1) * cellSize - position.z) / cellSize));
        return vector2Int;
    }

    private Vector2Int ChangeDestPoint(Transform trans)
    {
        Debug.Log(trans);

        // 목적지 위치(그리드) 지정
        Vector2Int point = ChangePosToPoint(trans.position);

        // 목적지 오브젝트(노드)가 바라보는 방향(각도) 저장
        int rot = Mathf.RoundToInt(trans.rotation.eulerAngles.y);

        // 바라보는 방향(앞칸)을 진짜 목적지 위치로 지정(다방향 접근 불가)
        int dx = point.x;
        int dy = point.y;

        if (rot == 90) dx += 1;
        else if (rot == 270) dx -= 1;
        else if (rot == 0) dy -= 1;
        else if (rot == 180) dy += 1;

        return new Vector2Int(dx, dy);
    }

    public void SetBusy(bool boolean)
    {
        isBusy = boolean;
    }
}