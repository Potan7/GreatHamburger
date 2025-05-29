using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Data
{
    /// <summary>
    /// 파일 로드 경로는 StreamingAssets/StageDescription/{파일이름}.json
    /// textName을 비워두면 자동으로 현재 씬 이름을 통해 로드합니다.
    /// textName을 설정하면 해당 이름의 파일을 로드합니다.
    /// 
    /// 파일 내용은 다음과 같은 형식입니다:
    /// title: "설명창 제목"
    /// descriptions: [내용 배열]
    /// 
    /// 내용 배열의 각 항목의 내용은 다음과 같은 형식입니다.
    /// text: "설명 내용" - 필수로 작성
    /// imagePath: "이미지 경로" (Resources/{imagePath}로 설정되어 있습니다. 확장자는 쓰지 않습니다.) - 선택사항
    /// waitJob: "대기 작업 이름" (WaitJob 열거형에 정의된 값 중 하나) - 선택사항
    /// </summary>


    public class DescriptionPanel : MonoBehaviour
    {
        public CanvasGroup panelCanvasGroup;
        public CanvasGroup imagePanelCanvasGroup;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public Image descriptionImage;

        [Header("로드할 파일 이름 수동 적용")]
        public string textName = string.Empty;

        public DescriptionTextList descriptionTextList;

        public Button nextButton;
        public Button backButton;
        int moveChatDirection = 0;
        int chatIndex = 0;

        bool[] jobDoneCount;

        RectTransform imagePanelRectTransform;

        public WaitJob currentWaitJob = WaitJob.None;

        void Start()
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.transform.localScale = new Vector3(0f, 1f, 1f);

            imagePanelRectTransform = imagePanelCanvasGroup.GetComponent<RectTransform>();

            nextButton.onClick.AddListener(() =>
            {
                moveChatDirection = 1;
            });
            backButton.onClick.AddListener(() =>
            {
                moveChatDirection = -1;
            });

            if (string.IsNullOrEmpty(textName))
            {
                textName = SceneManager.GetActiveScene().name;
            }

            imagePanelCanvasGroup.alpha = 0f;
            DOTween.Sequence()
                .Append(panelCanvasGroup.DOFade(1f, 0.5f))
                .Join(panelCanvasGroup.transform.DOScale(Vector3.one, 0.5f))
                .OnComplete(() =>
                {
                    ShowDescriptionText().Forget();
                });
        }

        async UniTask ShowDescriptionText()
        {
            // StreamingAssets에서 텍스트 파일을 읽어오기
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "StageDescription", textName + ".json");

            if (System.IO.File.Exists(filePath))
            {
                string json = await System.IO.File.ReadAllTextAsync(filePath);
                descriptionTextList = JsonUtility.FromJson<DescriptionTextList>(json);
            }
            else
            {
                Debug.LogError($"File not found: {filePath}");
                return;
            }

            // 타이틀 애니메이션
            TextAnimation(titleText, descriptionTextList.title).Forget();

            jobDoneCount = new bool[descriptionTextList.descriptions.Length];
            // 대화 내용 애니메이션
            while (chatIndex < descriptionTextList.descriptions.Length)
            {
                // 버튼 제어 및 인덱스 조정
                SetButton(false);
                if (moveChatDirection < 0 && chatIndex > 0)
                {
                    chatIndex--;
                }
                else if (moveChatDirection > 0)
                {
                    chatIndex++;
                }
                moveChatDirection = 0; // 대기 상태로 전환
                if (chatIndex >= descriptionTextList.descriptions.Length)
                {
                    break;
                }
                // 설명 텍스트 가져오기
                var desc = descriptionTextList.descriptions[chatIndex];
                jobDoneCount[chatIndex] = true;

                // 이미지가 있다면 설정
                if (!string.IsNullOrEmpty(desc.imagePath))
                {
                    // 이미지 로드
                    Sprite sprite = Resources.Load<Sprite>(desc.imagePath);
                    if (sprite != null)
                    {
                        // descriptionImage.gameObject.SetActive(true);
                        descriptionImage.sprite = sprite;
                        descriptionImage.SetNativeSize(); // 이미지 크기를 원본 크기로 설정

                        Vector2 imageSize = descriptionImage.rectTransform.sizeDelta;
                        Vector2 imagePanelSize = imagePanelRectTransform.sizeDelta - new Vector2(10f, 10f); // 패널 크기에 여백 추가

                        // 이미지가 패널보다 크면 패널 크기에 맞게 조정
                        if (imageSize.x > imagePanelSize.x || imageSize.y > imagePanelSize.y)
                        {
                            float scaleX = imagePanelSize.x / imageSize.x;
                            float scaleY = imagePanelSize.y / imageSize.y;
                            float scale = Mathf.Min(scaleX, scaleY);
                            descriptionImage.rectTransform.sizeDelta = new Vector2(imageSize.x * scale, imageSize.y * scale);
                        }

                        imagePanelCanvasGroup.DOFade(1f, 0.5f).From(0f); // 페이드 인 애니메이션
                    }
                    else
                    {
                        Debug.LogWarning($"Image not found at path: {desc.imagePath}");
                        descriptionImage.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (imagePanelCanvasGroup.alpha > 0f)
                        imagePanelCanvasGroup.DOFade(0f, 0.5f).From(1f); // 페이드 아웃 애니메이션
                    // descriptionImage.gameObject.SetActive(false);
                }

                // 텍스트 재생
                await TextAnimation(descriptionText, desc.text);

                // 버튼 생성
                SetButton(true);
                // 만약 대기 작업이 있다면 대기
                // 이때 마지막 액션은 대기작업 체크 X
                WaitJob waitJob = Enum.TryParse<WaitJob>(desc.waitJob, out var parsedJob) ? parsedJob : WaitJob.None;
                if (chatIndex < descriptionTextList.descriptions.Length - 1 && waitJob != WaitJob.None && !jobDoneCount[chatIndex + 1])
                {
                    nextButton.interactable = false;
                    currentWaitJob = waitJob;
                    await UniTask.WaitUntil(() => currentWaitJob == WaitJob.None);
                    nextButton.interactable = true;
                }
                else
                {
                    nextButton.interactable = true;
                }
                await UniTask.WaitUntil(() => moveChatDirection != 0);

            }
        }

        async UniTask TextAnimation(TextMeshProUGUI textMesh, string text, int delay = 50)
        {
            textMesh.text = string.Empty;
            foreach (char c in text)
            {
                textMesh.text += c;
                await UniTask.Delay(delay); // 글자 하나씩 나타나는 시간 조절
            }
        }

        void SetButton(bool isActive)
        {
            nextButton.gameObject.SetActive(isActive && chatIndex < descriptionTextList.descriptions.Length - 1);
            backButton.gameObject.SetActive(isActive && chatIndex > 0);
        }

        public void JobComplete(WaitJob job)
        {
            if (job == currentWaitJob)
            {
                currentWaitJob = WaitJob.None;
            }
        }

    }
}
