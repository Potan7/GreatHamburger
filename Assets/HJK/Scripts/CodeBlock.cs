using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;

public class CodeBlock : MonoBehaviour
{
    [SerializeField] private TextMeshPro blockText;
    [SerializeField] private GameObject upBtn;
    [SerializeField] private GameObject downBtn;
    public CodeBlockType codeBlockType { get; private set; }
    public CodeBlockSlot currentSlot;

    public int selectNumber { get; private set; }
    private int limitNumber = 10;
    private string blockName = "";
    
    private void Awake()
    {
        upBtn.SetActive(false);
        downBtn.SetActive(false);

        GetComponent<XRGrabInteractable>().selectExited.AddListener(OnRelease);
    }
    public void InitCodeBlock(CodeBlockType type) 
    {
        codeBlockType = type;
        currentSlot = null;

        string str = BlockCodingUIManager.blockName[(int)type];
        if (BlockCodingUIManager.instance.language == SystemLanguage.Korean) 
        {
            str = BlockCodingUIManager.koreanBlockName[(int)type];
        }
        blockText.text = str;
        if (!BlockCodingUIManager.instance.IsSentenceTypeBlock(codeBlockType)) 
        {
            Vector3 sc = transform.localScale;
            sc.x /= 2.0f;
            transform.localScale = sc;
            
            if (codeBlockType == CodeBlockType.Count ||
                codeBlockType == CodeBlockType.Ingredient ||
                codeBlockType == CodeBlockType.Node)
            {
                selectNumber = 0;
                upBtn.SetActive(true);
                OnClickBtn(0);
            }
        }
    }
    void OnRelease(SelectExitEventArgs args)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
    public string GetContents() 
    {
        if (codeBlockType == CodeBlockType.Count ||
            codeBlockType == CodeBlockType.Ingredient ||
            codeBlockType == CodeBlockType.Node)
        {
            return GetSelectedContents();
        }
        else if (codeBlockType == CodeBlockType.CVariable ||
                codeBlockType == CodeBlockType.IVariable ||
                codeBlockType == CodeBlockType.NVariable)
        {
            return blockName;
        }
        else if (codeBlockType == CodeBlockType.Same)
        {
            return "==";
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
        //stage 정보 확인 -> type에 맞는 리스트 받아오기 -> 리스트의 selectNumber번째 내용 반환

        return selectNumber.ToString();
    }

    public void OnClickBtn(int dir) //1 or -1
    {
        selectNumber += dir;
        if (selectNumber <= 0) 
        {
            selectNumber = 0;
        }
        if (selectNumber >= limitNumber)
        {
            selectNumber = limitNumber;
        }

        downBtn.SetActive(selectNumber > 0);
        upBtn.SetActive(selectNumber < limitNumber);
        blockText.text = selectNumber.ToString();
    }
}
