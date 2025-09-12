using System.Collections;
using UnityEngine;

public class SpitterProjectile : MonoBehaviour
{
    [SerializeField] private float directDamage;
    [SerializeField] private float travelDistance;
    [SerializeField] private GameObject spitterPuddlePrefab;
    [SerializeField] private float destructibleObjectDamageFactor = 0.25f;
    [SerializeField] private LayerMask destructibleLayerMask;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {        
        StartCoroutine(spawnSpitPuddleAfterLifetime());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HealthController>() && !collision.GetComponent<EnemyMovement>())
        {
            HealthController healthController = collision.GetComponent<HealthController>();

            float damage = IsInDestructibleLayer(collision.gameObject.layer) ? directDamage * destructibleObjectDamageFactor : directDamage;
            healthController.TakeDamage(damage);
        }

        if (!collision.GetComponent<EnemyMovement>())
        {
            Instantiate(spitterPuddlePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);            
        }
    }

    private bool IsInDestructibleLayer(int layer)
    {
        return (destructibleLayerMask.value & (1 << layer)) != 0;
    }

    private IEnumerator spawnSpitPuddleAfterLifetime()
    {
        float lifetime = travelDistance / rb.linearVelocity.magnitude;
        yield return new WaitForSeconds(lifetime);
        Instantiate(spitterPuddlePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
