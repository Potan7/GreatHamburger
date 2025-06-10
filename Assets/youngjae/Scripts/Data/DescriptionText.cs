using UnityEngine;
namespace Data
{
    public enum WaitJob
    {
        None,
        WaitForCutting,
        WaitForCooking,
        WaitForPlating
    }

    [System.Serializable]
    public class DescriptionText
    {
        /// <summary>
        /// 해당 창의 텍스트 내용입니다.
        /// </summary>
        public string text;

        /// <summary>
        /// 해당 창의 이미지 경로입니다.
        /// 경로는 Resources/{imagePath}로 설정되어 있습니다.
        /// 확장자는 쓰지 않습니다.
        /// </summary>
        public string imagePath;
        public string waitJob = WaitJob.None.ToString();
    }

    [System.Serializable]
    public class DescriptionTextList
    {
        /// <summary>
        /// 해당 설명창의 제목입니다.
        /// </summary>
        public string title;
        /// <summary>
        /// 설명창의 내용입니다. 배열의 각 항목이 이전, 다음 버튼으로 이동하는 칸입니다.
        /// </summary>
        public DescriptionText[] descriptions;
    }
}