using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CodeTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI codeText;
    [SerializeField] private GameObject codeBackground;

    private bool isRunning;
    private bool isError;
    private readonly Color NormalColor = new Color(0, 0, 0, 0);
    private readonly Color RunningColor = new Color(1, 1, 0, 1);
    private readonly Color ErrorColor = new Color(1, 0, 0, 1);
    public List<CodeBlockType> blockTypes { get; private set; }
    public List<string> currentTexts { get; private set; }

    public void InitCodeTextUI(List<CodeBlockType> types, string indent, List<string> str)
    {
        blockTypes = types;
        currentTexts = str;

        string text = indent;
        for (int i = 0; i < str.Count; i++)
        {
            string addText = str[i];
            if (types[i] == CodeBlockType.Ingredient)
            {
                BlockCodingUIManager.koreanBlockName.TryGetValue(BlockCodingUIManager.instance.ingredientList[int.Parse(str[i])], out var s);
                addText = s;
            }
            else if (types[i] == CodeBlockType.Node)
            {
                BlockCodingUIManager.koreanBlockName.TryGetValue(BlockCodingUIManager.instance.nodeList[int.Parse(str[i])], out var s);
                addText = s;
            }
            else if (types[i] == CodeBlockType.Hand)
            {
                addText = "Ме";
            }
            else if (BlockCodingUIManager.instance.IsVariableTypeBlock(types[i])) 
            {
                if (i == 0) 
                {
                    addText = addText + "  =  ";
                }
                else
                {
                    BlockCodingUIManager.koreanBlockName.TryGetValue(types[i].ToString(), out var s);
                    addText = s + addText;
                } 

            }
            text += addText + "  ";
        }
        codeText.text = text;

        isRunning = false;
        isError = false;
    }

    public void ResetTextColor()
    {
        codeBackground.GetComponent<Image>().color = NormalColor;
        codeBackground.SetActive(false);
    }
    public void EmphasizeRunningText(bool checkRunning) 
    {
        isRunning = checkRunning;
        if (isError) return;
        codeBackground.SetActive(checkRunning);

        Color c = NormalColor;
        if (isRunning) c = RunningColor;
        codeBackground.GetComponent<Image>().color = c;
    }
    public void EmphasizeErrorText(bool checkError)
    {
        isError = checkError;
        codeBackground.SetActive(checkError);

        Color c = NormalColor;
        if (isError) c = ErrorColor;
        codeBackground.GetComponent<Image>().color = c;
    }
}
