using UnityEngine;

namespace MapObject.Ingredients
{
    public class OvenIngredient : Ingredient
    {
        public Ingredient cookedPrefab; // Prefab to instantiate when the ingredient is cooked

        public override Ingredient DoCooking()
        {
            Debug.Log("Ingredient cooked in oven!");
            return ChangeNewIngredient(cookedPrefab);
        }
    }
}
