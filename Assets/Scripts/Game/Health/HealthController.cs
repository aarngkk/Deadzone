using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float currentHealth;
    [SerializeField] private float maximumHealth;
    [SerializeField] private float damageResistance = 0f;
    public float DamageResistance => damageResistance;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip[] hurtAudioClips;
    [SerializeField] private float hurtVolume = 1f;
    [SerializeField] private AudioClip[] deathAudioClips;
    [SerializeField] private float deathVolume = 1f;

    public bool isStunned;

    public float remainingHealthPercentage
    {
        get
        {
            return currentHealth / maximumHealth;
        }
    }

    public bool isInvincible { get; set; }

    public UnityEvent OnDied;

    public UnityEvent OnDamaged;

    public UnityEvent OnStunned;

    public UnityEvent OnHealthChanged;

    public void TakeDamage(float damageAmount)
    {
        if (currentHealth == 0 || isInvincible || damageAmount == 0)
        {
            return;
        }

        float damage = (damageResistance == 0) ? damageAmount : (damageAmount * (1 - damageResistance));
        currentHealth -= damage;

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

    public IEnumerator Stun(float stunDuration)
    {
        if (currentHealth == 0)
        {
            OnDied.Invoke();
        }
        else
        {
            isStunned = true;
            StunEffect(true);
            OnStunned.Invoke();

            yield return new WaitForSeconds(stunDuration);

            isStunned = false;
            StunEffect(false);
        }
    }

    private void StunEffect(bool stunned)
    {
        bool enableState = !stunned;

        if (GetComponent<EnemyMovement>())
        {
            EnemyMovement enemyMovement = GetComponent<EnemyMovement>();
            if (enemyMovement) enemyMovement.enabled = enableState;
            EnemyAttack enemyAttack = GetComponent<EnemyAttack>();
            if (enemyAttack) enemyAttack.enabled = enableState;
        }
        else if (GetComponent<PlayerMovement>())
        {
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement) playerMovement.enabled = enableState;
            PlayerShoot playerShoot = GetComponent<PlayerShoot>();
            if (playerShoot) playerShoot.enabled = enableState;
            PlayerMelee playerMelee = GetComponent<PlayerMelee>();
            if (playerMelee) playerMelee.enabled = enableState;
        }
    }

    public void SetDamageResistance(float resistance)
    {
        damageResistance = resistance;
    }
}
