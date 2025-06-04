using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockCodingUIManager : MonoBehaviour
{
    public static BlockCodingUIManager instance { get; private set; }
    public SystemLanguage language;

    [SerializeField] private GameObject blockWindow;
    [SerializeField] private GameObject codeWindow;

    [SerializeField] private GameObject codeBlockSlots;
    [SerializeField] private GameObject codeContent;

    [SerializeField] private BlockCodeExecutor executor;

    public GameObject robot;
    public InteractableRegistry mapInfo;

    [SerializeField] private GameObject enterIDEBtn;
    [SerializeField] private GameObject backToKitchenBtn;
    public bool isInIDE;

    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 codingSpawnPosition;
    [SerializeField] private Vector3 kitchenSpawnPosition;
    public List<string> ingredientList { get; private set; }
    public List<string> nodeList { get; private set; }

    private void Awake()
    {
        instance = this;

        blockWindow.SetActive(true);
        codeWindow.SetActive(false);

        ingredientList = new();
        nodeList = new();
        if (mapInfo != null) 
        {
            nodeList = mapInfo.GetNodeInfos();
        }

        //테스트용
        for (int i = 0; i < 10; i++)
        {
            ingredientList.Add("igrd " + i);
        }

        enterIDEBtn.SetActive(!isInIDE);
        backToKitchenBtn.SetActive(isInIDE);
    }
    public List<List<CodeBlock>> GetSlotContents()
    {
        List<List<CodeBlock>> codeBlocks = new();
        for (int i = 0; i < codeBlockSlots.transform.childCount; i++)
        {
            List<CodeBlock> block = codeBlockSlots.transform.GetChild(i).GetComponent<CodeBlockSlot>().GetCodeContent();
            if (block == null) continue;
            codeBlocks.Add(block);
        }
        return codeBlocks;
    }

    public void SetCodeWindow() 
    {
        if (!codeWindow.activeSelf) return;

        List<List<CodeBlock>> codeBlocks = GetSlotContents();

        string indent = "";
        for (int i = 0; i < codeContent.transform.childCount; i++)
        {
            codeContent.transform.GetChild(i).gameObject.SetActive(codeBlocks.Count > i);
            if (codeBlocks.Count > i)
            {
                if (codeBlocks[i][0].codeBlockType == CodeBlockType.EndFor ||
                    codeBlocks[i][0].codeBlockType == CodeBlockType.EndIf ||
                    codeBlocks[i][0].codeBlockType == CodeBlockType.EndWhile)
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

                List<CodeBlockType> types = new();
                List<string> str = new();
                string mainBlock = blockName[(int)codeBlocks[i][0].codeBlockType];
                if (instance.language == SystemLanguage.Korean)
                {
                    mainBlock = koreanBlockName[(int)codeBlocks[i][0].codeBlockType];
                }
                types.Add(codeBlocks[i][0].codeBlockType);
                str.Add(mainBlock);
                for (int j = 1; j < codeBlocks[i].Count; j++)
                {
                    types.Add(codeBlocks[i][j].codeBlockType);
                    str.Add(codeBlocks[i][j].GetContents());
                }
                codeContent.transform.GetChild(i).GetComponent<CodeTextUI>().InitCodeTextUI(types, indent, str);

                if (codeBlocks[i][0].codeBlockType == CodeBlockType.For ||
                    codeBlocks[i][0].codeBlockType == CodeBlockType.If ||
                    codeBlocks[i][0].codeBlockType == CodeBlockType.While) 
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
            if (code == null || code.GetComponent<CodeTextUI>().blockTypes == null) break;
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
    public void OnClickStartCodeButton()
    {
        OnClickChangeWindowButton(0);
        executor.InitBlockCodeExecutor(GetBlockCode(), robot);
    }
    public void OnClickBackToKitchenButton()
    {
        isInIDE = false;
        enterIDEBtn.SetActive(!isInIDE);
        backToKitchenBtn.SetActive(isInIDE);

        player.transform.position = kitchenSpawnPosition;
    }
    public void OnClickEnterIDEButton()
    {
        isInIDE = true;
        enterIDEBtn.SetActive(!isInIDE);
        backToKitchenBtn.SetActive(isInIDE);

        player.transform.position = codingSpawnPosition;
    }

    public bool IsSentenceTypeBlock(CodeBlockType t)
    {
        if (t <= CodeBlockType.Continue) return true;
        return false;
    }
    public bool IsCompareTypeBlock(CodeBlockType t)
    {
        if (t > CodeBlockType.Continue && t < CodeBlockType.Count) return true;
        return false;
    }
    public bool IsValueTypeBlock(CodeBlockType t)
    {
        if (t >= CodeBlockType.Count) return true;
        return false;
    }

    public static string[] blockName =
           {
            "Interact",
            "For",
            "EndFor",
            "If",
            "EndIf",
            "While",
            "EndWhile",
            "Break",
            "Continue",
            "==",
            ">",
            "<",
            "Count",
            "Ingredient",
            "Node",
            "Count Value",
            "Ingredient Value",
            "Node Value",

        };
    public static string[] koreanBlockName =
        {
            "상호작용",
            "For",
            "EndFor",
            "If",
            "EndIf",
            "While",
            "EndWhile",
            "Break",
            "Continue",
            "==",
            ">",
            "<",
            "횟수",
            "재료",
            "장소",
            "횟수변수",
            "재료변수",
            "장소변수",

        };
}
public enum CodeBlockType
{
    Interact,

    For,
    EndFor,
    If,
    EndIf,
    While,
    EndWhile,
    Break,
    Continue,

    Same,
    Greater,
    Less,

    Count,
    Ingredient,
    Node,

    CVariable,
    IVariable,
    NVariable,
}
