using System.Collections;
using System.Collections.Generic;
// using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Events;
// using static UnityEditor.Recorder.OutputPath;

public class RobotController : MonoBehaviour
{
    // �̵� ���� ��ġ, ������ ��ġ
    Vector2Int startPoint = new Vector2Int();
    Vector2Int destPoint = new Vector2Int();

    // ��(�׸���) ������ ��� Json����
    public TextAsset stageJson;

    // Json���Ͽ��� ������ �׸��� ����
    private int width;
    private int height;
    private float cellSize;
    private int[,] grid;

    // A* �˰������� �̿��� ���(����Ʈ)
    private List<Vector2Int> pathList;

    // Scripts
    private RobotMovement robotMovement;
    private RobotInteraction robotInteraction;
    [SerializeField] private InteractableRegistry interactableRegistry;

    private Transform hand;
    private bool isBusy = false;

    public UnityEvent occurError = new UnityEvent();
    public UnityEvent handsFree = new UnityEvent();

    void Awake()
    {
        hand = transform.Find("Hand");
    }

    void Start()
    {
        // ��(�׸���) ���� �ҷ�����
        LoadStageGridData();

        // Initialization
        robotMovement = GetComponent<RobotMovement>();
        robotMovement.Initialize(width, cellSize);
        robotInteraction = GetComponent<RobotInteraction>();

        // �۵� ����
        //StartCoroutine(RunRobotProgram());
    }

    void LoadStageGridData()
    {
        // Json ���Ϸκ��� ���� �ҷ��� ����
        StageGridData data = JsonUtility.FromJson<StageGridData>(stageJson.text);
        width = data.width;
        height = data.height;
        cellSize = data.cellSize;
        grid = data.To2DArray();
    }

    private IEnumerator RunRobotProgram()
    {
        // �κ� ���� �������� �Էµ� �Լ�

        yield return StartCoroutine(MoveToNodeAndInteract("Crate_Buns"));
        yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));
        yield return StartCoroutine(MoveToNodeAndInteract("Crate_Buns"));
        yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        //yield return StartCoroutine(MoveToNodeAndInteract("Crate_Lettuce"));
        //yield return StartCoroutine(MoveToNodeAndInteract("CuttingBoard"));
        //yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        //yield return StartCoroutine(MoveToNodeAndInteract("Crate_Tomatoes"));
        //yield return StartCoroutine(MoveToNodeAndInteract("CuttingBoard"));
        //yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        //yield return StartCoroutine(MoveToNodeAndInteract("Crate_Burgers"));
        //yield return StartCoroutine(MoveToNodeAndInteract("Oven"));
        //yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        //yield return StartCoroutine(MoveToNodeAndInteract("Crate_Cheese"));
        //yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        //yield return StartCoroutine(MoveToNodeAndInteract("Crate_Buns"));
        //yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        //yield return StartCoroutine(MoveToNodeAndInteract("Crate_Lettuce"));
        //yield return StartCoroutine(MoveToNodeAndInteract("Oven"));

        //yield return StartCoroutine(MoveToNodeAndInteract("Oven"));
        //yield return StartCoroutine(MoveToNodeAndInteract("Crate_Random_A"));
        //yield return StartCoroutine(MoveToNodeAndInteract("Crate_Buns"));
        //yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));

        //yield return StartCoroutine(MoveToNodeAndInteract("Trashcan"));
        //yield return StartCoroutine(MoveToNodeAndInteract("Crate_Buns"));
        //Debug.Log(WhatIsInHand());
        //yield return StartCoroutine(MoveToNodeAndInteract("Trashcan"));

        //for (int i = 0; i < 2; i++)
        //{
        //    yield return StartCoroutine(MoveToNodeAndInteract("Crate_Buns"));
        //    yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));
        //}

        //GameManager.instance.CheckResult();

        //Time.timeScale = 20;

        //for (int i = 0; i < 5; i++)
        //{
        //    GameManager.instance.CleanPlate();
        //    for (int j = 0; j < 2; j++)
        //    {
        //        yield return StartCoroutine(MoveToNodeAndInteract("Crate_Buns"));
        //        yield return StartCoroutine(MoveToNodeAndInteract("PlateTable"));
        //    }
        //    GameManager.instance.CheckResult();
        //}

        //Time.timeScale = 1;

        if (GameManager.instance.CheckResult())
        {
            PlayerMapManager.Instance.PlateSuccess();
        }
        yield break;
    }

    public IEnumerator MoveToNodeAndInteract(string name)
    {
        // �κ��� �ٻڸ� ��� (�ռ� ���� ���� ��)
        while (isBusy)
        {
            yield return null;
        }

        // �κ��� �̵� �� ��ȣ�ۿ��ϵ��� ����
        yield return StartCoroutine(MoveToNode(name));
        robotInteraction.Interact();

        // �߰��� ���� (������ �̰���)
        while (isBusy)
        {
            yield return null;
        }
    }

    private IEnumerator MoveToNode(string name)
    {
        Debug.Log("Robot move to " + name + ".");

        // ������ ���� ����
        Transform destination = interactableRegistry.GetTransform(name);

        // �κ� ��� ��ġ(�׸���) ����
        startPoint = ChangePosToPoint(transform.position);

        // ������ ��ġ(�׸���) ����
        destPoint = ChangeDestPoint(destination);

        // ��� Ž��
        pathList = AStarPathfinder.GetPathList(grid, width, height, startPoint, destPoint);

        // �̵� ����
        yield return StartCoroutine(robotMovement.MoveRobotToNode(pathList, destPoint, destination.rotation));
    }

    private Vector2Int ChangePosToPoint(Vector3 position)
    {
        // ������Ʈ�� position ��ǥ�� �׸��尪���� ����
        Vector2Int vector2Int = new Vector2Int(Mathf.RoundToInt(position.x / cellSize), Mathf.RoundToInt(((height - 1) * cellSize - position.z) / cellSize));
        return vector2Int;
    }

    private Vector2Int ChangeDestPoint(Transform trans)
    {
        // ������ ��ġ(�׸���) ����
        Vector2Int point = ChangePosToPoint(trans.position);

        // ������ ������Ʈ(���)�� �ٶ󺸴� ����(����) ����
        int rot = Mathf.RoundToInt(trans.rotation.eulerAngles.y);

        // �ٶ󺸴� ����(��ĭ)�� ��¥ ������ ��ġ�� ����(�ٹ��� ���� �Ұ�)
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

    public int WhatIsInHand()
    {
        int itemIndex;

        if (hand.childCount == 0)
        {
            return -1;
        }

        itemIndex = interactableRegistry.GetIngredientIndex(hand.GetChild(0).GetComponent<Ingredient>().GetName());
        return itemIndex;
    }
    public void CleanHand()
    {
        if (hand.childCount == 1)
        {
            Destroy(hand.GetChild(0).gameObject);
        }

        interactableRegistry.GetTransform("Crate_Buns").GetComponent<CrateBuns>().ResetCount();
    }
}