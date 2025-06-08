using Data;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using static Audio.AudioManager;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {

        #region Singleton And Constructor
        static AudioManager instance = null;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Instantiate(Resources.Load<AudioManager>("Singletone/AudioManager")); // Resources 폴더에서 SoundManager 프리팹을 로드하여 인스턴스 생성
                    instance.InitSoundManager();
                }
                return instance;
            }
        }
        SoundBank soundBank;
        void InitSoundManager()
        {
            Debug.Log("SoundManager Initialized"); // 사운드 매니저 초기화 로그 출력
            DontDestroyOnLoad(instance.gameObject); // 씬 전환 시 파괴되지 않도록 설정

            // 모든 소리 토글 설정
            SetAudioMute(!DataManager.Instance.IsMasterVolumeOn, SoundType.Master); // 마스터 음소거 설정
            SetAudioMute(!DataManager.Instance.IsSFXVolumeOn, SoundType.SFX); // SFX 음소거 설정
            SetAudioMute(!DataManager.Instance.IsBGMVolumeOn, SoundType.BGM); // BGM 음소거 설정

            soundBank = Resources.Load<SoundBank>("SoundBankObject");
        }

        void Awake()
        {
            if (instance == null || instance == this)
            {
                instance = this; 
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        public enum SoundType
        {
            Master = 0,
            SFX = 1,
            BGM = 2
        }

        public AudioMixer audioMixer; // 오디오 믹서
        public AudioMixerGroup sfxGroup;
        public AudioSource bgmSource; // BGM 소스

        /// <summary>
        /// 오디오 믹서의 볼륨을 설정합니다.
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="soundType"></param>
        public void SetAudioVolume(float volume, SoundType soundType)
        {
            if (volume < 0) volume = 0; // 볼륨이 0보다 작으면 0으로 설정
            if (volume > 1) volume = 1; // 볼륨이 1보다 크면 1로 설정

            // 음소거일 경우 return
            switch (soundType)
            {
                case SoundType.Master:
                    if (!DataManager.Instance.IsMasterVolumeOn) return; // 마스터 음소거 시 return
                    break;
                case SoundType.SFX:
                    if (!DataManager.Instance.IsSFXVolumeOn) return; // SFX 음소거 시 return
                    break;
                case SoundType.BGM:
                    if (!DataManager.Instance.IsBGMVolumeOn) return; // BGM 음소거 시 return
                    break;
            }

            audioMixer.SetFloat(soundType.ToString(), Mathf.Log10(volume) * 20); // 오디오 믹서의 볼륨 설정
        }

        /// <summary>
        /// 모든 볼륨을 데이터 바탕으로 업데이트합니다.
        /// </summary>
        public void UpdateAllVolume()
        {
            SetAudioVolume(DataManager.Instance.MasterVolume, SoundType.Master); // 마스터 볼륨 설정
            SetAudioVolume(DataManager.Instance.SFXVolume, SoundType.SFX); // SFX 볼륨 설정
            SetAudioVolume(DataManager.Instance.BGMVolume, SoundType.BGM); // BGM 볼륨 설정
        }

        /// <summary>
        /// 오디오 믹서의 음소거를 설정합니다.
        /// 음소거 시 -80으로 설정됩니다.
        /// 음소거 해제 시 데이터의 볼륨 값으로 설정됩니다.
        /// </summary>
        /// <param name="isMute"></param>
        /// <param name="soundType"></param>
        public void SetAudioMute(bool isMute, SoundType soundType)
        {
            if (isMute)
            {
                audioMixer.SetFloat(soundType.ToString(), -80); // 음소거 설정
            }
            else
            {
                switch (soundType)
                {
                    case SoundType.Master:
                        audioMixer.SetFloat(soundType.ToString(), DataManager.Instance.MasterVolume); // 마스터 음소거 해제
                        break;
                    case SoundType.SFX:
                        audioMixer.SetFloat(soundType.ToString(), DataManager.Instance.SFXVolume); // SFX 음소거 해제
                        break;
                    case SoundType.BGM:
                        audioMixer.SetFloat(soundType.ToString(), DataManager.Instance.BGMVolume); // BGM 음소거 해제
                        break;
                }
            }
        }

        /// <summary>
        /// 효과음 생성 및 해체
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="position"></param>
        public static void MakeSoundEffect(ESoundEffect sound, Vector3 position)
        {
            AudioSource audioUnit = new GameObject().AddComponent<AudioSource>();
            audioUnit.transform.position = position; // 오디오 소스의 위치 설정
            audioUnit.spatialBlend = 1f; // 3D 사운드로 설정
            audioUnit.outputAudioMixerGroup = Instance.sfxGroup; // SFX 오디오 믹서 그룹 설정

            audioUnit.clip = Instance.soundBank.GetAudioClip(sound);
            // if (Instance.audioMixer.GetFloat((SoundType.SFX).ToString(), out float volume)) audioUnit.volume = volume;
            audioUnit.Play();
            Destroy(audioUnit.gameObject, audioUnit.clip.length);
        }

        public static void PlayRandomBGMSound(EBGMGroup bgmGroup)
        {
            Debug.Log($"Playing BGM from group: {bgmGroup}");
            BGMBank load = Resources.Load<BGMBank>(bgmGroup.ToString());
            if (load == null)
            {
                Debug.LogError($"BGM Group '{bgmGroup}' not found in Resources.");
                return;
            }

            AudioClip audioClip = load.GetRandomBGMClip();
            Instance.bgmSource.clip = audioClip; // BGM 소스에 오디오 클립 설정
            Instance.bgmSource.Play();
        }
        public static void StopBGMSound()
        {
            if (Instance.bgmSource.isPlaying)
            {
                Instance.bgmSource.Stop(); // BGM 소스 정지
            }
        }

        void OnDestroy()
        {
            instance = null;
        }

    }
}

public enum ESoundEffect
{
    Cutting_Board,
    Pop,
    Cooking,
    Appear,
    Denied
}

public enum EBGMGroup
{
    MainMenu,
    GamePlay
}