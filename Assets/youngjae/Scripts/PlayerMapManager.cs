using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MapObject;
using MapObject.Ingredients;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMapManager : MonoBehaviour
{
    public static PlayerMapManager Instance { get; private set; }

    public List<GameObject> answerList = new();

    public GameObject answerCheckMark;

    public ExitDoor exitDoor;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 존재하는 경우 중복 객체 파괴
        }
    }

    public void CheckAnswer(List<PlayerIngredient> addedItemList)
    {
        if (answerList.Count != addedItemList.Count)
        {
            return;
        }
        for (int i = 0; i < answerList.Count; i++)
        {
            if (!addedItemList[i].name.Contains(answerList[i].name))
            {
                return;
            }
        }
        PlateSuccess();
    }

    void PlateSuccess()
    {
        answerCheckMark.SetActive(true);
        exitDoor.OpenDoor();
    }

    public void OnAddedItemToPlate(List<PlayerIngredient> addedItemList)
    {
        CheckAnswer(addedItemList);
    }

    public async void RestartGame()
    {
        var tcs = new UniTaskCompletionSource();
        var scene = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        scene.allowSceneActivation = false;
        PlayerManager.Instance.FadeIn(() =>
        {
            tcs.TrySetResult();
        });
        await tcs.Task;
        scene.allowSceneActivation = true;
    }

    public async void ClearMap()
    {
        var tcs = new UniTaskCompletionSource();
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        var scene = SceneManager.LoadSceneAsync(sceneIndex + 1);
        scene.allowSceneActivation = false;
        PlayerManager.Instance.FadeIn(() =>
        {
            tcs.TrySetResult();
        });
        await tcs.Task;
        scene.allowSceneActivation = true;
    }
}
