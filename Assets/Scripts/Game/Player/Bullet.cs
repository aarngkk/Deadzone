using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private float damage;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        DestroyWhenOffScreen();
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

    private void DestroyWhenOffScreen()
    {
        Vector2 screenPosition = mainCamera.WorldToScreenPoint(transform.position);

        if (screenPosition.x < 0 || screenPosition.x > mainCamera.pixelWidth || screenPosition.y < 0 || screenPosition.y > mainCamera.pixelHeight)
        {
            Destroy(gameObject);
        }
    }
}
 