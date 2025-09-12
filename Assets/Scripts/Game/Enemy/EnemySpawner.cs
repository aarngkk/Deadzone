using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float minimumSpawnTime;
    [SerializeField] private float maximumSpawnTime;
    [SerializeField] private float spawnIncreaseInterval = 60f;
    [SerializeField] private int maxNumberOfSpawnIncreases = 5;

    private float lastSpawnIncreaseTime = 0f;
    private float timeUntilSpawn;
    private int numberOfSpawnIncreased = 0;
    private float minimumSpawnTimeDecrement;
    private float maximumSpawnTimeDecrement;


    void Awake()
    {
        SetTimeUntilSpawn();
        minimumSpawnTimeDecrement = minimumSpawnTime * 0.1f;
        maximumSpawnTimeDecrement = maximumSpawnTime * 0.1f;
    }

    void Update()
    {
        SpawnEnemies();        
        IncreaseSpawnRate();
    }

    public void SpawnOneEnemy()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }

    private void SpawnEnemies()
    {
        timeUntilSpawn -= Time.deltaTime;

        if (timeUntilSpawn <= 0)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            SetTimeUntilSpawn();
        }
    }

    private void SetTimeUntilSpawn()
    {
        timeUntilSpawn = Random.Range(minimumSpawnTime, maximumSpawnTime);
    }

    private void IncreaseSpawnRate()
    {
        float timeSinceLastSpawnIncrease = Time.time - lastSpawnIncreaseTime;
        if (timeSinceLastSpawnIncrease >= spawnIncreaseInterval && numberOfSpawnIncreased < maxNumberOfSpawnIncreases)
        {
            minimumSpawnTime -= minimumSpawnTimeDecrement;
            maximumSpawnTime -= maximumSpawnTimeDecrement;
            lastSpawnIncreaseTime = Time.time;
            numberOfSpawnIncreased++;
        }
    }
}
