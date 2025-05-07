using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class MainmenuManager : MonoBehaviour
    {
        public int stageCount = 10; // 스테이지 개수
        public List<StageSelectButtonGroup> stageSelectButtonGroups = new List<StageSelectButtonGroup>(); // 스테이지 선택 버튼 그룹 리스트

        [Header("Stage Select UI")]
        public RectTransform stageSelectionPanel;
        // public Transform stageContentParent;
        public ScrollRect stageContectScrollRect;
        public StageSelectButtonGroup stageSelectButtonPrefab;
        public TextMeshProUGUI stageSelectTitleText;
        Vector2 stageSelectPanelSizeDelta;

        Sequence EnableStageSelctionPaenlSequence;
        Sequence DisableStageSelctionPaenlSequence;

        [Header("Fade UI")]
        public Image fadeImage;
        public float fadeDuration = 1f; // 페이드 아웃 시간

        void Start()
        {
            stageSelectionPanel.localPosition = new Vector3(0, 0, 0); // 초기 위치 설정
            stageSelectPanelSizeDelta = stageSelectionPanel.sizeDelta; // 초기 크기 저장

            // 초기에는 비활성화
            stageSelectionPanel.sizeDelta = new Vector2(0, stageSelectPanelSizeDelta.y);
            stageSelectionPanel.gameObject.SetActive(false); 
            stageSelectTitleText.color = new Color(stageSelectTitleText.color.r, stageSelectTitleText.color.g, stageSelectTitleText.color.b, 0); // 초기 투명도 설정
            SetSequence();

            for (int i = 0; i < stageCount; i++)
            {
                StageSelectButtonGroup stageSelectButtonGroup = Instantiate(stageSelectButtonPrefab, stageContectScrollRect.content);
                stageSelectButtonGroup.SetStageText(i + 1); // 스테이지 텍스트 설정

                int stageNumber = i; // 스테이지 번호

                stageSelectButtonGroup.onFirstStageButtonClicked.AddListener(() => OnStageButtonClicked(stageNumber * 2)); // 첫 번째 스테이지 버튼 클릭 이벤트 등록
                stageSelectButtonGroup.onSecondStageButtonClicked.AddListener(() => OnStageButtonClicked(stageNumber * 2 + 1)); // 두 번째 스테이지 버튼 클릭 이벤트 등록

                stageSelectButtonGroups.Add(stageSelectButtonGroup); // 리스트에 추가
            }
            stageContectScrollRect.normalizedPosition = new Vector2(0.5f, 1); // 스크롤 위치 초기화
        }

        private void SetSequence()
        {
            EnableStageSelctionPaenlSequence = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(stageSelectionPanel.DOLocalMoveX(650f, 1f)).SetEase(Ease.OutQuad)
                .Join(stageSelectionPanel.DOSizeDelta(stageSelectPanelSizeDelta, 1f)).SetEase(Ease.OutQuad)
                .Join(stageSelectTitleText.DOFade(1, 1.2f).SetEase(Ease.OutQuad));

            DisableStageSelctionPaenlSequence = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(stageSelectTitleText.DOFade(0, 0.3f).SetEase(Ease.OutQuad))
                .Join(stageSelectionPanel.DOSizeDelta(new Vector2(0, stageSelectPanelSizeDelta.y), 1f)).SetEase(Ease.OutQuad)
                .Join(stageSelectionPanel.DOLocalMoveX(0, 1f)).SetEase(Ease.OutQuad)
                .OnComplete(() => stageSelectionPanel.gameObject.SetActive(false)); // 애니메이션 완료 후 비활성화
        }

        public void OnStartButton()
        {
            if (DisableStageSelctionPaenlSequence.IsPlaying() || EnableStageSelctionPaenlSequence.IsPlaying()) // 애니메이션이 재생중이라면
            {
                return; // 아무것도 하지 않음
            }

            if (stageSelectionPanel.gameObject.activeSelf) // 스테이지 선택 UI가 활성화 되어있다면
            {
                DisableStageSelctionPaenlSequence.Restart();
            }
            else
            {
                stageSelectionPanel.gameObject.SetActive(true); // 스테이지 선택 UI 활성화
                EnableStageSelctionPaenlSequence.Restart(); // 애니메이션 재생
            }

        }

        public void OnOptionButton()
        {

        }

        public void OnExitButton()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        void OnDestroy()
        {
            // DOTween 시퀀스 해제
            EnableStageSelctionPaenlSequence.Kill();
            DisableStageSelctionPaenlSequence.Kill();
        }

        void OnStageButtonClicked(int stageNumber)
        {
            fadeImage.gameObject.SetActive(true); // 페이드 이미지 활성화
            fadeImage.DOFade(1, fadeDuration).OnComplete(() =>
            {
                // 스테이지 선택 후 처리할 로직을 여기에 추가합니다.
                SceneManager.LoadScene(1);
            });
        }
    }
}


