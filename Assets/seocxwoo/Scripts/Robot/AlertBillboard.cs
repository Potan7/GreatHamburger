using UnityEngine;

public class AlertBillboard : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    void Update()
    {
        if (targetCamera == null) return;

        // 카메라를 향하도록 회전
        transform.LookAt(targetCamera.transform);

        // 필요 시 회전을 뒤집어 Z축을 앞면으로 만들기
        transform.rotation = Quaternion.LookRotation(targetCamera.transform.forward);
    }
}
