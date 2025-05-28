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
        public string text;
        public string imagePath;
        public string waitJob = WaitJob.None.ToString();
    }

    [System.Serializable]
    public class DescriptionTextList
    {
        public string title;
        public DescriptionText[] descriptions;
    }
}