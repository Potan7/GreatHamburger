using Data;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        #region Singleton And Constructor
        static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Instantiate(Resources.Load<AudioManager>("Singletone/SoundManager")); // Resources 폴더에서 SoundManager 프리팹을 로드하여 인스턴스 생성
                    instance.InitSoundManager();
                }
                return instance;
            }
        }

        void InitSoundManager()
        {
            Debug.Log("SoundManager Initialized"); // 사운드 매니저 초기화 로그 출력
            DontDestroyOnLoad(instance.gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }

        void Start()
        {
            if (instance != this)
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

        public void SetAudioVolume(float volume, SoundType soundType)
        {
            audioMixer.SetFloat(soundType.ToString(), Mathf.Log10(volume) * 20); // 오디오 믹서의 볼륨 설정
        }

        public void UpdateAllVolume()
        {
            SetAudioVolume(DataManager.Instance.MasterVolume, SoundType.Master); // 마스터 볼륨 설정
            SetAudioVolume(DataManager.Instance.SFXVolume, SoundType.SFX); // SFX 볼륨 설정
            SetAudioVolume(DataManager.Instance.BGMVolume, SoundType.BGM); // BGM 볼륨 설정
        }   

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

    }
}
