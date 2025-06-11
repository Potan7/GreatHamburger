using System;
using System.Collections.Generic;
using Audio;
using DG.Tweening;
using MapObject.Ingredients;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerManager>();
            }
            return instance;
        }
    }

    public HashSet<PlayerIngredient> selectedIngredients = new HashSet<PlayerIngredient>();

    public Image fadeImage;
    public GameObject canvas;

    public bool isFadeIn = false;
    public XROrigin player;

    InputAction inputAction;

    void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
            // player = GetComponent<XROrigin>();

            if (isFadeIn || canvas.activeSelf == true)
            {
                canvas.SetActive(true);
                fadeImage.color = Color.black;
                fadeImage.DOFade(0, 2f).OnComplete(() =>
                {
                    canvas.SetActive(false);
                });
            }

            inputAction = GetComponent<InputActionManager>().actionAssets[0].FindAction("XRI Custom/Menu");
            inputAction.performed += OnMenuInput;
        }
        else
        {
            Destroy(gameObject); // 이미 존재하는 경우 중복 객체 파괴
        }
    }

    private void OnMenuInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MenuManager.Instance.SetMenuToggle();
        }
    }

    public void FadeIn(Action onComplete = null)
    {
        AudioManager.StopBGMSound();

        canvas.SetActive(true);
        fadeImage.color = Color.clear;
        fadeImage.DOFade(1, 1.5f).OnComplete(() =>
        {
            canvas.SetActive(false);
            onComplete?.Invoke();
        });
    }

    void OnDestroy()
    {
        inputAction.performed -= OnMenuInput;
    }

    #region Player Ingredients Management

    public void ItemSelected(PlayerIngredient ingredient)
    {
        // Debug.Log("Selected: " + ingredient.name);
        selectedIngredients.Add(ingredient);
    }

    public void ItemDeselected(PlayerIngredient ingredient)
    {
        // Debug.Log("Deselected: " + ingredient.name);
        selectedIngredients.Remove(ingredient);
    }

    public int GetSelectedIngredientCount()
    {
        return selectedIngredients.Count;
    }

    public PlayerIngredient GetFirstSelectedIngredientWithItemDeselect(bool deselectOther = false)
    {
        if (selectedIngredients.Count == 0)
            return null;

        PlayerIngredient ingredient = selectedIngredients.First();
        ingredient.CancelSelection();
        selectedIngredients.Remove(ingredient);

        if (deselectOther)
        {
            var selectedIngredientsCopy = new HashSet<PlayerIngredient>(selectedIngredients);
            foreach (var item in selectedIngredientsCopy)
            {
                item.CancelSelection();
            }
            selectedIngredients.Clear();
        }

        return ingredient;
    }
    #endregion
}
