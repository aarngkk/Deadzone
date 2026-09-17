using UnityEngine;

public class TurretAim : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] GameObject gunObject;
    [SerializeField] private LayerMask lineOfSightMask;
    [SerializeField] private bool targetPlayer;
    public bool hasTarget
    {
        get; private set;
    }
    private bool hasLineOfSight;

    void Update()
    {
        HandleAiming();
    }

    private void HandleAiming()
    {
        var collidersInRange = Physics2D.OverlapCircleAll(transform.position, range);
        GameObject closestEnemy = null;

        if (collidersInRange.Length != 0)
        {
            float closestEnemyDistance = Mathf.Infinity;

            foreach (Collider2D collider in collidersInRange)
            {
                EnemyMovement isEnemy = collider.GetComponent<EnemyMovement>();
                PlayerMovement isPlayer = collider.GetComponent<PlayerMovement>();

                if (isEnemy || (targetPlayer && isPlayer))
                {
                    Vector2 directionToEnemy = (collider.transform.position - transform.position).normalized;
                    float distanceToEnemy = Vector2.Distance(transform.position, collider.gameObject.transform.position);

                    RaycastHit2D ray = Physics2D.Raycast(transform.position, directionToEnemy, distanceToEnemy, lineOfSightMask);
                    hasLineOfSight = (isEnemy) ? (ray.collider != null && ray.collider.CompareTag("Enemy")) : (ray.collider != null && ray.collider.CompareTag("Player"));

                    if (distanceToEnemy < closestEnemyDistance && hasLineOfSight)
                    {
                        closestEnemyDistance = distanceToEnemy;
                        closestEnemy = collider.gameObject;
                    }
                }
            }

            if (closestEnemy != null)
            {
                hasTarget = true;
                Vector2 targetDirection = (closestEnemy.transform.position - transform.position).normalized;
                float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                gunObject.transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
            }
            else
            {
                hasTarget = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
