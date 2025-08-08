using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float currentHealth;
    [SerializeField] private float maximumHealth;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip[] hurtAudioClips;
    [SerializeField] private float hurtVolume = 1f;
    [SerializeField] private AudioClip[] deathAudioClips;
    [SerializeField] private float deathVolume = 1f;

    public float RemainingHealthPercentage
    {
        get
        {
            return currentHealth / maximumHealth;
        }
    }

    public bool IsInvincible { get; set; }  

    public UnityEvent OnDied;

    public UnityEvent OnDamaged;

    public UnityEvent OnHealthChanged;

    public void TakeDamage(float damageAmount)
    {
        if (currentHealth == 0)
        {
            return;
        }
        
        if (IsInvincible)
        {
            return;
        }

        currentHealth -= damageAmount;

        OnHealthChanged.Invoke();

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Play hurt sound when player takes damage
        if (hurtAudioClips.Length > 0 && currentHealth > 0)
        {
            SoundFXManager.instance.PlayNonRepeatingRandomClip(hurtAudioClips, transform, hurtVolume, gameObject.name + "_Hurt");
        }

        //Play death sound when player dies
        if (deathAudioClips.Length > 0 && currentHealth == 0)
        {
            SoundFXManager.instance.PlayNonRepeatingRandomClip(deathAudioClips, transform, deathVolume, gameObject.name + "_Death");
        }

        if (currentHealth == 0)
        {
            OnDied.Invoke();
        }
        else
        {
            OnDamaged.Invoke();
        }
    }

    public void AddHealth(float amountToAdd)
    {
        if (currentHealth == maximumHealth)
        {
            return;
        }

        currentHealth += amountToAdd;

        OnHealthChanged.Invoke();

        if (currentHealth > maximumHealth)
        {
            currentHealth = maximumHealth;
        }
    }
}
