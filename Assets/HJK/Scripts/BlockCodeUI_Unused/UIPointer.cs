using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPointer : MonoBehaviour, IPointerMoveHandler
{

    public void OnPointerMove(PointerEventData eventData)
    {
        Vector3 hitWorldPos = eventData.pointerCurrentRaycast.worldPosition;
        Debug.LogError(hitWorldPos);
        Vector3 targetWorldPos = hitWorldPos;
        GetComponent<RectTransform>().position = new Vector3(targetWorldPos.x, targetWorldPos.y, GetComponent<RectTransform>().position.z);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
