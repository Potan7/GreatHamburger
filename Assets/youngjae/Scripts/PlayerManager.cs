using System;
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
    public static PlayerManager Instance { get; private set; }

    public PlayerIngredient selectIngredient = null;

    public Image fadeImage;
    public GameObject canvas;

    public bool isFadeIn = false;
    public XROrigin player;

    InputAction inputAction;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
}
