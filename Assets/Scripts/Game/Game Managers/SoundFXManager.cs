using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private Dictionary<string, int> lastPlayedClipIndexMap = new Dictionary<string, int>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public AudioSource PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
        return audioSource;
    }

    public void PlayNonRepeatingRandomClip(AudioClip[] clips, Transform spawnTransform, float volume, string clipGroupID)
    {
        if (clips == null || clips.Length == 0) return;

        int lastIndex = lastPlayedClipIndexMap.ContainsKey(clipGroupID) ? lastPlayedClipIndexMap[clipGroupID] : -1;
        int newIndex;

        if (clips.Length == 1)
        {
            newIndex = 0;
        }
        else
        {
            do
            {
                newIndex = Random.Range(0, clips.Length);
            } while (newIndex == lastIndex);
        }

        lastPlayedClipIndexMap[clipGroupID] = newIndex;
        PlaySoundFXClip(clips[newIndex], spawnTransform, volume);
    }

    public AudioSource PlayLoopingSoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
        return audioSource;
    }
}
