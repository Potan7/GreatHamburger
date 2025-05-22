using UnityEngine;

namespace MapObject.Ingredients
{
    public class OvenIngredient : PlayerIngredient
    {
        public PlayerIngredient cookedPrefab; // Prefab to instantiate when the ingredient is cooked

        public override PlayerIngredient DoCooking()
        {
            Debug.Log("Ingredient cooked in oven!");
            return ChangeNewIngredient(cookedPrefab);
        }
    }
}
