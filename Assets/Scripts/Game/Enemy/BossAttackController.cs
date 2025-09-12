using System.Collections;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    [SerializeField] private CircleCollider2D chargeAttackCollider;
    [SerializeField] private float chargeAttackRange;
    [SerializeField] private float chargeAttackDamage;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeRotationSpeed;
    [SerializeField] private float chargeDistance;
    [SerializeField] private float chargeCooldownDuration;
    [SerializeField] private float chargeStunDuration;
    [SerializeField] private float chargeKnockbackDistance;
    [SerializeField] private float chargeKnockbackSpeed;
    [SerializeField] private float chargeWindupDuration;
    [SerializeField] private AudioClip[] chargeAudioClips;
    [SerializeField] private float chargeAudioVolume = 1f;
    [SerializeField] private float destructibleObjectDamageFactor = 6f;
    [SerializeField] private LayerMask destructibleLayerMask;
    private EnemyMovement enemyMovement;
    private CircleCollider2D circleCollider;
    private PlayerAwarenessController playerAwarenessController;
    private EnemyAttack enemyAttack;
    private Animator animator;
    private BossGameController bossGameController;
    private HealthController bossHealthController;
    public bool isCharging { get; private set; }
    private bool chargeOnCooldown;
    private float originalColliderRadius, originalSpeed, originalRotationSpeed;
    private Coroutine chargeAttackCoroutine;
    private AudioSource chargeWindupAudio;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        circleCollider = GetComponent<CircleCollider2D>();
        playerAwarenessController = GetComponent<PlayerAwarenessController>();
        enemyAttack = GetComponent<EnemyAttack>();
        animator = GetComponent<Animator>();
        bossGameController = GetComponent<BossGameController>();
        bossHealthController = GetComponent<HealthController>();
        chargeAttackCollider.radius = chargeAttackRange;
        originalColliderRadius = circleCollider.radius;
        originalSpeed = enemyMovement.Speed;
        originalRotationSpeed = enemyMovement.RotationSpeed;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<HealthController>() && !collision.gameObject.GetComponent<EnemyMovement>() && isCharging)
        {
            HealthController healthController = collision.gameObject.GetComponent<HealthController>();            

            if (collision.gameObject.GetComponent<PlayerMovement>())
            {
                healthController.TakeDamage(chargeAttackDamage);
                StartCoroutine(enemyAttack.Knockback(collision.gameObject, chargeKnockbackDistance, chargeKnockbackSpeed));
                StartCoroutine(healthController.Stun(chargeStunDuration));
            }
            else if (IsInDestructibleLayer(collision.gameObject.layer))
            {
                healthController.TakeDamage(chargeAttackDamage * destructibleObjectDamageFactor);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!chargeOnCooldown && collision.GetComponent<PlayerMovement>() && playerAwarenessController.AwareOfPlayer && !bossGameController.isSpawningWave)
        {
            chargeAttackCoroutine = StartCoroutine(ChargeAttack());
        }
    }

    private IEnumerator ChargeAttack()
    {
        chargeOnCooldown = true;
        enemyMovement.Speed = 0;
        animator.SetBool("IsWindingUp", true);
        bossHealthController.SetDamageResistance(0.5f);

        chargeWindupAudio = SoundFXManager.instance.PlayNonRepeatingRandomClip(chargeAudioClips, transform, chargeAudioVolume, gameObject.name + "_Windup");
        yield return new WaitForSeconds(chargeWindupDuration);

        bossGameController.ToggleBossChargingAudio(true);
        animator.SetBool("IsWindingUp", false);
        animator.SetBool("IsCharging", true);
        isCharging = true;
        
        enemyAttack.enabled = false;        
        circleCollider.radius *= 1.5f;
        enemyMovement.Speed = chargeSpeed;
        enemyMovement.RotationSpeed = chargeRotationSpeed;

        float chargeTime = chargeDistance / chargeSpeed;
        yield return new WaitForSeconds(chargeTime);
        bossHealthController.SetDamageResistance(0f);
        bossGameController.ToggleBossChargingAudio(false);
        animator.SetBool("IsCharging", false);
        isCharging = false;        
        enemyAttack.enabled = true;

        circleCollider.radius = originalColliderRadius;
        enemyMovement.Speed = originalSpeed;
        enemyMovement.RotationSpeed = originalRotationSpeed;

        yield return new WaitForSeconds(chargeCooldownDuration);
        chargeOnCooldown = false;
    }

    public void InterruptChargeAttack()
    {
        if (chargeAttackCoroutine != null)
        {
            StopCoroutine(chargeAttackCoroutine);
            chargeOnCooldown = false;
            bossHealthController.SetDamageResistance(0f);
            isCharging = false;
            enemyAttack.enabled = true;
            circleCollider.radius = originalColliderRadius;
            enemyMovement.RotationSpeed = originalRotationSpeed;
            bossGameController.ToggleBossChargingAudio(false);

            if (chargeWindupAudio != null)
            {
                chargeWindupAudio.Stop();
                chargeWindupAudio = null;
            }
        }
    }

    private bool IsInDestructibleLayer(int layer)
    {
        return (destructibleLayerMask.value & (1 << layer)) != 0;
    }
}
