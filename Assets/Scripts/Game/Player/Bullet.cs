using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private bool reduceDestructibleDamage;
    [SerializeField] private float destructibleObjectDamageFactor = 0.25f;
    [SerializeField] private LayerMask destructibleLayerMask;
    private PolygonCollider2D mapBounds;

    private void Awake()
    {
        mapBounds = GameObject.FindWithTag("MapBounds").GetComponent<PolygonCollider2D>();
    }

    private void Update()
    {
        DestroyWhenOutOfBounds();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HealthController>())
        {         
            HealthController healthController = collision.GetComponent<HealthController>();

            if (reduceDestructibleDamage)
            {
                float damageDealt = IsInDestructibleLayer(collision.gameObject.layer) ?  damage * destructibleObjectDamageFactor : damage;
                healthController.TakeDamage(damageDealt);
            }
            else
            {
                healthController.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private bool IsInDestructibleLayer(int layer)
    {
        return (destructibleLayerMask.value & (1 << layer)) != 0;
    }

    private void DestroyWhenOutOfBounds()
    {
        if (mapBounds != null)
        {
            if (!mapBounds.OverlapPoint(transform.position))
            {
                Destroy(gameObject);
            }
        }
    }
}
 