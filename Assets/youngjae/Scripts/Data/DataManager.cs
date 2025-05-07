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
                    instance.InitDataManager();
                }
                return instance;
            }
        }

        void InitDataManager()
        {
            Debug.Log("DataManager Initialized"); // 데이터 매니저 초기화 로그 출력
            DontDestroyOnLoad(instance.gameObject); // 씬 전환 시 파괴되지 않도록 설정
            LoadDataContainer(); // 데이터 컨테이너 로드
            savefilePath = Path.Combine(Application.persistentDataPath, SAVEFILE_NAME); // 저장 파일 경로 설정
        }

        void Start()
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        public const int STAGE_COUNT = 10; // 스테이지 개수
        public const string SAVEFILE_NAME = "data.json"; // 저장 파일 이름
        public string savefilePath;

        #region DataContainer

        [SerializeField]
        private DataContainer dataContainer;
        public DataContainer CurrentDataContainer
        {
            get => dataContainer;
            set => dataContainer = value;
        }

        public float MasterVolume
        {
            get => CurrentDataContainer.MasterVolume;
            set => CurrentDataContainer.MasterVolume = value;
        }

        public bool IsMasterVolumeOn
        {
            get => CurrentDataContainer.IsMasterVolumeOn;
            set => CurrentDataContainer.IsMasterVolumeOn = value;
        }

        public float SFXVolume
        {
            get => CurrentDataContainer.SFXVolume;
            set => CurrentDataContainer.SFXVolume = value;
        }

        public bool IsSFXVolumeOn
        {
            get => CurrentDataContainer.IsSFXVolumeOn;
            set => CurrentDataContainer.IsSFXVolumeOn = value;
        }

        public float BGMVolume
        {
            get => CurrentDataContainer.BGMVolume;
            set => CurrentDataContainer.BGMVolume = value;
        }

        public bool IsBGMVolumeOn
        {
            get => CurrentDataContainer.IsBGMVolumeOn;
            set => CurrentDataContainer.IsBGMVolumeOn = value;
        }

        public bool[] StageClear
        {
            get => CurrentDataContainer.StageClear;
            set => CurrentDataContainer.StageClear = value;
        }
        #endregion

        #region DataContainer Load/Save

        /// <summary>
        /// 데이터 컨테이너를 로드합니다.
        /// 파일이 존재하지 않을 경우 초기화합니다.
        /// </summary>
        void LoadDataContainer()
        {
            // 파일이 존재할 경우 불러오고
            if (File.Exists(savefilePath))
            {
                string json = File.ReadAllText(savefilePath);
                CurrentDataContainer = JsonUtility.FromJson<DataContainer>(json);
            }
            else
            {
                ResetDataContainer();
            }
        }

        /// <summary>
        /// 데이터 컨테이너를 초기화합니다.
        /// </summary>
        public void ResetDataContainer()
        {
            CurrentDataContainer = new DataContainer
            {
                StageClear = new bool[STAGE_COUNT * 2] // 스테이지 클리어 여부
            };
        }

        /// <summary>
        /// 데이터 컨테이너를 저장합니다.
        /// </summary>
        public void SaveDataContainer()
        {
            string json = JsonUtility.ToJson(CurrentDataContainer);
            File.WriteAllText(savefilePath, json);
        }

        private void OnApplicationQuit()
        {
            SaveDataContainer();
        }
        #endregion
    }

}
