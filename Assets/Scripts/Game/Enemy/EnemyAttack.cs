using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float damageAmount;
    [SerializeField] private float destructibleObjectDamageFactor;
    [SerializeField] private float attackDestructibleCooldown;
    [SerializeField] private string destructibleObjectLayerName;
    private int destructibleObjectLayer;
    private float lastDestructibleHitTime = -Mathf.Infinity;

    private void Awake()
    {
        destructibleObjectLayer = LayerMask.NameToLayer(destructibleObjectLayerName);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>())
        {
            var healthController = collision.gameObject.GetComponent<HealthController>();

            healthController.TakeDamage(damageAmount);
        }

        else if (collision.gameObject.layer == destructibleObjectLayer)
        {
            float timeSinceLastDestructibleHit = Time.time - lastDestructibleHitTime;

            if (timeSinceLastDestructibleHit > attackDestructibleCooldown)
            {
                var healthController = collision.gameObject.GetComponent<HealthController>();

                healthController.TakeDamage(damageAmount * destructibleObjectDamageFactor);

                lastDestructibleHitTime = Time.time;
            }
        }
    }
}
