using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MapObject.Ingredients
{
    public class CuttingIngredient : PlayerIngredient
    {
        public PlayerIngredient cuttedPrefab; // Prefab to instantiate when the ingredient is cut

        public override PlayerIngredient DoCutting()
        {
            Debug.Log("Ingredient cut by knife!");
            return ChangeNewIngredient(cuttedPrefab);
        }
    }
}


