using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityController : MonoBehaviour
{
    private HealthController healthController;
    private SpriteFlash spriteflash;

    private void Awake()
    {
        healthController = GetComponent<HealthController>();
        spriteflash = GetComponent<SpriteFlash>();
    }

    public void StartInvincibility(float invincibilityDuration, Color flashColor, int numberOfFlashes)
    {
        StartCoroutine(InvincibilityCoroutine(invincibilityDuration, flashColor, numberOfFlashes));
    }

    private IEnumerator InvincibilityCoroutine(float invincibilityDuration, Color flashColor, int numberOfFlashes)
    {
        healthController.IsInvincible = true;
        yield return spriteflash.FlashCoroutine(invincibilityDuration, flashColor, numberOfFlashes);
        healthController.IsInvincible = false;
    }
}
