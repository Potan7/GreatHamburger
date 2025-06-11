using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CodeBlock : MonoBehaviour
{
    [SerializeField] private TextMeshPro blockText;
    [SerializeField] private GameObject upBtn;
    [SerializeField] private GameObject downBtn;
    public CodeBlockType codeBlockType { get; private set; }
    public CodeBlockSlot currentSlot;

    private const float DEFAULT_SIZE = 0.5f;

    public int selectNumber { get; private set; }
    private int limitNumber = 0;
    private string blockName = "";
    
    private void Awake()
    {
        if (upBtn != null && upBtn != null)
        {
            upBtn.SetActive(false);
            downBtn.SetActive(false);
            upBtn.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
            downBtn.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }

        GetComponent<XRGrabInteractable>().selectExited.AddListener(OnRelease);
    }
    public void InitCodeBlock(CodeBlockType type) 
    {
        codeBlockType = type;
        currentSlot = null;

        string str = BlockCodingUIManager.blockName[(int)type];
        if (BlockCodingUIManager.instance.language == SystemLanguage.Korean) 
        {
            str = BlockCodingUIManager.koreanBlockName.GetValueOrDefault(type.ToString());
        }
        blockText.text = str;
        if (!BlockCodingUIManager.instance.IsSentenceTypeBlock(codeBlockType))
        {
            if (BlockCodingUIManager.instance.IsValueTypeBlock(codeBlockType) ||
                (BlockCodingUIManager.instance.IsVariableTypeBlock(codeBlockType))
                )
            {
                selectNumber = 0;
                upBtn.SetActive(true);
                OnClickBtn(0);
            }
        }

        SetBlockSize();
    }
    void OnRelease(SelectExitEventArgs args)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
    public string GetContents() 
    {
        if (BlockCodingUIManager.instance.IsValueTypeBlock(codeBlockType))
        {
            return GetSelectedContents();
        }
        else if (BlockCodingUIManager.instance.IsVariableTypeBlock(codeBlockType))
        {
            return blockName;
        }
        else if (codeBlockType == CodeBlockType.Hand)
        {
            return "Hand";
        }
        else if (codeBlockType == CodeBlockType.Same)
        {
            return "= =";
        }
        else if (codeBlockType == CodeBlockType.Greater)
        {
            return ">";
        }
        else if (codeBlockType == CodeBlockType.Less)
        {
            return "<";
        }
        else return null;
    }
    public string GetSelectedContents()
    {
        return selectNumber.ToString();
    }

    public void OnClickBtn(int dir) //1 or -1
    {
        if (codeBlockType == CodeBlockType.Count) limitNumber = 10;
        else if (codeBlockType == CodeBlockType.Ingredient) limitNumber = BlockCodingUIManager.instance.ingredientList.Count;
        else if (codeBlockType == CodeBlockType.Node) limitNumber = BlockCodingUIManager.instance.nodeList.Count;
        else if (BlockCodingUIManager.instance.IsVariableTypeBlock(codeBlockType)) limitNumber = BlockCodingUIManager.VAR_LIMIT_CNT;

        selectNumber += dir;
        if (selectNumber <= 0) 
        {
            selectNumber = 0;
        }
        if (selectNumber >= limitNumber - 1)
        {
            selectNumber = limitNumber - 1;
        }

        downBtn.SetActive(selectNumber > 0);
        upBtn.SetActive(selectNumber < limitNumber - 1);

        SetBlockText();
    }

    public void SetBlockSize()
    {
        Vector3 sc = transform.localScale;
        sc.x = DEFAULT_SIZE;
        if (BlockCodingUIManager.instance.IsValueTypeBlock(codeBlockType) ||
            BlockCodingUIManager.instance.IsCompareTypeBlock(codeBlockType) ||
            codeBlockType == CodeBlockType.Hand ||
            (BlockCodingUIManager.instance.IsVariableTypeBlock(codeBlockType) && (currentSlot != null && currentSlot.transform.parent.childCount == 3)))
        {
            sc.x /= 2.0f;
            OnClickBtn(0);
        }
        transform.localScale = sc;

        SetBlockText();
    }
    private void SetBlockText()
    {
        if (codeBlockType == CodeBlockType.Count)
        {
            blockText.text = selectNumber.ToString();
        }
        else if (codeBlockType == CodeBlockType.Ingredient)
        {
            BlockCodingUIManager.koreanBlockName.TryGetValue(BlockCodingUIManager.instance.ingredientList[selectNumber], out var text);
            blockText.text = text;
        }
        else if (codeBlockType == CodeBlockType.Node)
        {
            BlockCodingUIManager.koreanBlockName.TryGetValue(BlockCodingUIManager.instance.nodeList[selectNumber], out var text);
            blockText.text = text;
        }
        else if (codeBlockType == CodeBlockType.CVariable || codeBlockType == CodeBlockType.IVariable || codeBlockType == CodeBlockType.NVariable)
        {
            BlockCodingUIManager.koreanBlockName.TryGetValue(codeBlockType.ToString(), out var text);
            blockName = ((char)('A' + selectNumber)).ToString();

            string assignOperator = "  =  ";
            if ((currentSlot != null && currentSlot.transform.parent.childCount == 3)) assignOperator = "";
            blockText.text = text + "  " + blockName + assignOperator;
        }
        BlockCodingUIManager.instance.SetCodeWindow();
    }
}
