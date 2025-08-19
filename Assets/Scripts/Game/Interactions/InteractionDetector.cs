using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private List<IInteractable> interactablesInRange = new List<IInteractable>();
    private IInteractable closestInteractable = null;

    [SerializeField] private GameObject interactionIcon;
    [SerializeField] private float interactionIconOffset = 0.7f;

    void Start()
    {
        interactionIcon.SetActive(false);
    }

    void LateUpdate()
    {
        if (interactionIcon.activeSelf)
        {
            interactionIcon.transform.position = transform.position + Vector3.up * interactionIconOffset;
            interactionIcon.transform.rotation = Quaternion.identity;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {    
        if (context.performed)
        {
            float distanceToClosestInteractable = Mathf.Infinity;

            foreach (var interactable in interactablesInRange)
            {
                float distanceToInteractable = Vector2.Distance(transform.position, ((MonoBehaviour)interactable).gameObject.GetComponent<Collider2D>().ClosestPoint(transform.position));
                if (distanceToInteractable < distanceToClosestInteractable)
                {
                    distanceToClosestInteractable = distanceToInteractable;
                    closestInteractable = interactable;
                }
            }

            if (closestInteractable != null)
            {
                SetClosestInteractable(closestInteractable);
                closestInteractable.Interact();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.canInteract())
        {
            interactablesInRange.Add(interactable);
            if (!interactionIcon.activeSelf) interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            interactablesInRange.Remove(interactable);

            if (interactable == closestInteractable)
            {
                closestInteractable = null;
            }

            if (interactablesInRange.Count == 0)
            {
                interactionIcon.SetActive(false);
            }
        }
    }

    private void SetClosestInteractable(IInteractable interactable)
    {
        if (interactable.getInteractionIcon() != null)
        {
            interactionIcon.GetComponent<SpriteRenderer>().sprite = interactable.getInteractionIcon();
        }

        interactionIcon.SetActive(true);
    }
}
