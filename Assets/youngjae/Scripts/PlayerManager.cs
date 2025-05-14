using DG.Tweening;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Image fadeImage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // player = GetComponent<XROrigin>();

            if (fadeImage.gameObject.activeSelf == true)
            {
                fadeImage.DOFade(0, 1f).OnComplete(() =>
                {
                    fadeImage.gameObject.SetActive(false);
                });
            }
        }
        else
        {
            Destroy(gameObject); // 이미 존재하는 경우 중복 객체 파괴
        }
    }

    public XROrigin player;


}
