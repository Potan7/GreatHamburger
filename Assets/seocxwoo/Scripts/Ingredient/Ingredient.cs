using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] private IngredientData data;

    public string GetName()
    {
        return data.ingredientName;
    }

    public float GetHeight()
    {
        return data.height;
    }

    public int GetIndex()
    {
        return data.index;
    }
}
