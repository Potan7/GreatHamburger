using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainMenu
{
    public class StageSelectButtonGroup : MonoBehaviour
    {
        public TextMeshProUGUI stage1Text;
        public TextMeshProUGUI stage2Text;

        public Button stage1Button;
        public Button stage2Button;

        public UnityEvent onFirstStageButtonClicked;
        public UnityEvent onSecondStageButtonClicked;

        public void SetStageText(int stageNumber)
        {
            stage1Text.text = $"스테이지 {stageNumber} - 1";
            stage2Text.text = $"스테이지 {stageNumber} - 2";
        }

        public void SetFirstStageInteractive(bool isInteractive)
        {
            stage1Button.interactable = isInteractive; // 버튼 상호작용 설정
        }

        public void SetSecondStageInteractive(bool isInteractive)
        {
            stage2Button.interactable = isInteractive; // 버튼 상호작용 설정
        }

        public void OnFirstStageButtonClicked()
        {
            onFirstStageButtonClicked?.Invoke(); // 첫 번째 스테이지 버튼 클릭 시 이벤트 호출
        }

        public void OnSecondStageButtonClicked()
        {
            onSecondStageButtonClicked?.Invoke(); // 두 번째 스테이지 버튼 클릭 시 이벤트 호출
        }
    }
}
