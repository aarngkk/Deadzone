using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage;
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
            healthController.TakeDamage(damage);  
        }

        Destroy(gameObject);
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
 