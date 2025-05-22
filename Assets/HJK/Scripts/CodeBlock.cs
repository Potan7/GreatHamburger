using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;

public class CodeBlock : MonoBehaviour
{
    public CodeBlockType codeBlockType { get; private set; }

    private void Awake()
    {
        GetComponent<XRGrabInteractable>().selectExited.AddListener(OnRelease);
    }
    public void InitCodeBlock(CodeBlockType type) 
    {
        codeBlockType = type;
        transform.GetChild(0).GetComponent<TextMeshPro>().text = type.ToString();
    }
    void OnRelease(SelectExitEventArgs args)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
