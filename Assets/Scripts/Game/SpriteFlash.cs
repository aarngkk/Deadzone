using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        startColor = spriteRenderer.color;
    }

    public void StartFlash(float flashDuration, Color flashColor, int numberOfFlashes)
    {
        StopFlashCoroutine();
        flashCoroutine = StartCoroutine(FlashCoroutine(flashDuration, flashColor, numberOfFlashes));
    }

    public IEnumerator FlashCoroutine(float flashDuration, Color flashColor, int numberOfFlashes)
    {
        float elapsedFlashTime = 0;
        float elapsedFlashPercentage = 0;

        while (elapsedFlashTime < flashDuration)
        {
            elapsedFlashTime += Time.deltaTime;
            elapsedFlashPercentage = elapsedFlashTime / flashDuration;

            if (elapsedFlashPercentage > 1)
            {
                elapsedFlashPercentage = 1;
            }

            float pingPongPercentage = Mathf.PingPong(elapsedFlashPercentage * 2 * numberOfFlashes, 1);
            spriteRenderer.color = Color.Lerp(startColor, flashColor, pingPongPercentage);

            yield return null;
        }
    }

    private void StopFlashCoroutine()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
            spriteRenderer.color = startColor;
        }
    }
}
