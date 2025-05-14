using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockCodingUIManager : MonoBehaviour
{
    [SerializeField] private GameObject blockWindow;
    [SerializeField] private GameObject codeWindow;

    [SerializeField] private GameObject codeBlockSlots;
    [SerializeField] private GameObject codeContent;

    private void Start()
    {
        blockWindow.SetActive(true);
        codeWindow.SetActive(false);
    }
    public void SetCodeWindow() 
    {
        if (!codeWindow.activeSelf) return;

        List<string> codeStrings = new List<string>();
        for (int i = 0; i < codeBlockSlots.transform.childCount; i++) 
        {
            string str = codeBlockSlots.transform.GetChild(i).GetComponent<CodeBlockSlot>().GetCodeContent();
            if (str == null) continue;
            codeStrings.Add(str);
        }

        for (int i = 0; i < codeContent.transform.childCount; i++)
        {
            codeContent.transform.GetChild(i).gameObject.SetActive(codeStrings.Count > i);
            if (codeStrings.Count > i) 
            {
                codeContent.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = codeStrings[i];
            }
        }
    }

    public void OnClickChangeWindowButton(int mode) 
    {
        bool isOnBlockWindow = (mode == 1);
        blockWindow.SetActive(isOnBlockWindow);
        codeWindow.SetActive(!isOnBlockWindow);
        SetCodeWindow();
    }
}
