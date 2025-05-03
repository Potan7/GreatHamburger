using UnityEngine;

public class TestManager : MonoBehaviour
{
    public RobotController robotController;
    [SerializeField] private Vector2Int destination;

    void Start()
    {
        // 초기 명령 테스트: 목적지 좌표를 설정하고 이동 명령 전달
        SendRobotTo(destination);
    }

    public void SendRobotTo(Vector2Int targetPos)
    {
        //robotController.SetDestination(targetPos);
    }
}
