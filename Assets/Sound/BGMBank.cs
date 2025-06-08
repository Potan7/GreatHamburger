using UnityEngine;

[CreateAssetMenu(fileName = "BGMBank", menuName = "BGMBank", order = 0)]
public class BGMBank : ScriptableObject
{
    public AudioClip[] bgmClips;

    public AudioClip GetRandomBGMClip()
    {
        int randomIndex = Random.Range(0, bgmClips.Length);
        return bgmClips[randomIndex];
    }
}