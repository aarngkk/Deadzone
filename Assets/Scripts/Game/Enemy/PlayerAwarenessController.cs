using System.Collections;
using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{
    public bool AwareOfPlayer { get; private set; }

    public Vector2 DirectionToPlayer { get; private set; }

    [SerializeField] public float playerAwarenessDistance;
    [SerializeField] public float fieldOfViewAngle = 90f;
    [SerializeField] private float timeBeforeLosePlayer;
    [SerializeField] private LayerMask lineOfSightMask;

    public bool hasLineOfSight { get; private set; }
    private Transform player;
    private Coroutine losePlayerCoroutine = null;

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
            float angleToPlayer = Vector2.Angle(enemyToPlayerVector, transform.up);
            if (angleToPlayer <= fieldOfViewAngle * 0.5f && hasLineOfSight)
            {
                if (losePlayerCoroutine != null)
                {
                    StopCoroutine(losePlayerCoroutine);
                    losePlayerCoroutine = null;
                }
                AwareOfPlayer = true;
            }
        }
        else if (AwareOfPlayer)
        {
            if (losePlayerCoroutine == null) losePlayerCoroutine = StartCoroutine(LosePlayer());
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, distance, lineOfSightMask);

        hasLineOfSight = (ray.collider != null && ray.collider.CompareTag("Player"));
    }

    private IEnumerator LosePlayer()
    {
        yield return new WaitForSeconds(timeBeforeLosePlayer);
        AwareOfPlayer = false;
        losePlayerCoroutine = null;
    }

    public void IncreaseAwareness(float awarenessDistance)
    {
        playerAwarenessDistance = awarenessDistance;
    }
}
