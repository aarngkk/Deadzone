using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ZombieBomberController : MonoBehaviour
{
    public UnityEvent OnExplode;

    private Transform player;
    private HealthController healthController;
    private Animator animator;
    private Coroutine explosionCoroutine;
    private AudioSource triggeredAudio;
    private EnemyMovement enemyMovement;
    private bool explosionTriggered = false;
    private bool isAggroed = false;

    [SerializeField] private CircleCollider2D explosionCollider;
    [SerializeField] private float explosionTriggerDistance;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDelay;
    [SerializeField] private float chainExplosionDelay;
    [SerializeField] private float explosionDamageAmount;
    [SerializeField] private float increasedSpeed;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip explodeAudioClip;
    [SerializeField] private float explodeVolume = 1f;
    [SerializeField] private AudioClip[] triggeredAudioClips;
    [SerializeField] private float triggeredVolume = 1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        healthController = GetComponent<HealthController>();
        player = FindFirstObjectByType<PlayerMovement>().transform;
        enemyMovement = GetComponent<EnemyMovement>();

        explosionCollider.radius = explosionRadius;
        explosionCollider.enabled = false;
    }

    void Update()
    {
        Vector2 enemyToPlayerVector = player.position - transform.position;

        if (enemyToPlayerVector.magnitude <= explosionTriggerDistance && !explosionTriggered)
        {
            Debug.Log("Tick, tock, tick, tock!");
            TriggerAggro();
            explosionTriggered = true;
            explosionCoroutine = StartCoroutine(ExplosionCoroutine(explosionDelay));
        }
    }

    private IEnumerator ExplosionCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Detonate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var healthController = collision.gameObject.GetComponent<HealthController>();

        if (healthController != null)
        {
            healthController.IsInvincible = false;
            healthController.TakeDamage(explosionDamageAmount);
        }
    }

    public void Detonate()
    {
        if (triggeredAudio != null)
        {
            triggeredAudio.Stop();
            Destroy(triggeredAudio);
            triggeredAudio = null;
        }

        explosionCollider.enabled = true;
        OnExplode.Invoke();
        SoundFXManager.instance.PlaySoundFXClip(explodeAudioClip, transform, explodeVolume);

        var nearbyBombers = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var collider in nearbyBombers)
        {
            var bomber = collider.GetComponent<ZombieBomberController>();
            if (bomber != null && bomber != this)
            {
                if (bomber.explosionCoroutine != null) bomber.StopCoroutine(bomber.explosionCoroutine);
                bomber.explosionTriggered = true;
                bomber.StartCoroutine(bomber.ExplosionCoroutine(chainExplosionDelay));
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionTriggerDistance);
    }

    public void TriggerAggro()
    {
        if (!isAggroed)
        {
            triggeredAudio = SoundFXManager.instance.PlayNonRepeatingRandomClip(triggeredAudioClips, transform, triggeredVolume, gameObject.name + "_Triggered");
            enemyMovement.Speed = increasedSpeed;
            animator.SetBool("IsRunning", true);
        }

        isAggroed = true;
    }
}
