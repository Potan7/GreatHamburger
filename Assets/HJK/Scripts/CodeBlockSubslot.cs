using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CodeBlockSubslot : CodeBlockSlot
{
    void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeInHierarchy) return;
        if (isFull) return;
        XRGrabInteractable grab = other.GetComponent<XRGrabInteractable>();
        if (grab == null) return;

        CodeBlockType type = grab.GetComponent<CodeBlock>().codeBlockType;
        if (BlockCodingUIManager.instance.IsSentenceTypeBlock(type)) return;

        if (transform.parent.GetComponent<CodeBlockSlot>().currentType == CodeBlockType.Interact && type != CodeBlockType.Node) return;
        if (transform.parent.GetComponent<CodeBlockSlot>().currentType == CodeBlockType.For && type != CodeBlockType.Count) return;
        if (transform.GetSiblingIndex() == 1 && !BlockCodingUIManager.instance.IsCompareTypeBlock(type)) return;
        if (transform.GetSiblingIndex() == 2 && transform.parent.GetChild(0).GetComponent<CodeBlockSlot>().currentSlotBlock != null) 
        {
            CodeBlockType sType = transform.parent.GetChild(0).GetComponent<CodeBlockSlot>().currentType;
            if (sType == CodeBlockType.Count && (type != CodeBlockType.CVariable && type != CodeBlockType.Count)) return;
            if (sType == CodeBlockType.CVariable && (type != CodeBlockType.CVariable && type != CodeBlockType.Count)) return;

            if (sType == CodeBlockType.Ingredient && (type != CodeBlockType.IVariable && type != CodeBlockType.Ingredient)) return;
            if (sType == CodeBlockType.IVariable && (type != CodeBlockType.IVariable && type != CodeBlockType.Ingredient)) return;

            if (sType == CodeBlockType.Node && (type != CodeBlockType.NVariable && type != CodeBlockType.Node)) return;
            if (sType == CodeBlockType.NVariable && (type != CodeBlockType.NVariable && type != CodeBlockType.Node)) return;
        }

        SetBlockIntoSlot(grab);
    }
}
