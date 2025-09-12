using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterPuddle : MonoBehaviour
{   
    [SerializeField] private float damagePerTick;
    [SerializeField] private float timePerTick;
    [SerializeField] private float enemyDamageFactor = 0.25f;
    [SerializeField] private float destructibleObjectDamageFactor = 0.25f;
    [SerializeField] private LayerMask destructibleLayerMask;
    [SerializeField] private AudioClip spitPuddleSizzlingClip;
    [SerializeField] private float spitPuddleSizzlingVolume = 1f;
    private List<Collider2D> collidersInPuddle = new List<Collider2D>();

    private void Start()
    {
        SoundFXManager.instance.PlaySoundFXClip(spitPuddleSizzlingClip, transform, spitPuddleSizzlingVolume);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<HealthController>() && !collision.GetComponent<ZombieSpitterController>() && !collidersInPuddle.Contains(collision))
        {
            collidersInPuddle.Add(collision);
            HealthController healthController = collision.GetComponent<HealthController>();
            StartCoroutine(DealDamage(collision, healthController));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collidersInPuddle.Remove(collision);
    }

    private IEnumerator DealDamage(Collider2D collider, HealthController healthController)
    {
        float damage = 0f;

        while (collidersInPuddle.Contains(collider))
        {
            if (IsInDestructibleLayer(collider.gameObject.layer))
            {
                damage = damagePerTick * destructibleObjectDamageFactor;
            }
            else
            {
                if (collider.GetComponent<PlayerMovement>())
                {
                    damage = damagePerTick;
                }
                else if (collider.GetComponent<EnemyMovement>())
                {
                    damage = damagePerTick * enemyDamageFactor;
                }                
            }

            healthController.TakeDamage(damage);
            yield return new WaitForSeconds(timePerTick);
        }
    }

    private bool IsInDestructibleLayer(int layer)
    {
        return (destructibleLayerMask.value & (1 << layer)) != 0;
    }
}
