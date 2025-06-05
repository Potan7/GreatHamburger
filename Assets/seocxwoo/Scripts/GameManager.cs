using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 전체를 인자로 받고 offset값 초기화 같이 해줘야함
    [SerializeField] private Transform plate;
    [SerializeField] private HamburgerData data;
    private List<int> recipe = new List<int>();

    public static GameManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        recipe = data.ingredientList;
    }

    public void CheckResult()
    {
        if (recipe.Count != plate.childCount)
        {
            Debug.LogWarning("Wrong Answer. Type 1");
            return;
        }

        for (int i = 0; i < recipe.Count; i++)
        {
            if (recipe[i] != plate.GetChild(i).GetComponent<Ingredient>().GetIndex())
            {
                Debug.LogWarning("Wrong Answer. Type 2");
                return;
            }
        }

        Debug.Log("Correct with Recipe.");
    }

    public void CleanPlate()
    {
        foreach (Transform child in plate)
        {
            Destroy(child.gameObject);
        }
    }
}