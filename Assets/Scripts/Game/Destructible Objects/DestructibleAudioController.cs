using UnityEngine;

public class DestructibleAudioController : MonoBehaviour
{
    [SerializeField] private AudioClip[] takeDamageAudioClips;
    [SerializeField] private float takeDamageAudioVolume = 1f;
    [SerializeField] private AudioClip[] destroyedAudioClips;
    [SerializeField] private float destroyedAudioVolume = 1f;

    public void PlayTakeDamageAudio()
    {
        SoundFXManager.instance.PlayNonRepeatingRandomClip(takeDamageAudioClips, transform, takeDamageAudioVolume, gameObject.name + "_Damaged");
    }

    public void PlayDestroyedAudio()
    {
        SoundFXManager.instance.PlayNonRepeatingRandomClip(destroyedAudioClips, transform, destroyedAudioVolume, gameObject.name + "_Destroyed");
    }
}
