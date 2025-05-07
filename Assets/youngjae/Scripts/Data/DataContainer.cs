using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class DataContainer
    {
        public float MasterVolume = 1.0f;
        public float SFXVolume = 0.5f; // 사운드 볼륨
        public float BGMVolume = 0.5f; // 배경 음악 볼륨

        public bool[] StageClear; // 스테이지 클리어 여부
    }
}

