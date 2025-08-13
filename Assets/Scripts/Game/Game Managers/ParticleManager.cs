using System.Collections;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject particlesPrefab;
    [SerializeField] private bool randomiseRotation;

    public void SpawnParticles()
    {
        Quaternion rotation = randomiseRotation 
            ? Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) 
            : transform.rotation;

        Instantiate(particlesPrefab, transform.position, rotation);
    }
}
