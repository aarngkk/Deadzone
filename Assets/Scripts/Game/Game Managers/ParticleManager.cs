using System.Collections;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject particlesPrefab;
    [SerializeField] private Transform particlesTransform;
    [SerializeField] private bool randomiseRotation;

    public void SpawnParticles()
    {
        Vector3 particlesSpawnPoint = (particlesTransform != null) ? particlesTransform.position : transform.position;

        Quaternion rotation = randomiseRotation 
            ? Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) 
            : transform.rotation;

        Instantiate(particlesPrefab, particlesSpawnPoint, rotation);
    }
}
