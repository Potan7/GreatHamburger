using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Data;

namespace MainMenu
{
    public class MainmenuManager : MonoBehaviour
    {
        public List<StageSelectButtonGroup> stageSelectButtonGroups = new List<StageSelectButtonGroup>(); // 스테이지 선택 버튼 그룹 리스트

        [Header("Stage Select UI")]
        public RectTransform stageSelectionPanel;
        // public Transform stageContentParent;
        public ScrollRect stageContectScrollRect;
        public StageSelectButtonGroup stageSelectButtonPrefab;
        public TextMeshProUGUI stageSelectTitleText;
        Vector2 stageSelectPanelSizeDelta;
        float stageSelectPanelInitialXPos; // 초기 X 위치

        [Header("Setting UI")]
        public RectTransform settingPanel;
        public SettingPanelManager settingPanelManager; // 설정 패널 매니저
        Vector2 settingPanelSizeDelta;
        float settingPanelInitialXPos; // 초기 X 위치

        Sequence EnableStageSelctionPaenlSequence;
        Sequence DisableStageSelctionPaenlSequence;

        Sequence EnableSettingPanelSequence;
        Sequence DisableSettingPanelSequence;

        [Header("Fade UI")]
        public Image fadeImage;
        public float fadeDuration = 1f; // 페이드 아웃 시간

        void Start()
        {
            // StageSelection 초기화
            stageSelectPanelInitialXPos = stageSelectionPanel.localPosition.x; // 초기 X 위치 저장
            stageSelectionPanel.localPosition = new Vector3(0, 0, 0); // 초기 위치 설정
            stageSelectPanelSizeDelta = stageSelectionPanel.sizeDelta; // 초기 크기 저장

            stageSelectionPanel.sizeDelta = new Vector2(0, stageSelectPanelSizeDelta.y);
            stageSelectionPanel.gameObject.SetActive(false); 
            stageSelectTitleText.color = new Color(stageSelectTitleText.color.r, stageSelectTitleText.color.g, stageSelectTitleText.color.b, 0); // 초기 투명도 설정

            // SettingPanel 초기화
            settingPanelInitialXPos = settingPanel.localPosition.x; // 초기 X 위치 저장
            settingPanel.localPosition = new Vector3(0, 0, 0); // 초기 위치 설정
            settingPanelSizeDelta = settingPanel.sizeDelta; // 초기 크기 설정

            settingPanel.sizeDelta = new Vector2(0, settingPanelSizeDelta.y); // 초기 크기 설정
            settingPanel.gameObject.SetActive(false); // 비활성화

            SetSequence();

            for (int i = 0; i < DataManager.STAGE_COUNT; i++)
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
            // DOTween 시퀀스 설정
            // StageSelectionPanel 등장 애니메이션
            EnableStageSelctionPaenlSequence = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(stageSelectionPanel.DOLocalMoveX(stageSelectPanelInitialXPos, 1f)).SetEase(Ease.OutQuad)
                .Join(stageSelectionPanel.DOSizeDelta(stageSelectPanelSizeDelta, 1f)).SetEase(Ease.OutQuad)
                .Join(stageSelectTitleText.DOFade(1, 1f).SetEase(Ease.OutQuad));

            // StageSelectionPanel 퇴장 애니메이션
            DisableStageSelctionPaenlSequence = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(stageSelectTitleText.DOFade(0, 0.3f).SetEase(Ease.OutQuad))
                .Join(stageSelectionPanel.DOSizeDelta(new Vector2(0, stageSelectPanelSizeDelta.y), 1f)).SetEase(Ease.OutQuad)
                .Join(stageSelectionPanel.DOLocalMoveX(0, 1f)).SetEase(Ease.OutQuad)
                .OnComplete(() => stageSelectionPanel.gameObject.SetActive(false));

            // SettingPanel 등장 애니메이션
            EnableSettingPanelSequence = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(settingPanel.DOLocalMoveX(settingPanelInitialXPos, 1f)).SetEase(Ease.OutQuad)
                .Join(settingPanel.DOSizeDelta(settingPanelSizeDelta, 1f)).SetEase(Ease.OutQuad)
                .OnComplete(() => settingPanelManager.SetPanelInfomation()); // 설정 패널 정보 초기화

            // SettingPanel 퇴장 애니메이션
            DisableSettingPanelSequence = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(settingPanel.DOSizeDelta(new Vector2(0, settingPanelSizeDelta.y), 1f)).SetEase(Ease.OutQuad)
                .Join(settingPanel.DOLocalMoveX(0, 1f)).SetEase(Ease.OutQuad)
                .OnComplete(() => settingPanel.gameObject.SetActive(false)); // 비활성화
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
            if (DisableSettingPanelSequence.IsPlaying() || EnableSettingPanelSequence.IsPlaying()) // 애니메이션이 재생중이라면
            {
                return; // 아무것도 하지 않음
            }

            if (settingPanel.gameObject.activeSelf) // 설정 UI가 활성화 되어있다면
            {
                DisableSettingPanelSequence.Restart();
            }
            else
            {
                settingPanel.gameObject.SetActive(true); // 설정 UI 활성화
                EnableSettingPanelSequence.Restart(); // 애니메이션 재생
            }
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

            EnableSettingPanelSequence.Kill();
            DisableSettingPanelSequence.Kill();
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


