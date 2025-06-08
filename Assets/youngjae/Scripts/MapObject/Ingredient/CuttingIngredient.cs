using System;
using Audio;
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
            AudioManager.MakeSoundEffect(ESoundEffect.Cutting_Board, transform.position);
            Debug.Log("Ingredient cut by knife!");
            return ChangeNewIngredient(cuttedPrefab);
        }
    }
}


