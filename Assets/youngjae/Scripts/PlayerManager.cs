using System;
using DG.Tweening;
using MapObject.Ingredients;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Ingredient selectIngredient = null;

    public Image fadeImage;
    public GameObject canvas;

    public bool isFadeIn = false;
    public XROrigin player;

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
        }
        else
        {
            Destroy(gameObject); // 이미 존재하는 경우 중복 객체 파괴
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

}
