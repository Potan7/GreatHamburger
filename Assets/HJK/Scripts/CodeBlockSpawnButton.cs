using UnityEngine;
using UnityEngine.XR;
using TMPro;

public class CodeBlockSpawnButton : MonoBehaviour
{
    public GameObject blockPrefab;
    public CodeBlockType codeBlockType;

    private void Start()
    {
        BlockCodingUIManager.koreanBlockName.TryGetValue(codeBlockType.ToString(), out var str);
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = str;
    }
    public void OnButtonClick()
    {
        if (!BlockCodingUIManager.instance.isInIDE) return;

        var block = Instantiate(blockPrefab, transform.position, transform.rotation);
        block.GetComponent<CodeBlock>().InitCodeBlock(codeBlockType);
    }
}
