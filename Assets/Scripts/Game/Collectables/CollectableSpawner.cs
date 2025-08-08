using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField]  private List<GameObject> collectablePrefabs;
    [SerializeField] private GameObject shotgunCollectablePrefab;

    public void SpawnCollectable(Vector2 position)
    {
        int index = Random.Range(0, collectablePrefabs.Count);
        var selectedCollectable = collectablePrefabs[index];

        Instantiate(selectedCollectable, position, Quaternion.identity);
    }

    public void SpawnShotgunCollectable(Vector2 position)
    {
        Instantiate(shotgunCollectablePrefab, position, Quaternion.identity);
    }
}
