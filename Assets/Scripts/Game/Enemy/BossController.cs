using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private AudioClip bossFootstepsAudioClip;
    [SerializeField] private float bossFootstepsVolume = 1f;
    private AudioSource bossFootstepsAudio;
    private EnemySpawner[] enemySpawners;

    private void Start()
    {
        bossFootstepsAudio = SoundFXManager.instance.PlayLoopingSoundFXClip(bossFootstepsAudioClip, transform, bossFootstepsVolume);
        enemySpawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        stopEnemySpawn();
    }

    public void stopBossFootstepAudio()
    {
        bossFootstepsAudio.Stop();
        Destroy(bossFootstepsAudio.gameObject);
    }

    public void stopEnemySpawn()
    {
        foreach (EnemySpawner enemySpawner in enemySpawners)
        {
            enemySpawner.gameObject.SetActive(false);
        }
    }

    public void startEnemySpawn()
    {
        foreach (EnemySpawner enemySpawner in enemySpawners)
        {
            enemySpawner.gameObject.SetActive(true);
        }
    }
}
