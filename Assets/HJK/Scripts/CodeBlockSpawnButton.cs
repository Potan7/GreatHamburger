using UnityEngine;
using UnityEngine.XR;
using TMPro;

public class CodeBlockSpawnButton : MonoBehaviour
{
    public GameObject blockPrefab;
    public CodeBlockType codeBlockType;

    private void Start()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = codeBlockType.ToString();
    }
    public void OnButtonClick()
    {
        var block = Instantiate(blockPrefab, transform.position, transform.rotation);
        block.GetComponent<CodeBlock>().InitCodeBlock(codeBlockType);
    }
}
