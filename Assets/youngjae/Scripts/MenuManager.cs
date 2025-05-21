using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using Data;
using Audio;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    #region Singleton And Constructor
    static MenuManager instance;
    public static MenuManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load<MenuManager>("Singletone/MenuManager")); // Resources 폴더에서 MenuManager 프리팹을 로드하여 인스턴스 생성
                instance.InitMenuManager();
            }
            return instance;
        }
    }

    void Start()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    XRInputSubsystem inputSubsystem;
    public CanvasGroup canvasGroup;

    public Slider[] soundSliderList; // 사운드 슬라이더 리스트
    public Toggle[] soundToggleList; // 사운드 토글 리스트

    DataManager dataManager => DataManager.Instance; // 데이터 매니저 인스턴스

    Sequence fadeOutSequence;
    Sequence fadeInSequence;

    Vector3 playerPosition;
    public float playerExitRadius = 1.5f;
    public bool isMenuActive = false;

    void InitMenuManager()
    {
        Debug.Log("MenuManager Initialized"); // 메뉴 매니저 초기화 로그 출력
        DontDestroyOnLoad(instance.gameObject); // 씬 전환 시 파괴되지 않도록 설정

        var subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems(subsystems); // XRInputSubsystem 인스턴스 가져오기

        if (subsystems.Count > 0)
        {
            inputSubsystem = subsystems[0];
        }
        else
        {
            Debug.LogError("No XRInputSubsystem found!");
        }

        fadeOutSequence = DOTween.Sequence().SetAutoKill(false).SetUpdate(true).Append(canvasGroup.DOFade(0, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            canvasGroup.gameObject.SetActive(false); // 페이드 아웃 후 UI 비활성화
        })).Pause();

        fadeInSequence = DOTween.Sequence().SetAutoKill(false).SetUpdate(true).Append(canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutQuad)).Pause();

        // UI 초기값 설정
        SetInitialSoundSettings();

        for (int i = 0; i < soundSliderList.Length; i++)
        {
            int index = i; // 클로저를 위해 로컬 변수 사용
            soundSliderList[i].onValueChanged.AddListener((value) => ChangeSoundVolume(index, value)); // 슬라이더 값 변경 시 호출되는 메서드 등록
        }
        for (int i = 0; i < soundToggleList.Length; i++)
        {
            int index = i; // 클로저를 위해 로컬 변수 사용
            soundToggleList[i].onValueChanged.AddListener((isOn) => ChangeToggleSoundVolume(index, isOn)); // 토글 값 변경 시 호출되는 메서드 등록
        }
        // 0 - Master, 1 - SFX, 2 - BGM

        canvasGroup.gameObject.SetActive(false); // 초기에는 UI 비활성화
    }

    void SetInitialSoundSettings()
    {
        soundSliderList[(int)AudioManager.SoundType.Master].value = dataManager.MasterVolume;
        soundSliderList[(int)AudioManager.SoundType.SFX].value = dataManager.SFXVolume;
        soundSliderList[(int)AudioManager.SoundType.BGM].value = dataManager.BGMVolume;

        soundToggleList[(int)AudioManager.SoundType.Master].isOn = dataManager.IsMasterVolumeOn;
        soundToggleList[(int)AudioManager.SoundType.SFX].isOn = dataManager.IsSFXVolumeOn;
        soundToggleList[(int)AudioManager.SoundType.BGM].isOn = dataManager.IsBGMVolumeOn;

        AudioManager.Instance.UpdateAllVolume(); // 모든 볼륨 업데이트
    }

    void Update()
    {
        if (isMenuActive)
        {
            if (Vector3.Distance(playerPosition, PlayerManager.Instance.transform.position) > playerExitRadius)
            {
                SetMenuButton(false); // 플레이어가 메뉴에서 멀어지면 메뉴 비활성화
            }
        }
    }

    private void ChangeToggleSoundVolume(int index, bool isOn)
    {
        var soundType = (AudioManager.SoundType)index; // 인덱스를 SoundType으로 변환

        switch (soundType)
        {
            case AudioManager.SoundType.Master:
                dataManager.IsMasterVolumeOn = isOn; // 마스터 음소거 설정
                break;
            case AudioManager.SoundType.SFX:
                dataManager.IsSFXVolumeOn = isOn; // SFX 음소거 설정
                break;
            case AudioManager.SoundType.BGM:
                dataManager.IsBGMVolumeOn = isOn; // BGM 음소거 설정
                break;
        }
        AudioManager.Instance.SetAudioMute(!isOn, soundType); // 오디오 믹서에 음소거 설정
    }

    public void SetMenuButton(bool isActive)
    {
        // Debug.Log($"Menu button is {(isActive ? "active" : "inactive")}");
        if (isActive)
        {
            canvasGroup.gameObject.SetActive(true); // 메뉴 활성화
            canvasGroup.interactable = true; // UI 상호작용 가능

            // UI 위치 : 카메라 정면 (y좌표는 고정)
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 cameraForward = Camera.main.transform.forward;

            Vector3 targetPosition = cameraPosition + cameraForward * 1.5f; // 카메라 앞쪽으로 이동
            targetPosition.y = PlayerManager.Instance.transform.position.y + 1f; // y좌표 고정

            transform.position = targetPosition; // 메뉴 위치 설정
            // 플레이어 기준 지평선과 평행하게 회전 (180도 반전)
            Vector3 lookDirection = cameraPosition - targetPosition;
            lookDirection.y = 0; // y축(상하) 성분 제거하여 수평만 바라보게 함
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                // 180도 반전: 반대 방향을 바라보게 함
                transform.rotation = Quaternion.LookRotation(-lookDirection.normalized, Vector3.up);
            }

            SetInitialSoundSettings();
            playerPosition = PlayerManager.Instance.transform.position; // 플레이어 위치 저장

            fadeInSequence.Restart(); // 메뉴 활성화 시 페이드 인 시퀀스 시작
        }
        else
        {
            canvasGroup.interactable = false;
            fadeOutSequence.Restart(); // 메뉴 비활성화 시 페이드 아웃 시퀀스 시작
        }
        isMenuActive = isActive; // 메뉴 활성화 상태 업데이트
    }
    public void SetMenuToggle()
    {
        SetMenuButton(!isMenuActive); // 메뉴 토글
    }

    public void ChangeSoundVolume(int idx, float value)
    {
        var soundType = (AudioManager.SoundType)idx; // 인덱스를 SoundType으로 변환
        switch (soundType)
        {
            case AudioManager.SoundType.Master:
                dataManager.MasterVolume = value; // 마스터 볼륨 설정
                break;
            case AudioManager.SoundType.SFX:
                dataManager.SFXVolume = value; // SFX 볼륨 설정
                break;
            case AudioManager.SoundType.BGM:
                dataManager.BGMVolume = value; // BGM 볼륨 설정
                break;
        }

        AudioManager.Instance.SetAudioVolume(value, soundType); // 오디오 믹서에 볼륨 설정
    }

    public void OnRecenterButtonClicked()
    {
        if (inputSubsystem?.TryRecenter() == true) // XRInputSubsystem의 TryRecenter 메서드 호출
        {
            Debug.Log("XRInputSubsystem recentered successfully.");
        }
        else
        {
            Debug.LogWarning("XRInputSubsystem is not available.");
        }
    }

    public async void OnExitButtonClicked()
    {
        canvasGroup.interactable = false; // UI 비활성화
        var tcs = new UniTaskCompletionSource();
        var scene = SceneManager.LoadSceneAsync(0);
        scene.allowSceneActivation = false;
        PlayerManager.Instance.FadeIn(() =>
        {
            tcs.TrySetResult();
        });
        await tcs.Task;
        scene.allowSceneActivation = true;

        Destroy(gameObject); // 메뉴 매니저 파괴
        instance = null; // 인스턴스 초기화
    }

    void OnDestroy()
    {
        fadeOutSequence.Kill();
        fadeInSequence.Kill();
    }
}
