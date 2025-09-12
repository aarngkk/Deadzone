using UnityEngine;

public class EnemyAudioController : MonoBehaviour
{
    [SerializeField] private AudioClip[] deathAudioClips;
    [SerializeField] private float deathVolume = 1f;

    public void playDeathAudio()
    {
        SoundFXManager.instance.PlayNonRepeatingRandomClip(deathAudioClips, transform, deathVolume, gameObject.name + "_Death");
    }
}
