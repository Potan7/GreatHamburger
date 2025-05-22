using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockCodingUIManager : MonoBehaviour
{
    [SerializeField] private GameObject blockWindow;
    [SerializeField] private GameObject codeWindow;

    [SerializeField] private GameObject codeBlockSlots;
    [SerializeField] private GameObject codeContent;

    [SerializeField] private BlockCodeExecutor executor;

    private void Start()
    {
        blockWindow.SetActive(true);
        codeWindow.SetActive(false);
    }
    public List<CodeBlock> GetSlotContents()
    {
        List<CodeBlock> codeBlocks = new List<CodeBlock>();
        for (int i = 0; i < codeBlockSlots.transform.childCount; i++)
        {
            CodeBlock block = codeBlockSlots.transform.GetChild(i).GetComponent<CodeBlockSlot>().GetCodeContent();
            if (block == null) continue;
            codeBlocks.Add(block);
        }
        return codeBlocks;
    }

    public void SetCodeWindow() 
    {
        if (!codeWindow.activeSelf) return;

        List<CodeBlock> codeBlocks = GetSlotContents();

        string indent = "";
        for (int i = 0; i < codeContent.transform.childCount; i++)
        {
            codeContent.transform.GetChild(i).gameObject.SetActive(codeBlocks.Count > i);
            if (codeBlocks.Count > i)
            {
                if (codeBlocks[i].codeBlockType == CodeBlockType.EndFor ||
                    codeBlocks[i].codeBlockType == CodeBlockType.EndIf ||
                    codeBlocks[i].codeBlockType == CodeBlockType.EndWhile)
                {
                    if (indent == "  ")
                    {
                        indent = "";
                    }
                    else
                    {
                        indent = indent.Substring(indent.Length - 2);
                    }
                }

                codeContent.transform.GetChild(i).GetComponent<CodeTextUI>().InitCodeTextUI(codeBlocks[i].codeBlockType, indent + codeBlocks[i].codeBlockType.ToString());

                if (codeBlocks[i].codeBlockType == CodeBlockType.For ||
                    codeBlocks[i].codeBlockType == CodeBlockType.If ||
                    codeBlocks[i].codeBlockType == CodeBlockType.While) 
                {
                    indent += "  ";
                }
            }
        }
    }
    public List<GameObject> GetBlockCode()
    {
        List<GameObject> blockCodes = new List<GameObject>();
        for (int i = 0; i < codeContent.transform.childCount; i++)
        {
            GameObject code = codeContent.transform.GetChild(i).gameObject;
            if (code == null) break;
            blockCodes.Add(code);
        }
        return blockCodes;
    }

    public void OnClickChangeWindowButton(int mode) 
    {
        bool isOnBlockWindow = (mode == 1);
        blockWindow.SetActive(isOnBlockWindow);
        codeWindow.SetActive(!isOnBlockWindow);
        SetCodeWindow();
    }
    public void OnClickBackToKitchenButton()
    {
        executor.InitBlockCodeExecutor(GetBlockCode());
    }
}
public enum CodeBlockType
{
    Move,
    Interact,

    For,
    EndFor,
    If,
    EndIf,
    While,
    EndWhile,

    Ingredient,
    Node,
    IVariable,
    NVariable,
}
