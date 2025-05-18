using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public HingeJoint joint; // HingeJoint component for the door

    public float openAngle = 90f; // Angle to which the door will open

    [ContextMenu("Open Door")]
    public void OpenDoor()
    {
        joint.useSpring = true;

        JointLimits limits = joint.limits;
        limits.max = openAngle; // Set the maximum angle to open
        joint.limits = limits;

        joint.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        joint.GetComponent<Collider>().isTrigger = true; // Set the collider to trigger
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMapManager.Instance.ClearMap();
        }
    }
}