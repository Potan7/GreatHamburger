using UnityEngine;

public class MapSubCanvas : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void OnSkipButtonClicked()
    {
        PlayerMapManager.Instance.PlateSuccess();
    }

    public void OnRestartButtonClicked()
    {
        GetComponent<CanvasGroup>().interactable = false;
        PlayerMapManager.Instance.RestartGame();
    }
}
