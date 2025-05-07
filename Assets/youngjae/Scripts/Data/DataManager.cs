using System.IO;
using UnityEngine;

namespace Data
{
    public class DataManager : MonoBehaviour
    {
        #region Singleton And Constructor
        static DataManager instance;
        public static DataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Instantiate(Resources.Load<DataManager>("Singletone/DataManager")); // Resources 폴더에서 DataManager 프리팹을 로드하여 인스턴스 생성
                    DontDestroyOnLoad(instance.gameObject); // 씬 전환 시 파괴되지 않도록 설정
                    instance.InitDataManager();
                }
                return instance;
            }
        }

        void InitDataManager()
        {
            CurrentDataContainer = LoadDataContainer(); // 데이터 컨테이너 로드
        }

        void Awake()
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        public const int STAGE_COUNT = 10; // 스테이지 개수
        public const string SAVEFILE_NAME = "data.json"; // 저장 파일 이름
        public readonly string SAVEFILE_PATH = Path.Combine(Application.persistentDataPath, SAVEFILE_NAME); // 저장 파일 경로

        #region DataContainer
        public DataContainer CurrentDataContainer { get; private set; }

        public float MasterVolume
        {
            get => CurrentDataContainer.MasterVolume;
            set => CurrentDataContainer.MasterVolume = value;
        }

        public float SFXVolume
        {
            get => CurrentDataContainer.SFXVolume;
            set => CurrentDataContainer.SFXVolume = value;
        }

        public float BGMVolume
        {
            get => CurrentDataContainer.BGMVolume;
            set => CurrentDataContainer.BGMVolume = value;
        }

        public bool[] StageClear
        {
            get => CurrentDataContainer.StageClear;
            set => CurrentDataContainer.StageClear = value;
        }
        #endregion

        #region DataContainer Load/Save

        DataContainer LoadDataContainer()
        {
            // 파일이 존재할 경우 불러오고
            if (File.Exists(SAVEFILE_PATH))
            {
                string json = File.ReadAllText(SAVEFILE_PATH);
                return JsonUtility.FromJson<DataContainer>(json);
            }

            // 파일이 존재하지 않을 경우 기본값으로 초기화
            return ResetDataContainer();
        }

        DataContainer ResetDataContainer()
        {
            var newDataContainer = new DataContainer
            {
                MasterVolume = 1.0f,
                SFXVolume = 0.5f, // 사운드 볼륨
                BGMVolume = 0.5f, // 배경 음악 볼륨
                StageClear = new bool[STAGE_COUNT * 2] // 스테이지 클리어 여부
            };

            return newDataContainer;
        }

        public void SaveDataContainer()
        {
            string json = JsonUtility.ToJson(CurrentDataContainer);
            File.WriteAllText(SAVEFILE_PATH, json);
        }
        #endregion
    }

}
