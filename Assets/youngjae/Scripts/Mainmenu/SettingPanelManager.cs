using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;
using Audio;

namespace MainMenu
{
    public class SettingPanelManager : MonoBehaviour
    {
        public Toggle[] soundToggleList;
        public Slider[] soundSliderList; // 사운드 슬라이더 리스트

        DataManager dataManager => DataManager.Instance; // 데이터 매니저 인스턴스

        void Start()
        {
            for (int i = 0; i < soundSliderList.Length; i++)
            {
                int index = i;
                soundSliderList[i].onValueChanged.AddListener((value) => ChagneSoundVolume(index, value)); // 슬라이더 값 변경 시 호출되는 메서드 등록
                soundToggleList[i].onValueChanged.AddListener((isOn) => ChangeToggleSoundVolume(index, isOn)); // 토글 값 변경 시 호출되는 메서드 등록
            }
        }

        public void SetPanelInfomation()
        {
            soundSliderList[(int)AudioManager.SoundType.Master].value = dataManager.MasterVolume; // 마스터 볼륨 슬라이더 값 설정
            soundSliderList[(int)AudioManager.SoundType.SFX].value = dataManager.SFXVolume; // SFX 볼륨 슬라이더 값 설정
            soundSliderList[(int)AudioManager.SoundType.BGM].value = dataManager.BGMVolume; // BGM 볼륨 슬라이더 값 설정

            soundToggleList[(int)AudioManager.SoundType.Master].isOn = dataManager.IsMasterVolumeOn; // 마스터 음소거 토글 설정
            soundToggleList[(int)AudioManager.SoundType.SFX].isOn = dataManager.IsSFXVolumeOn; // SFX 음소거 토글 설정
            soundToggleList[(int)AudioManager.SoundType.BGM].isOn = dataManager.IsBGMVolumeOn; // BGM 음소거 토글 설정

            AudioManager.Instance.UpdateAllVolume(); // 모든 볼륨 업데이트
        }

        void ChagneSoundVolume(int index, float value)
        {
            var soundType = (AudioManager.SoundType)index; // 인덱스를 SoundType으로 변환
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

        void ChangeToggleSoundVolume(int index, bool isOn)
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

        public void OnResetDataButton()
        {
            dataManager.ResetDataContainer(); // 데이터 초기화
        }
    }
}


