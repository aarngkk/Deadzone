using System.Collections;
using UnityEngine;

public class ZombieSpitterController : MonoBehaviour
{
    [SerializeField] private GameObject spitProjectilePrefab;
    [SerializeField] private GameObject spitPuddlePrefab;
    [SerializeField] private Transform projectileOffset;
    [SerializeField] private float spitProjectileSpeed;
    [SerializeField] private float spitAttackRange;
    [SerializeField] private float spitAttackCooldown;
    [SerializeField] private float spitAttackWindupDuration;
    private Animator animator;
    private PlayerAwarenessController playerAwarenessController;
    private bool isOnCooldown;
    private bool playerInRange;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip[] spitAttackWindupClips;
    [SerializeField] private float spitAttackWindupVolume = 1f;
    [SerializeField] private CircleCollider2D rangeCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerAwarenessController = GetComponent<PlayerAwarenessController>();
        rangeCollider.radius = spitAttackRange;
    }

    private void Update()
    {
        if (playerInRange && !isOnCooldown && playerAwarenessController.hasLineOfSight)
        {
            StartCoroutine(SpitAttack());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovement>())
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent <PlayerMovement>())
        {
            playerInRange = false;
        }
    }

    private IEnumerator SpitAttack()
    {
        isOnCooldown = true;
        EnemyMovement enemyMovement = GetComponent<EnemyMovement>();

        SoundFXManager.instance.PlayNonRepeatingRandomClip(spitAttackWindupClips, transform, spitAttackWindupVolume, gameObject.name + "_Windup");

        float speed = enemyMovement.Speed;
        enemyMovement.Speed = 0;
        animator.SetBool("IsRunning", false);

        yield return new WaitForSeconds(spitAttackWindupDuration);

        GameObject spitProjectile = Instantiate(spitProjectilePrefab, projectileOffset.position, transform.rotation);
        Rigidbody2D rigidbody = spitProjectile.GetComponent<Rigidbody2D>();
        rigidbody.linearVelocity = spitProjectileSpeed * transform.up;

        yield return new WaitForSeconds(spitAttackWindupDuration / 1.5f);
        enemyMovement.Speed = speed;
        animator.SetBool("IsRunning", true);

        yield return new WaitForSeconds(spitAttackCooldown);
        isOnCooldown = false;
    }

    public void SpawnSpitPuddle()
    {
        Instantiate(spitPuddlePrefab, transform.position, Quaternion.identity);
    }
}
