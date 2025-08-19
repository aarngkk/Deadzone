using UnityEngine;

public class HomingRocket : MonoBehaviour
{
    [SerializeField] private float detectionRadius;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rocketSpeed;
    private Rigidbody2D rb;
    private RocketController rocketController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rocketController = GetComponent<RocketController>();
    }

    private void FixedUpdate()
    {
        if (rocketController != null && rocketController.explosionTriggered)
        {
            return;
        }

        HandleHoming();
    }

    private void HandleHoming()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        if (nearbyEnemies.Length == 0)
        {
            rb.linearVelocity = transform.up * rocketSpeed;
            return;
        }

        float closestEnemyDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var collider in nearbyEnemies)
        {
            EnemyMovement enemy = collider.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                float enemyDistance = Vector2.Distance(transform.position, enemy.transform.position);
                if (enemyDistance < closestEnemyDistance)
                {
                    closestEnemyDistance = enemyDistance;
                    closestEnemy = enemy.transform;
                }                    
            }
        }

        if (closestEnemy == null)
        {
            rb.linearVelocity = transform.up * rocketSpeed;
            return;
        }

        Vector2 direction = (closestEnemy.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float newAngle = Mathf.MoveTowardsAngle(
            rb.rotation,
            targetAngle,
            rotationSpeed * Time.deltaTime
            );

        rb.MoveRotation(newAngle);

        rb.linearVelocity = transform.up * rocketSpeed;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
