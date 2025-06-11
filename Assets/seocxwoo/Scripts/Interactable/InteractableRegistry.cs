using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InteractableRegistry : MonoBehaviour
{
    private class InteractableObject
    {
        public string name;
        public Transform transform;
    };

    public int stageNumber = 1;
    // 상호작용 가능한 오브젝트(ex. 도마, 화구 등)를 저장한 리스트
    [SerializeField] private List<GameObject> gameObjects = new List<GameObject>();

    // 앞선 리스트에서 정보(이름, Transform)를 저장하여 관리하는 리스트
    private List<InteractableObject> interactables = new List<InteractableObject>();

    [SerializeField] private List<IngredientData> ingredient = new List<IngredientData>();

    void Awake()
    {
        foreach (GameObject obj in gameObjects)
        {
            InteractableObject interactable = new InteractableObject();
            interactable.name = obj.name;
            interactable.transform = obj.transform;
            interactables.Add(interactable);
        }
    }

    public Transform GetTransform(string name)
    {
        foreach(InteractableObject interactable in interactables)
        {
            if (interactable.name == name)
            {
                return interactable.transform;
            }
        }

        Debug.LogWarning("Can't Find " +  name + ".");
        return null;
    }
    public List<string> GetNodeInfos() 
    {
        List<string> str = new();
        foreach (var i in interactables) 
        {
            str.Add(i.name);
        }
        return str;
    }
    public List<string> GetIngredientInfos()
    {
        List<string> str = new();
        foreach (var i in ingredient)
        {
            string s = i.name;
            if (s == "Bun_bottom" || s == "Bun_top") s = "Bun";
            str.Add(s);
        }
        return str;
    }
    public int GetIngredientIndex(string name)
    {
        for (int i = 0; i < ingredient.Count; i++) 
        {
            if (ingredient[i].ingredientName == name) return i;
        }
        return -1;
    }
}
