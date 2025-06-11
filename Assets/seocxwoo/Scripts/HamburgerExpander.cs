using UnityEngine;
using UnityEngine.InputSystem;

public class HamburgerExpander : MonoBehaviour
{
    public float spacing = 0.5f;
    public float expandSpeed = 5f;

    private bool isExpanded = false;
    private Vector3[] originalPos;

    void Start()
    {
        int childCount = transform.childCount;
        originalPos = new Vector3[childCount];

        for (int i = 0; i < childCount; i++)
        {
            originalPos[i] = transform.GetChild(i).localPosition;
        }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // »õ Input System ¹æ½Ä
        {
            isExpanded = !isExpanded;
        }

        AnimateChildren();
    }

    void AnimateChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 targetPos = isExpanded ? new Vector3(0, spacing * i, 0) : originalPos[i];

            child.localPosition = Vector3.Lerp(child.localPosition, targetPos, Time.deltaTime * expandSpeed);
        }
    }
}