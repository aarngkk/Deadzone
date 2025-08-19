using UnityEngine;

public class DestructibleAudioController : MonoBehaviour
{
    [SerializeField] private AudioClip[] destroyedAudioClips;
    [SerializeField] private float destroyedAudioVolume = 1f;

    public void PlayDestroyedAudio()
    {
        SoundFXManager.instance.PlayNonRepeatingRandomClip(destroyedAudioClips, transform, destroyedAudioVolume, gameObject.name + "_Destroyed");
    }
}
