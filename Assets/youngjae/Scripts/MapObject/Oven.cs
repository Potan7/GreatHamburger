using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MapObject
{
    public class Oven : MonoBehaviour
    {
        public XRGrabInteractable ovenDoor;
        public HingeJoint ovenHingeJoint;
        Collider ovenDoorCollider;

        bool isGrabbed = false;
        XRBaseInteractor interactor;

        public float doorRange = 2f;
        public bool isOvenDoorOpen = false;

        public event Action OnDoorClosed;

        int defaultLayer;
        int toolLayer;


        void Start()
        {
            defaultLayer = LayerMask.NameToLayer("Default");
            toolLayer = LayerMask.NameToLayer("Tool");

            ovenDoor.selectEntered.AddListener(OnGrabbingOvenDoor);
            ovenDoor.selectExited.AddListener(OnReleasingOvenDoor);
            ovenDoorCollider = ovenDoor.GetComponent<Collider>();
        }

        void Update()
        {
            if (isGrabbed)
            {
                float distance = Vector3.Distance(interactor.transform.position, ovenDoor.transform.position);
                if (distance > doorRange)
                {
                    Debug.Log($"Oven door closed {distance}");
                    ovenDoor.interactionManager.CancelInteractableSelection((IXRSelectInteractable)ovenDoor);
                }
            }
        }

        private void OnReleasingOvenDoor(SelectExitEventArgs arg0)
        {
            float currentAngle = ovenHingeJoint.angle;
            if (currentAngle < 15)
            {
                ovenHingeJoint.useSpring = true;
                // Debug.Log("Oven door closed");
                isOvenDoorOpen = false;
                OnDoorClosed?.Invoke();
                ovenDoor.gameObject.layer = defaultLayer;
            }
            else
            {
                ovenDoor.gameObject.layer = toolLayer;
            }
            isGrabbed = false;
        }

        private void OnGrabbingOvenDoor(SelectEnterEventArgs arg0)
        {
            ovenHingeJoint.useSpring = false;
            interactor = arg0.interactorObject as XRBaseInteractor;
            isGrabbed = true;
            isOvenDoorOpen = true;
        }

        public void ForceCloseOvenDoor()
        {
            if (isGrabbed)
            {
                ovenDoor.interactionManager.CancelInteractableSelection((IXRSelectInteractable)ovenDoor);
                isGrabbed = false;
            }

            ovenDoor.enabled = false;
            ovenHingeJoint.useSpring = true;
            ovenDoor.gameObject.layer = defaultLayer;
        }

        public async void CloseDoor()
        {
            if (isGrabbed)
            {
                ovenDoor.interactionManager.CancelInteractableSelection((IXRSelectInteractable)ovenDoor);
                isGrabbed = false;
            }

            ovenDoor.enabled = false;
            ovenHingeJoint.useSpring = true;

            await UniTask.WaitWhile(() => ovenHingeJoint.angle > 15);
            ovenDoor.enabled = true;
            isOvenDoorOpen = false;
            OnDoorClosed?.Invoke();
            ovenDoor.gameObject.layer = defaultLayer;
        }

        public void OpenDoor()
        {
            ovenHingeJoint.useSpring = false;
            ovenDoor.enabled = true;
            ovenDoor.gameObject.layer = toolLayer;
        }
    }
}
