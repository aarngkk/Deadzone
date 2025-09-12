using UnityEngine;

public class EnemySpawnersController : MonoBehaviour
{
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private GameObject[] runnerSpawners, bomberSpawners, spitterSpawners, zombieSurvivorSpawners;
    [SerializeField] private int runnerActivationScore, bomberActivationScore, spitterActivationScore, zombieSurvivorActivationScore;
    private bool runnerSpawnActivated, bomberSpawnActivated, spitterSpawnActivated, zombieSurvivorSpawnActivated;

    public void ActivateSpawner(GameObject[] spawners)
    {
        foreach (GameObject spawner in spawners)
        {
            spawner.SetActive(true);
        }
    }

    public void ActivateWhenReachedScore(int score)
    {
        if (!runnerSpawnActivated && score >= runnerActivationScore)
        {
            foreach (GameObject runnerSpawner in runnerSpawners)
            {
                runnerSpawner.SetActive(true);
                runnerSpawner.GetComponent<EnemySpawner>().SpawnOneEnemy();
                runnerSpawnActivated = true;
            }
        }

        if (!spitterSpawnActivated && score >= spitterActivationScore)
        {
            foreach (GameObject spitterSpawner in spitterSpawners)
            {
                spitterSpawner.SetActive(true);
                spitterSpawner.GetComponent<EnemySpawner>().SpawnOneEnemy();
                spitterSpawnActivated = true;
            }
        }

        if (!zombieSurvivorSpawnActivated && score >= zombieSurvivorActivationScore)
        {
            foreach (GameObject zombieSurvivorSpawner in zombieSurvivorSpawners)
            {
                zombieSurvivorSpawner.SetActive(true);
                zombieSurvivorSpawner.GetComponent<EnemySpawner>().SpawnOneEnemy();
                zombieSurvivorSpawnActivated = true;
            }
        }
    }

    public void ActivateBomberOnBossDeath()
    {
        foreach (GameObject bomberSpawner in bomberSpawners)
        {
            bomberSpawner.SetActive(true);
            bomberSpawner.GetComponent<EnemySpawner>().SpawnOneEnemy();
        }
    }
}
