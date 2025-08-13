using System.Collections;
using UnityEngine;

public class ParticleFade : MonoBehaviour
{
    [SerializeField] private float particleDuration;
    [SerializeField] private float particleFadeDuration;

    private SpriteRenderer[] spriteRenderers;
    private Color[] startColors;

    private void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0) return;

        startColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            startColors[i] = spriteRenderers[i].color;
        }

        StartCoroutine(FadeParticles());
    }

    private IEnumerator FadeParticles()
    {
        yield return new WaitForSeconds(particleDuration);

        float elapsedTime = 0f;

        while (elapsedTime < particleFadeDuration)
        {
            float t = elapsedTime / particleFadeDuration;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color start = startColors[i];
                spriteRenderers[i].color = new Color(
                    start.r,
                    start.g,
                    start.b,
                    Mathf.Lerp(start.a, 0f, t)
                );
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
