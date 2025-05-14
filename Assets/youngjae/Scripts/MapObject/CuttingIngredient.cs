using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MapObject
{
    public class CuttingIngredient : MonoBehaviour
    {
        public GameObject cuttedPrefab; // Prefab to instantiate when the ingredient is cut

        XRGrabInteractable interactable;
        Rigidbody rb;

        CuttingBoard cuttingBoard;

        void Start()
        {
            interactable = GetComponent<XRGrabInteractable>();
            rb = GetComponent<Rigidbody>();

            interactable.selectEntered.AddListener(OnSelected);
            interactable.selectExited.AddListener(OnDeselected);

            cuttingBoard = FindFirstObjectByType<CuttingBoard>();
        }

        private void OnDeselected(SelectExitEventArgs arg0)
        {
            Debug.Log("Deselected");
            if (cuttingBoard != null)
            {
                cuttingBoard.currentIngredient = null;
            }
        }

        private void OnSelected(SelectEnterEventArgs arg0)
        {
            Debug.Log("Selected");
            if (cuttingBoard != null)
            {
                cuttingBoard.currentIngredient = this;
            }
        }

        public void SetPosition(Vector3 position)
        {
            interactable.interactionManager.CancelInteractableSelection((IXRSelectInteractable)interactable);
            interactable.enabled = false;

            transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, 0));

            rb.constraints = RigidbodyConstraints.FreezeAll;

            cuttingBoard.currentIngredient = this;

        }

        public void ReEnable()
        {
            interactable.enabled = true;
            rb.constraints = RigidbodyConstraints.None;
        }

        void OnCollisionEnter(Collision collision)
        {
            // Debug.Log("Collision detected with: " + collision.gameObject.name);
            if (collision.gameObject.CompareTag("Knife"))
            {
                Debug.Log("Ingredient cut by knife!");
                if (cuttedPrefab != null)
                {
                    Instantiate(cuttedPrefab, transform.position, transform.rotation);
                    cuttedPrefab = null;
                }
                cuttingBoard.currentIngredient = null;

                gameObject.SetActive(false);
                Destroy(gameObject); // Destroy the ingredient after cutting

            }
        }
    }
}


