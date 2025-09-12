using UnityEngine;

public class EnemyDamagedFlash : MonoBehaviour
{
    [SerializeField] private float flashDuration;
    [SerializeField] private Color flashColor;
    [SerializeField] private Color damageResistantFlashColor;
    [SerializeField] private int numberOfFlashes;
    private HealthController healthController;

    private SpriteFlash spriteFlash;

    private void Awake()
    {
        spriteFlash = GetComponent<SpriteFlash>();
        healthController = GetComponent<HealthController>();
    }

    public void StartFlash()
    {
        Color color = (healthController.DamageResistance == 0f) ? flashColor : damageResistantFlashColor;
        spriteFlash.StartFlash(flashDuration, color, numberOfFlashes);
    }
}
