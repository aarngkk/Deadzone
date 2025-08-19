using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TurretAim : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] GameObject gunObject;
    public bool hasTarget
    {
        get; private set;
    }

    void Update()
    {
        HandleAiming();
    }

    private void HandleAiming()
    {
        var collidersInRange = Physics2D.OverlapCircleAll(transform.position, range);
        Vector2 direction = Vector2.zero;

        if (collidersInRange.Length != 0)
        {
            float closestEnemyDistance = Mathf.Infinity;
            GameObject closestEnemy = null;

            foreach (var collider in collidersInRange)
            {
                var enemyInRange = collider.GetComponent<EnemyMovement>();
                if (enemyInRange != null)
                {
                    var distanceToEnemy = Vector2.Distance(transform.position, enemyInRange.gameObject.transform.position);

                    if (distanceToEnemy < closestEnemyDistance)
                    {
                        closestEnemyDistance = distanceToEnemy;
                        closestEnemy = enemyInRange.gameObject;
                    }
                }
            }

            if (closestEnemy != null)
            {
                hasTarget = true;
                direction = (closestEnemy.transform.position - transform.position).normalized;
            }

            else
            {
                hasTarget = false;
            }
        }

        if (direction != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gunObject.transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
