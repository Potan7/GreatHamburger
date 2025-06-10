using UnityEngine;

public class AlertBillboard : MonoBehaviour
{
    void Update()
    {
        // 카메라를 향하도록 회전
        transform.LookAt(Camera.main.transform);

        // 필요 시 회전을 뒤집어 Z축을 앞면으로 만들기
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
