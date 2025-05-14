using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MapObject
{

    public class Plate : MonoBehaviour
    {
        private List<Transform> stackedIngredients = new List<Transform>();
        private BoxCollider plateCollider;

        void Start()
        {
            plateCollider = GetComponent<BoxCollider>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ingredient"))
            {
                StackIngredient(other.transform);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ingredient"))
            {
                StackIngredient(collision.transform);
            }
        }

        void StackIngredient(Transform ingredient)
        {
            // 이미 쌓인 재료인지 확인 (중복 스택 방지)
            if (stackedIngredients.Contains(ingredient))
            {
                return;
            }
            Debug.Log("Stacking ingredient: " + ingredient.name);

            // Rigidbody 비활성화하여 물리적 움직임 제어
            Rigidbody ingredientRb = ingredient.GetComponent<Rigidbody>();
            if (ingredientRb != null)
            {
                ingredientRb.isKinematic = true;
                ingredientRb.linearVelocity = Vector3.zero;
                ingredientRb.angularVelocity = Vector3.zero;
            }

            var interactable = ingredient.GetComponent<XRGrabInteractable>();
            if (interactable != null)
            {
                interactable.enabled = false;
            }

            // 재료의 위치 조정
            Quaternion rotation = Quaternion.Euler(0, 0, 0);

            Renderer ingredientRenderer = ingredient.GetComponent<Renderer>();
            if (ingredientRenderer != null)
            {
                Vector3 ingredientSize = ingredientRenderer.bounds.size;
                Vector3 plateSize = plateCollider.size;

                Vector3 newPosition = new Vector3(
                    transform.position.x,
                    transform.position.y + plateSize.y / 2 + ingredientSize.y / 2,
                    transform.position.z
                );
                
                plateCollider.size = new Vector3(plateSize.x, plateSize.y + ingredientSize.y, plateSize.z);
                ingredient.SetLocalPositionAndRotation(newPosition, rotation);
            }


            // 부모 설정 (선택 사항)
            ingredient.SetParent(transform);

            stackedIngredients.Add(ingredient);

            FindFirstObjectByType<ItemSpawner>().SpawnItem();
        }
    }
}