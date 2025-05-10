using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMelee : MonoBehaviour
{
    [SerializeField] public float meleeDuration;
    [SerializeField] public float meleeCooldown;

    public GameObject meleeHitbox;
    private Animator animator;
    private bool meleeContinuously;
    private float lastMeleeTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (meleeContinuously)
        {
            float timeSinceLastMelee = Time.time - lastMeleeTime;

            if (timeSinceLastMelee > meleeCooldown)
            {
                StartCoroutine(PerformMeleeAttack());

                lastMeleeTime = Time.time;

                animator.SetTrigger("IsMeleeAttacking");
            }
        }
    }

    private IEnumerator PerformMeleeAttack()
    {
        meleeHitbox.SetActive(true);
        yield return new WaitForSeconds(meleeDuration);
        meleeHitbox.SetActive(false);
    }

    private void OnMelee(InputValue inputValue)
    {
        meleeContinuously = inputValue.isPressed;
    }
}
