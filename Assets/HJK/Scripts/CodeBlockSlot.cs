using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CodeBlockSlot : MonoBehaviour
{
    private bool isFull = false;
    GameObject currentSlotBlock = null;

    public CodeBlock GetCodeContent() 
    {
        if (currentSlotBlock == null) return null;

        return currentSlotBlock.GetComponent<CodeBlock>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (isFull) return;
        XRGrabInteractable grab = other.GetComponent<XRGrabInteractable>();
        if (grab == null) return;
        var interactor = grab.firstInteractorSelecting;
        if (interactor == null) return;
        isFull = true;
        currentSlotBlock = grab.gameObject;

        grab.GetComponent<CodeBlock>().StopAllCoroutines();
        grab.interactionManager.CancelInteractorSelection(interactor);

        other.transform.position = transform.position;
        other.transform.rotation = transform.rotation;

        if (other.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = true;
        }

        grab.selectEntered.AddListener(evt =>
        {
            grab.selectEntered.RemoveAllListeners();
            currentSlotBlock = null;
            StartCoroutine(SlotCoolTime());
        });
    }
    IEnumerator SlotCoolTime() 
    {
        yield return new WaitForSeconds(1.0f);
        isFull = false;
        yield break;
    }
}

