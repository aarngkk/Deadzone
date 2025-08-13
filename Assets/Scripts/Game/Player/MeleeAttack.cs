using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HealthController>())
        {
            HealthController healthController = collision.GetComponent<HealthController>();
            healthController.TakeDamage(10);
        }
    }
}
