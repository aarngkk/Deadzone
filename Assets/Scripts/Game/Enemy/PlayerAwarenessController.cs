using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{
    public bool AwareOfPlayer { get; private set; }

    public Vector2 DirectionToPlayer { get; private set; }

    [SerializeField] public float playerAwarenessDistance;

    private Transform player;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>().transform;
    }

    void Update()
    {
        Vector2 enemyToPlayerVector = player.position - transform.position;
        DirectionToPlayer = enemyToPlayerVector.normalized;

        if (enemyToPlayerVector.magnitude <= playerAwarenessDistance)
        {
            AwareOfPlayer = true;
        }
        else
        {
            AwareOfPlayer = false;
        }
    }
}
