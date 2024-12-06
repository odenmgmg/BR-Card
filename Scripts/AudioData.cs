using UnityEngine;

[CreateAssetMenu]
public class AudioData : ScriptableObject
{
    [SerializeField]
    public AudioClip[] audioBGMClips, audioSEClips;
}
