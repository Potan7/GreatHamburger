using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class SettingPanelManager : MonoBehaviour
    {
        public enum SoundIndex
        {
            MasterVolume = 0,
            SFXVolume = 1,
            BGMVolume = 2
        }

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
            soundSliderList[(int)SoundIndex.MasterVolume].value = dataManager.MasterVolume; // 마스터 볼륨 슬라이더 값 설정
            soundSliderList[(int)SoundIndex.SFXVolume].value = dataManager.SFXVolume; // SFX 볼륨 슬라이더 값 설정
            soundSliderList[(int)SoundIndex.BGMVolume].value = dataManager.BGMVolume; // BGM 볼륨 슬라이더 값 설정

            soundToggleList[(int)SoundIndex.MasterVolume].isOn = dataManager.IsMasterVolumeOn; // 마스터 음소거 토글 설정
            soundToggleList[(int)SoundIndex.SFXVolume].isOn = dataManager.IsSFXVolumeOn; // SFX 음소거 토글 설정
            soundToggleList[(int)SoundIndex.BGMVolume].isOn = dataManager.IsBGMVolumeOn; // BGM 음소거 토글 설정
        }

        void ChagneSoundVolume(int index, float value)
        {
            switch (index)
            {
                case (int)SoundIndex.MasterVolume:
                    dataManager.MasterVolume = value; // 마스터 볼륨 설정
                    break;
                case (int)SoundIndex.SFXVolume:
                    dataManager.SFXVolume = value; // SFX 볼륨 설정
                    break;
                case (int)SoundIndex.BGMVolume:
                    dataManager.BGMVolume = value; // BGM 볼륨 설정
                    break;
            }
        }

        void ChangeToggleSoundVolume(int index, bool isOn)
        {
            switch (index)
            {
                case (int)SoundIndex.MasterVolume:
                    dataManager.IsMasterVolumeOn = isOn; // 마스터 음소거 설정
                    break;
                case (int)SoundIndex.SFXVolume:
                    dataManager.IsSFXVolumeOn = isOn; // SFX 음소거 설정
                    break;
                case (int)SoundIndex.BGMVolume:
                    dataManager.IsBGMVolumeOn = isOn; // BGM 음소거 설정
                    break;
            }
        }

        public void OnResetDataButton()
        {
            dataManager.ResetDataContainer(); // 데이터 초기화
        }
    }
}


