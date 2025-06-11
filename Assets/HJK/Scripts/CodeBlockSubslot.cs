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

        if (transform.parent.GetComponent<CodeBlockSlot>().currentType == CodeBlockType.Interact && (type != CodeBlockType.Node && type != CodeBlockType.NVariable)) return;
        if (transform.parent.GetComponent<CodeBlockSlot>().currentType == CodeBlockType.For && (type != CodeBlockType.Count && type != CodeBlockType.CVariable)) return;

        if (transform.parent.GetComponent<CodeBlockSlot>().currentType == CodeBlockType.CVariable && (type != CodeBlockType.Count && type != CodeBlockType.CVariable)) return;
        if (transform.parent.GetComponent<CodeBlockSlot>().currentType == CodeBlockType.IVariable && (type != CodeBlockType.Ingredient && type != CodeBlockType.Hand && type != CodeBlockType.IVariable)) return;
        if (transform.parent.GetComponent<CodeBlockSlot>().currentType == CodeBlockType.NVariable && (type != CodeBlockType.Node && type != CodeBlockType.NVariable)) return;

        if (transform.GetSiblingIndex() == 1 && !BlockCodingUIManager.instance.IsCompareTypeBlock(type)) return;
        if (transform.GetSiblingIndex() != 1 && BlockCodingUIManager.instance.IsCompareTypeBlock(type)) return;

        if (transform.GetSiblingIndex() == 2 && transform.parent.GetChild(0).GetComponent<CodeBlockSlot>().currentSlotBlock != null) 
        {
            CodeBlockType sType = transform.parent.GetChild(0).GetComponent<CodeBlockSlot>().currentType;
            if ((sType == CodeBlockType.Count || sType == CodeBlockType.CVariable) 
                && (type != CodeBlockType.Count && type != CodeBlockType.CVariable)) return;

            if ((sType == CodeBlockType.Ingredient || sType == CodeBlockType.Hand || sType == CodeBlockType.IVariable) 
                && (type != CodeBlockType.Ingredient && type != CodeBlockType.Hand && type != CodeBlockType.IVariable)) return;

            if ((sType == CodeBlockType.Node || sType == CodeBlockType.NVariable) 
                && (type != CodeBlockType.Node && type != CodeBlockType.NVariable)) return;
        }

        SetBlockIntoSlot(grab);
    }
}
