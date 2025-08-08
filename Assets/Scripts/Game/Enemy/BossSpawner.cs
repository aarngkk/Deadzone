using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private int bossSpawnScore;
    private bool hasBossSpawned = false;

    private void OnEnable()
    {
        scoreController.OnScoreChanged.AddListener(HandleScoreChanged);
    }

    private void OnDisable()
    {
        scoreController.OnScoreChanged.RemoveListener(HandleScoreChanged);
    }

    private void HandleScoreChanged(int score)
    {
        if (!hasBossSpawned && score >= bossSpawnScore)
        {
            SpawnBoss();
            hasBossSpawned = true;
        }
    }

    private void SpawnBoss()
    {
        Instantiate (bossPrefab, transform.position, Quaternion.identity);
    }
}
