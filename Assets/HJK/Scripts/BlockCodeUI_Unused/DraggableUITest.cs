using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUITest : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform _rect;
    Canvas _canvas;
    Vector3 _offset;

    public GameObject canvas;
    public GameObject blockPanelContent;
    public GameObject codePanelContent;
    public GameObject placeholder;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        placeholder.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _offset = _rect.position - eventData.pointerCurrentRaycast.worldPosition;

        placeholder.SetActive(true);
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 hitWorldPos = eventData.pointerCurrentRaycast.worldPosition;
        Vector3 targetWorldPos = hitWorldPos + _offset;
        float z = _rect.position.z;
        _rect.position = new Vector3(targetWorldPos.x, targetWorldPos.y, z);

        GetComponent<Image>().raycastTarget = false;
        transform.SetParent(canvas.transform);

        var hit = eventData.pointerCurrentRaycast.gameObject;
        if (!hit.TryGetComponent<CodeLine>(out var cl) && hit != codePanelContent)
        {
            return;
        }
        //Debug.LogError(hit);
        var contentArea = hit?.GetComponentInParent<ContentSizeFitter>();

        if (contentArea != null)
        {
            placeholder.transform.SetParent(contentArea.transform, worldPositionStays: false);
        }
        else
        {
            placeholder.transform.SetParent(codePanelContent.transform, worldPositionStays: false);
        }
        // 인덱스 동기화
        int idx = hit.transform.GetSiblingIndex();
        //Debug.LogError(hit.GetComponent<RectTransform>().position.y + " / " + _rect.position.y);
        if (hit.GetComponent<RectTransform>().position.y > _rect.position.y) idx++;
        placeholder.transform.SetSiblingIndex(idx);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(placeholder.transform);
        GetComponent<Image>().raycastTarget = true;
        //GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}