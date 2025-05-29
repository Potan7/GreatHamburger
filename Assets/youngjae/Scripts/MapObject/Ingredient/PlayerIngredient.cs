using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MapObject.Ingredients
{
    [RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
    public class PlayerIngredient : MonoBehaviour
    {
        public XRGrabInteractable interactable { get;  private set; }
        public Rigidbody rb { get; private set; }
        public new Collider collider { get; private set; }

        public event Action<PlayerIngredient> OnIngredientCollision = null;
        public event Action<PlayerIngredient> OnIngredientSelected = null;
        public event Action<PlayerIngredient> OnIngredientDeselected = null;


        private void Awake()
        {
            interactable = GetComponent<XRGrabInteractable>();
            rb = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();

            interactable.selectEntered.AddListener(OnSelected);
            interactable.selectExited.AddListener(OnDeselected);

            OnIngredientSelected += PlayerManager.Instance.ItemSelected;
            OnIngredientDeselected += PlayerManager.Instance.ItemDeselected;
        }

        private void OnDeselected(SelectExitEventArgs arg0)
        {
            // Debug.Log("Deselected");
            // PlayerManager.Instance.ItemDeselected(this);
            OnIngredientDeselected?.Invoke(this);
        }

        private void OnSelected(SelectEnterEventArgs arg0)
        {
            // Debug.Log("Selected");
            // PlayerManager.Instance.ItemSelected(this);
            OnIngredientSelected?.Invoke(this);

            if (rb.constraints != RigidbodyConstraints.None)
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }

        public void SetPosition(Vector3 position, bool isFreeze = false)
        {
            CancelSelection();

            transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, 0));

            if (isFreeze)
            {
                SetFreeze(true);
            }
        }

        public void SetInteractable(bool isInteractable)
        {
            interactable.enabled = isInteractable;
        }

        public void SetFreeze(bool isFreeze)
        {
            if (isFreeze)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }

        public virtual PlayerIngredient DoCutting()
        {
            return this;
        }

        public virtual PlayerIngredient DoCooking()
        {
            return this;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out PlayerIngredient ingredient))
            {
                // Debug.Log($"Ingredient collided with {ingredient.name}");
                OnIngredientCollision?.Invoke(ingredient);
            }
        }

        protected PlayerIngredient ChangeNewIngredient(PlayerIngredient newIngredient)
        {
            PlayerIngredient cutted = this;
            if (newIngredient != null)
            {
                cutted = Instantiate(newIngredient, transform.position, transform.rotation, transform.parent);
            }
            PlayerManager.Instance.ItemDeselected(this);

            gameObject.SetActive(false);
            Destroy(gameObject); // Destroy the ingredient after cutting

            return cutted;
        }

        public void CancelSelection()
        {
            interactable.interactionManager.CancelInteractableSelection((IXRSelectInteractable)interactable);
        }
    }

}
