using System.Collections.Generic;
using System.Threading.Tasks;
using Audio;
using Cysharp.Threading.Tasks;
using Data;
using MapObject.Ingredients;
using UnityEngine;
using UnityEngine.UI;

namespace MapObject
{

    public class OvenInside : MonoBehaviour
    {
        public Oven oven;
        public Transform ingredientPoint;
        public Image progressBar;

        public float cookingTime = 3f; // Time in seconds to cook the ingredient

        // bool isCooking = false;
        readonly HashSet<PlayerIngredient> ingredients = new HashSet<PlayerIngredient>();

        void Start()
        {
            oven.OnDoorClosed += OnOvenClosed;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerIngredient insideIngredient))
            {
                ingredients.Add(insideIngredient);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PlayerIngredient insideIngredient))
            {
                ingredients.Remove(insideIngredient);
            }

            // if (isCooking)
            // {
            //     isCooking = false;
            // }
        }

        public void OnOvenClosed()
        {
            if (ingredients.Count == 0)
                return;

            PutIngredientToOven().Forget();
        }

        async UniTask PutIngredientToOven()
        {
            // isCooking = true;
            foreach (var ingredient in ingredients)
            {
                ingredient.SetInteractable(false);
            }

            oven.ForceCloseOvenDoor();
            await UniTask.WaitWhile(() => oven.ovenHingeJoint.angle > 15);

            AudioManager.MakeSoundEffect(ESoundEffect.Cooking, transform.position);
            progressBar.gameObject.SetActive(true);
            float time = 0;
            while (time < cookingTime)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / cookingTime;
                await UniTask.Yield();
            }

            progressBar.gameObject.SetActive(false);
            Debug.Log("Oven cooking done");

            var ingredientsCopy = new List<PlayerIngredient>(ingredients);

            foreach (var ingredient in ingredientsCopy)
            {
                ingredients.Remove(ingredient);
                var cookedIngredient = ingredient.DoCooking();
                ingredients.Add(cookedIngredient);
                cookedIngredient.SetInteractable(true);
            }

            FindAnyObjectByType<DescriptionPanel>().JobComplete(WaitJob.WaitForCooking);

            oven.OpenDoor();
        }
    }
}