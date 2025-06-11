using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CodeBlockSlot : MonoBehaviour
{
    protected bool isFull = false;
    public GameObject currentSlotBlock { get; private set; }
    public CodeBlockType currentType/* { get; private set; }*/;

    [SerializeField] private List<GameObject> subslots;
    private void Awake()
    {
        currentSlotBlock = null;
        for (int i = 0; i < subslots.Count; i++) 
        {
            subslots[i].SetActive(false);
        }
    }
    public List<CodeBlock> GetCodeContent() 
    {
        if (currentSlotBlock == null) return null;

        List<CodeBlock> list = new();
        list.Add(currentSlotBlock.GetComponent<CodeBlock>());

        for (int i = 0; i < subslots.Count; i++)
        {
            List<CodeBlock> sub = subslots[i].GetComponent<CodeBlockSlot>().GetCodeContent();
            if (sub != null && sub[0] != null)
            {
                list.Add(sub[0]);
            }
        }

        return list;
    }
    void OnTriggerEnter(Collider other)
    {
        if (isFull) return;
        XRGrabInteractable grab = other.GetComponent<XRGrabInteractable>();
        if (grab == null) return;
        if (!BlockCodingUIManager.instance.IsSentenceTypeBlock(grab.GetComponent<CodeBlock>().codeBlockType) 
            && !BlockCodingUIManager.instance.IsVariableTypeBlock(grab.GetComponent<CodeBlock>().codeBlockType)) return;

        SetBlockIntoSlot(grab);
    }
    protected void SetBlockIntoSlot(XRGrabInteractable grab) 
    {
        var interactor = grab.firstInteractorSelecting;
        if (interactor == null) return;
        isFull = true;
        currentSlotBlock = grab.gameObject;
        currentSlotBlock.GetComponent<CodeBlock>().currentSlot = this;
        currentType = grab.GetComponent<CodeBlock>().codeBlockType;

        grab.GetComponent<CodeBlock>().StopAllCoroutines();
        grab.interactionManager.CancelInteractorSelection(interactor);

        grab.transform.position = transform.position;
        grab.transform.rotation = transform.rotation;

        if (grab.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = true;
        }

        grab.selectEntered.AddListener(ResetSlot);

        if (subslots.Count > 0)
        {
            int cnt = NeededSubslotCount(currentType);
            for (int i = 0; i < cnt; i++)
            {
                subslots[i].SetActive(true);
            }
        }

        grab.GetComponent<CodeBlock>().SetBlockSize();
        BlockCodingUIManager.instance.SetCodeWindow();
    }
    public void ResetSlot(SelectEnterEventArgs e)
    {
        if (currentSlotBlock == null) return;
        currentSlotBlock.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(ResetSlot);
        currentSlotBlock.GetComponent<CodeBlock>().currentSlot = null;
        currentSlotBlock = null;
        for (int i = 0; i < subslots.Count; i++)
        {
            subslots[i].GetComponent<CodeBlockSlot>().ResetSlot(null);
            subslots[i].SetActive(false);
        }
        StartCoroutine(SlotCoolTime());

        BlockCodingUIManager.instance.SetCodeWindow();
    }
    IEnumerator SlotCoolTime() 
    {
        yield return new WaitForSeconds(1.0f);
        isFull = false;
        for (int i = 0; i < subslots.Count; i++)
        {
            subslots[i].GetComponent<CodeBlockSlot>().isFull = false;
        }
        yield break;
    }
    private int NeededSubslotCount(CodeBlockType t) 
    {
        if (t == CodeBlockType.If ||
            t == CodeBlockType.While
            ) 
        {
            return 3;
        } 
        else if (t == CodeBlockType.Interact ||
                 t == CodeBlockType.For ||
                 t == CodeBlockType.CVariable ||
                 t == CodeBlockType.IVariable ||
                 t == CodeBlockType.NVariable) 
        {
            return 1;
        }
        return 0;
    }
}

