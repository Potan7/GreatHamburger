using UnityEngine;
using TMPro;

public class CodeTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI codeText;
    [SerializeField] private GameObject codeBackground;

    public CodeBlockType blockType { get; private set; }

    public void InitCodeTextUI(CodeBlockType type, string str) 
    {
        blockType = type;
        codeText.text = str;
    }

    public void EmphasizeText(bool highlight) 
    {
        codeBackground.SetActive(highlight);
    }
}
