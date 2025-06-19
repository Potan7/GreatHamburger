using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockCodingUIManager : MonoBehaviour
{
    public const int VAR_LIMIT_CNT = 2;
    public static BlockCodingUIManager instance { get; private set; }
    public SystemLanguage language;

    [SerializeField] private GameObject blockWindow;
    [SerializeField] private GameObject codeWindow;

    [SerializeField] private GameObject codeBlockSlots;

    [SerializeField] private GameObject blockContent;
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

    private void Start()
    {
        instance = this;

        blockWindow.SetActive(true);
        codeWindow.SetActive(false);

        ingredientList = new();
        nodeList = new();
        if (mapInfo != null)
        {
            ingredientList = mapInfo.GetIngredientInfos();
            nodeList = mapInfo.GetNodeInfos();
            //Debug.LogError(nodeList.Count);
        }
        for (int i = 0; i < blockContent.transform.childCount; i++)
        {
            if (mapInfo.stageNumber < blockActiveLevel.GetValueOrDefault(blockContent.transform.GetChild(i).GetComponent<CodeBlockSpawnButton>().codeBlockType))
            {
                blockContent.transform.GetChild(i).gameObject.SetActive(false);
            }
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

    const string IDT = "      ";
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
                    if (indent == IDT)
                    {
                        indent = "";
                    }
                    else
                    {
                        indent = indent.Substring(indent.Length - IDT.Length);
                    }
                }

                List<CodeBlockType> types = new();
                List<string> str = new();
                string mainBlock = blockName[(int)codeBlocks[i][0].codeBlockType];
                if (!IsVariableTypeBlock(codeBlocks[i][0].codeBlockType))
                {
                    mainBlock = koreanBlockName.GetValueOrDefault(codeBlocks[i][0].codeBlockType.ToString());
                }
                else
                {
                    mainBlock = koreanBlockName.GetValueOrDefault(codeBlocks[i][0].codeBlockType.ToString()) + codeBlocks[i][0].GetContents();
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
                    indent += IDT;
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
        if (t >= CodeBlockType.Same && t <= CodeBlockType.Less) return true;
        return false;
    }
    public bool IsValueTypeBlock(CodeBlockType t)
    {
        if (t >= CodeBlockType.Count && t <= CodeBlockType.Node) return true;
        return false;
    }
    public bool IsVariableTypeBlock(CodeBlockType t)
    {
        if (t >= CodeBlockType.CVariable && t <= CodeBlockType.NVariable) return true;
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
            "Hand",

            "Count Value",
            "Ingredient Value",
            "Node Value",
        };
    private Dictionary<CodeBlockType, int> blockActiveLevel = new Dictionary<CodeBlockType, int>
    {
        { CodeBlockType.Interact, 1},

        { CodeBlockType.For, 5},
        { CodeBlockType.EndFor, 5},
        { CodeBlockType.If, 6},
        { CodeBlockType.EndIf, 6},
        { CodeBlockType.While, 7},
        { CodeBlockType.EndWhile, 7},

        { CodeBlockType.Break, 7},
        { CodeBlockType.Continue, 7},

        { CodeBlockType.Same, 6},
        { CodeBlockType.Greater, 6},
        { CodeBlockType.Less, 6},

        { CodeBlockType.Count, 5},
        { CodeBlockType.Ingredient, 6},
        { CodeBlockType.Node, 1},

        { CodeBlockType.Hand, 6},

        { CodeBlockType.CVariable, 10},
        { CodeBlockType.IVariable, 10},
        { CodeBlockType.NVariable, 10},
    };
    public static Dictionary<string, string> koreanBlockName = new Dictionary<string, string>
    {
        { "Interact", "상호작용"},

        { "For", "For" },
        { "EndFor", "EndFor"  },
        { "If", "If"  },
        { "EndIf", "EndIf"  },
        { "While", "While"  },
        { "EndWhile", "EndWhile"  },

        { "Break", "Break"  },
        { "Continue", "Continue"  },

        { "Same", "=="  },
        { "Greater", ">"  },
        { "Less", "<"  },
        { "= =", "= ="  },
        { ">", ">"  },
        { "<", "<"  },

        { "Count", "횟수"  },
        { "Ingredient", "재료"  },
        { "Node", "장소"  },
        { "Hand", "손에 든 재료"  },
        { "CVariable", "횟수변수"  },
        { "IVariable", "재료변수"  },
        { "NVariable", "장소변수"  },

        { "Crate_Buns", "빵 상자"  },
        { "Crate_Burgers", "고기 상자"  },
        { "Crate_Cheese", "치즈 상자"  },
        { "Crate_Lettuce", "양상추 상자"  },
        { "Crate_Tomatoes", "토마토 상자"  },
        { "Crate_Random", "랜덤 상자"  },
        { "Crate_Random_A", "랜덤 상자 A"  },
        { "Crate_Random_B", "랜덤 상자 B"  },
        { "CuttingBoard", "도마"  },
        { "Oven", "오븐"  },
        { "PlateTable", "제출 접시"  },
        { "Trashcan", "쓰레기통"  },

        { "Bun", "빵" },
        { "Burger_cooked", "구운 고기" },
        { "Burger_uncooked", "생고기" },
        { "Cheese", "치즈" },
        { "Cheese_cutted", "잘린 치즈" },
        { "Lettuce", "양상추" },
        { "Lettuce_cutted", "잘린 양상추" },
        { "Tomato", "토마토" },
        { "Tomato_cutted", "잘린 토마토" },
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

    Hand,

    CVariable,
    IVariable,
    NVariable,
}
