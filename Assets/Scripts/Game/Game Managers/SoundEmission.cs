using UnityEngine;

public class SoundEmission : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        EnemyMovement enemyMovement = collision.GetComponent<EnemyMovement>();
        Vector2 directionToSound = transform.position - collision.transform.position;

        if (enemyMovement)
        {
            enemyMovement.MoveTowards(directionToSound);
        }
    }
}
