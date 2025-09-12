using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private float meleeAttackDamage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HealthController>())
        {
            HealthController healthController = collision.GetComponent<HealthController>();
            healthController.TakeDamage(meleeAttackDamage);
        }
    }
}
