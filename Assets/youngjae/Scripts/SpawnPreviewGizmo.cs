using UnityEngine;
using UnityEditor;
using MapObject;

[ExecuteInEditMode]
public class SpawnPreviewGizmo : MonoBehaviour
{
    public Mesh previewMesh = null;             // 미리 보여줄 메시
    public Vector3 offset = Vector3.up;  // 위치 보정
    Color gizmoColor = Color.white; // Gizmo 색상

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR

        var mesh = previewMesh;
        if (mesh == null)
        {
            var spawner = transform.parent.GetComponent<ItemSpawner>();
            if (spawner == null || spawner.itemList.Count == 0)
            {
                return; // ItemSpawner가 없거나 아이템이 없는 경우 Gizmo를 그리지 않음
            }
            if (spawner is RandomItemSpawner randomSpawner)
            {
                // 랜덤 아이템 스포너는 다 띄우기
                for (int i = 0; i < randomSpawner.itemList.Count; i++)
                {
                    var item = randomSpawner.itemList[i];
                    if (item == null) continue;

                    var itemMesh = item.GetComponent<MeshFilter>().sharedMesh;
                    if (itemMesh != null)
                    {
                        Gizmos.DrawMesh(itemMesh, transform.position + offset * i, transform.rotation, Vector3.one);
                    }
                }
                return;

            }
            mesh = spawner.itemList[0].GetComponent<MeshFilter>().sharedMesh;
        }


        Gizmos.color = gizmoColor; // 원하는 단색으로 설정
        Gizmos.DrawMesh(mesh, transform.position + offset, transform.rotation, Vector3.one);
#endif
    }
}
