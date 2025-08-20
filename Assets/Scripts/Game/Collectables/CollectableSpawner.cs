using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CollectableDrop
{
    public string name;
    public GameObject prefab;
    public float dropChance;
}

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField] private List<CollectableDrop> collectables;
    [SerializeField] private GameObject shotgunCollectablePrefab;

    public void SpawnCollectable(Vector2 position)
    {
        float totalChance = 0f;
        foreach (var item in collectables)
        {
            totalChance += item.dropChance;
        }

        float randomValue = UnityEngine.Random.value * totalChance;
        float cumulative = 0f;

        foreach (var item in collectables)
        {
            cumulative += item.dropChance;
            if (randomValue <= cumulative)
            {
                Instantiate(item.prefab, position, Quaternion.identity);
                return;
            }
        }
    }

    public void SpawnShotgunCollectable(Vector2 position)
    {
        Instantiate(shotgunCollectablePrefab, position, Quaternion.identity);
    }

    public void AddAmmoCollectableDrop(WeaponType weaponType, GameObject ammoPrefab, float dropChance)
    {
        string ammoName = weaponType.ToString() + " Ammo";

        if (!collectables.Exists(c => c.name == ammoName))
        {
            CollectableDrop newAmmoDrop = new CollectableDrop
            {
                name = ammoName,
                prefab = ammoPrefab,
                dropChance = dropChance
            };

            collectables.Add(newAmmoDrop);
        }
        else
        {
            Debug.LogWarning(ammoName + " already exists in the drop table.");
        }
    }
}
