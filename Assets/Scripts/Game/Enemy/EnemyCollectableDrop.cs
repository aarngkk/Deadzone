using UnityEngine;

public class EnemyCollectableDrop : MonoBehaviour
{
    [SerializeField] private float chanceOfCollectableDrop;

    private CollectableSpawner collectableSpawner;

    private void Awake()
    {
        collectableSpawner = FindFirstObjectByType<CollectableSpawner>();
    }

    public void RandomlyDropCollectable()
    {
        float random = Random.Range(0f, 1f);

        if (chanceOfCollectableDrop >= random)
        {
            collectableSpawner.SpawnCollectable(transform.position);
        }
    }

    public void BossShotgunDrop()
    {
        collectableSpawner.SpawnShotgunCollectable(transform.position);
    }
}
