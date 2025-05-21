using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MapObject;
using MapObject.Ingredients;
using UnityEngine;
using UnityEngine.UI;

public class OvenInside : MonoBehaviour
{
    public Oven oven;
    public Transform ingredientPoint;
    public Image progressBar;

    public float cookingTime = 3f; // Time in seconds to cook the ingredient

    bool isCooking = false;
    Ingredient ingredient;

    void Start()
    {
        oven.OnDoorClosed += OnOvenClosed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCooking || !oven.isOvenDoorOpen)
            return;

        if (other.TryGetComponent(out Ingredient insideIngredient))
        {
            ingredient = insideIngredient;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (ingredient != null && ingredient.gameObject == other.gameObject)
        {
            ingredient = null;
        }

        if (isCooking)
        {
            isCooking = false;
        }
    }

    public void OnOvenClosed()
    {
        if (ingredient == null)
            return;

        PutIngredientToOven().Forget();
    }

    async UniTask PutIngredientToOven()
    {
        ingredient.SetPosition(ingredientPoint.position);
        isCooking = true;

        oven.ForceCloseOvenDoor();
        await UniTask.WaitWhile(() => oven.ovenHingeJoint.angle > 15);

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
        ingredient = ingredient.DoCooking();

        ingredient.ReEnable();

        oven.OpenDoor();
    }
}
