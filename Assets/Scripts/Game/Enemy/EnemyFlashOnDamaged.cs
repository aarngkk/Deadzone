using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlashOnDamaged : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bullet>() != null || collision.GetComponent<MeleeAttack>() != null)
        {
            StartCoroutine(FlashRed());
        }
    }

    public IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}
