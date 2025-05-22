using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MapObject.Ingredients
{
    [RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
    public class Ingredient : MonoBehaviour
    {
        public XRGrabInteractable interactable;
        public Rigidbody rb;
        public new Collider collider;

        public event Action<Ingredient> OnIngredientCollision = null;

        private void Awake()
        {
            interactable = GetComponent<XRGrabInteractable>();
            rb = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();

            interactable.selectEntered.AddListener(OnSelected);
            interactable.selectExited.AddListener(OnDeselected);
        }

        private void OnDeselected(SelectExitEventArgs arg0)
        {
            // Debug.Log("Deselected");
            PlayerManager.Instance.selectIngredient = null;
        }

        private void OnSelected(SelectEnterEventArgs arg0)
        {
            // Debug.Log("Selected");
            PlayerManager.Instance.selectIngredient = this;
        }

        public void SetPosition(Vector3 position)
        {
            interactable.interactionManager.CancelInteractableSelection((IXRSelectInteractable)interactable);
            interactable.enabled = false;

            transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, 0));

            rb.constraints = RigidbodyConstraints.FreezeAll;
            PlayerManager.Instance.selectIngredient = null;
        }

        public void ReEnable()
        {
            interactable.enabled = true;
            rb.constraints = RigidbodyConstraints.None;
        }

        public virtual Ingredient DoCutting()
        {
            return this;
        }

        public virtual Ingredient DoCooking()
        {
            return this;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Ingredient ingredient))
            {
                // Debug.Log($"Ingredient collided with {ingredient.name}");
                OnIngredientCollision?.Invoke(ingredient);
            }
        }

        protected Ingredient ChangeNewIngredient(Ingredient newIngredient)
        {
            Ingredient cutted = this;
            if (newIngredient != null)
            {
                cutted = Instantiate(newIngredient, transform.position, transform.rotation, transform.parent);
            }
            PlayerManager.Instance.selectIngredient = null;

            gameObject.SetActive(false);
            Destroy(gameObject); // Destroy the ingredient after cutting

            return cutted;
        }
    }

}
