using UnityEngine;

public class Collectable : MonoBehaviour
{
    private ICollectableBehaviour collectableBehaviour;

    private void Awake()
    {
        collectableBehaviour = GetComponent<ICollectableBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerMovement>();

        if (player != null)
        {
            if (gameObject.GetComponent<HealthCollectableBehaviour>() != null && player.GetComponent<HealthController>().remainingHealthPercentage == 1)
            {
                return;
            }

            collectableBehaviour.OnCollected(player.gameObject);
            Destroy(gameObject);
        }
    }
}
