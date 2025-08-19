using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RocketController : MonoBehaviour
{
    [SerializeField] private float explosionDamage;
    [SerializeField] private float explosionRadius;
    [SerializeField] private AudioClip explodeAudioClip;
    [SerializeField] private float explodeVolume = 1f;

    private CircleCollider2D explosionCollider;
    private PolygonCollider2D mapBounds;
    private Animator animator;
    public bool explosionTriggered {  get; private set; }
    private HashSet<HealthController> damagedTargets = new HashSet<HealthController>();

    private void Awake()
    {
        mapBounds = GameObject.FindWithTag("MapBounds").GetComponent<PolygonCollider2D>();
        explosionCollider = GetComponentInChildren<CircleCollider2D>();
        animator = GetComponent<Animator>();
        explosionCollider.radius = explosionRadius;
        explosionCollider.enabled = false;
    }

    private void Update()
    {
        DestroyWhenOutOfBounds();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!explosionTriggered)
        {
            TriggerExplosion();
        }

        var healthController = collision.gameObject.GetComponent<HealthController>();

        if (healthController == null || damagedTargets.Contains(healthController)) return;

        if (healthController.gameObject.GetComponent<PlayerMovement>())
        {
            healthController.IsInvincible = false;
            healthController.TakeDamage(explosionDamage * 0.75f);
        }
        else
        {
            healthController.TakeDamage(explosionDamage);
        }
        damagedTargets.Add(healthController);
    }

    private void TriggerExplosion()
    {
        explosionTriggered = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        animator.SetTrigger("Explode");
        SoundFXManager.instance.PlaySoundFXClip(explodeAudioClip, transform, explodeVolume);
        explosionCollider.enabled = true;
        Destroy(gameObject, 0.5f);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
