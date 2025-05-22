using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MapObject.Ingredients;
using UnityEngine;

namespace MapObject
{
    public class Knife : MonoBehaviour
    {
        Collider knifeCollider;
        public bool canCutting = false;
        bool cuttingCoolTime = false;

        void Start()
        {
            knifeCollider = GetComponent<Collider>();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!canCutting || cuttingCoolTime)
                return;

            // Debug.Log($"Knife collided with {collision.gameObject.name}");
            if (collision.gameObject.TryGetComponent(out PlayerIngredient ingredient))
            {
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