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

    [SerializeField] private List<GameObject> interactables = new List<GameObject>();

    private List<InteractableObject> gameObjects = new List<InteractableObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject obj in interactables)
        {
            InteractableObject interactable = new InteractableObject();
            interactable.name = obj.name;
            interactable.transform = obj.transform;
            gameObjects.Add(interactable);
        }
    }

    public Transform GetTransform(string name)
    {
        foreach(InteractableObject interactable in gameObjects)
        {
            Debug.Log(name + " " + interactable.name); 
            if (interactable.name == name)
            {
                return interactable.transform;
            }
        }

        Debug.Log("Can't find " +  name);
        return null;
    }
}
