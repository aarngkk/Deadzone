using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float screenBorder;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;
    private Camera mainCamera;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        SetPlayerVelocity();
        RotateTowardsMouse();
        SetAnimation();
    }

    private void SetAnimation()
    {
        bool isMoving = movementInput != Vector2.zero;

        animator.SetBool("IsMoving", isMoving);
    }

    private void SetPlayerVelocity()
    {
        smoothedMovementInput = Vector2.SmoothDamp(smoothedMovementInput, movementInput, ref movementInputSmoothVelocity, 0.1f);
        rb.linearVelocity = movementInput * speed;

        PreventPlayerGoingOffScreen();
    }

    private void PreventPlayerGoingOffScreen()
    {
        Vector2 screenPosition = mainCamera.WorldToScreenPoint(transform.position); 

        if ((screenPosition.x < screenBorder && rb.linearVelocity.x < 0) || (screenPosition.x > mainCamera.pixelWidth - screenBorder && rb.linearVelocity.x > 0))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if ((screenPosition.y < screenBorder && rb.linearVelocity.y < 0) || (screenPosition.y > mainCamera.pixelHeight - screenBorder && rb.linearVelocity.y > 0))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 direction = (mouseWorldPosition - transform.position).normalized;

        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }

    private void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }
}
