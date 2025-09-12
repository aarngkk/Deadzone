using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float damageAmount;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float destructibleObjectDamageFactor;
    [SerializeField] private float attackDestructibleCooldown;
    [SerializeField] private LayerMask destructibleObjectLayerMask;
    private bool attackOnCooldown;
    private float lastDestructibleHitTime = -Mathf.Infinity;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!attackOnCooldown)
        {
            StartCoroutine(DealDamage(collision));
        }
    }

    private IEnumerator DealDamage(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>())
        {
            var healthController = collision.gameObject.GetComponent<HealthController>();

            healthController.TakeDamage(damageAmount);
        }

        else if (IsInDestructibleLayer(collision.gameObject.layer))
        {
            float timeSinceLastDestructibleHit = Time.time - lastDestructibleHitTime;

            if (timeSinceLastDestructibleHit > attackDestructibleCooldown)
            {
                var healthController = collision.gameObject.GetComponent<HealthController>();

                healthController.TakeDamage(damageAmount * destructibleObjectDamageFactor);

                lastDestructibleHitTime = Time.time;
            }
        }

        attackOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        attackOnCooldown = false;
    }

    public IEnumerator Knockback(GameObject other, float distance, float knockbackSpeed)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) yield break;

        Vector2 direction = (other.transform.position - transform.position).normalized;
        Vector2 startPosition = other.transform.position;
        Vector2 targetPosition = startPosition + direction * distance;
        float knockbackTime = distance / knockbackSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < knockbackTime)
        {
            rb.MovePosition(Vector2.MoveTowards(other.transform.position, targetPosition, knockbackSpeed * Time.deltaTime));
            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }        
    }

    private bool IsInDestructibleLayer(int layer)
    {
        return (destructibleObjectLayerMask.value & (1 << layer)) != 0;
    }
}
