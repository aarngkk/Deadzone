using System.Collections;
using UnityEngine;

public class BossGameController : MonoBehaviour
{
    [SerializeField] private GameObject enemySpawners;
    [SerializeField] private GameObject[] zombieWaves;
    [SerializeField] private float wave1Health = 0.66f, wave2Health = 0.33f;
    [SerializeField] private float spawningDamageResistance = 0.75f;
    [SerializeField] private float spawnWaveRoarDuration = 2f;
    [SerializeField] private AudioClip bossFootstepsAudioClip;
    [SerializeField] private AudioClip bossFastFootstepsAudioClip;
    [SerializeField] private float bossFootstepsVolume = 1f;
    [SerializeField] private AudioClip[] spawnWaveRoarClips;
    [SerializeField] private float spawnWaveRoarVolume = 1f;
    public bool isSpawningWave {  get; private set; }
    private bool wave1Spawned, wave2Spawned;
    private HealthController healthController;
    private EnemyMovement enemyMovement;
    private float originalSpeed;
    private AudioSource bossFootstepsAudio;
    private Animator animator;
    private BossAttackController bossAttackController;

    private void Awake()
    {
        healthController = GetComponent<HealthController>();
        enemyMovement = GetComponent<EnemyMovement>();
        animator = GetComponent<Animator>();
        bossAttackController = GetComponent<BossAttackController>();
    }

    private void Start()
    {
        bossFootstepsAudio = SoundFXManager.instance.PlayLoopingSoundFXClip(bossFootstepsAudioClip, transform, bossFootstepsVolume);
        originalSpeed = enemyMovement.Speed;
        StopEnemySpawn();
    }

    public void CheckHealthThreshold()
    {
        float healthPercentage = healthController.remainingHealthPercentage;
        if (healthPercentage <= wave1Health && !wave1Spawned)
        {
            StartCoroutine(SpawnZombieWave(1));
            wave1Spawned = true;
        }

        if (healthPercentage <= wave2Health && !wave2Spawned)
        {
            StartCoroutine(SpawnZombieWave(2));
            wave2Spawned = true;
        }

    }

    public void stopBossFootstepAudio()
    {
        bossFootstepsAudio.Stop();
        Destroy(bossFootstepsAudio.gameObject);
    }

    public void StopEnemySpawn()
    {
            enemySpawners.SetActive(false);
    }

    private IEnumerator SpawnZombieWave(int waveNumber)
    {
        bossAttackController.InterruptChargeAttack();

        healthController.SetDamageResistance(spawningDamageResistance);
        SoundFXManager.instance.PlayNonRepeatingRandomClip(spawnWaveRoarClips, transform, spawnWaveRoarVolume, gameObject.name + "_Roar");
        enemyMovement.SetSpeed(0);
        isSpawningWave = true;
        animator.SetBool("IsIdle", true);
        zombieWaves[waveNumber - 1].SetActive(true);
        yield return new WaitForSeconds(spawnWaveRoarDuration);
        isSpawningWave = false;
        animator.SetBool("IsIdle", false);
        enemyMovement.SetSpeed(originalSpeed);
        healthController.SetDamageResistance(0f);
    }

    public void StartEnemySpawn()
    {
        enemySpawners.SetActive(true);
    }

    public void ToggleBossChargingAudio(bool value)
    {
        AudioClip clip = value ? bossFastFootstepsAudioClip : bossFootstepsAudioClip;
        bossFootstepsAudio.Stop();
        bossFootstepsAudio.clip = clip;
        bossFootstepsAudio.Play();
    }
}
