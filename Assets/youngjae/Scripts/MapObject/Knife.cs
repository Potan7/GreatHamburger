using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Data;
using MapObject.Ingredients;
using UnityEngine;

namespace MapObject
{
    public class Knife : MonoBehaviour
    {
        // Collider knifeCollider;
        public bool canCutting = false;
        bool cuttingCoolTime = false;
        public Rigidbody rb;

        void Start()
        {
            // knifeCollider = GetComponent<Collider>();
            rb = GetComponent<Rigidbody>();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!canCutting || cuttingCoolTime)
                return;

            // Debug.Log($"Knife collided with {collision.gameObject.name}");
            if (collision.gameObject.TryGetComponent(out PlayerIngredient ingredient))
            {
                FindAnyObjectByType<DescriptionPanel>().JobComplete(WaitJob.WaitForCutting);
                ingredient.DoCutting();
                WaitCoolTime().Forget();
            }
        }

        private async UniTaskVoid WaitCoolTime()
        {
            cuttingCoolTime = true;
            await UniTask.Delay(1000);
            cuttingCoolTime = false;
        }
    }
}