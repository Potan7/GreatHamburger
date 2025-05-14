using UnityEngine;

[ExecuteAlways]
public class CodeBlockSlotPositionSetter : MonoBehaviour
{
    public float xInterval = 0.25f;
    public float yInterval = -0.25f;

    void Update()
    {
        for (int i = 0; i < transform.childCount; i++) 
        {
            transform.GetChild(i).localPosition = new Vector3(i * xInterval, i * yInterval, 0);
        }
    }
}