using UnityEngine;

[CreateAssetMenu]
public class SoundBank : ScriptableObject
{
    [SerializeField] AudioClip cuttingBoad;
    [SerializeField] AudioClip cooking;
    [SerializeField] AudioClip appear;
    [SerializeField] AudioClip pop;
    [SerializeField] AudioClip denied;


    public AudioClip GetAudioClip(ESoundEffect type)
    {
        switch (type)
        {
            case ESoundEffect.Cutting_Board:
                return cuttingBoad;
            case ESoundEffect.Pop:
                return pop;
            case ESoundEffect.Appear:
                return appear;
            case ESoundEffect.Cooking:
                return cooking;
            case ESoundEffect.Denied:
                return denied;
            default:
                Debug.LogError("사운드 추가 오류");
                return null;
        }
    }
}
