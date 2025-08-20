using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private List<IInteractable> interactablesInRange = new List<IInteractable>();
    private IInteractable closestInteractable = null;

    [SerializeField] private GameObject interactionIconObject;
    [SerializeField] private Sprite defaultInteractionIcon;
    [SerializeField] private float interactionIconOffset = 0.7f;

    void Start()
    {
        interactionIconObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (interactionIconObject.activeSelf)
        {
            interactionIconObject.transform.position = transform.position + Vector3.up * interactionIconOffset;
            interactionIconObject.transform.rotation = Quaternion.identity;
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
                SetInteractableIcon(closestInteractable);
                closestInteractable.Interact();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.canInteract())
        {
            interactablesInRange.Add(interactable);
            SetInteractableIcon(interactable);
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
                interactionIconObject.SetActive(false);
            }
        }
    }

    private void SetInteractableIcon(IInteractable interactable)
    {
        if (interactable.getInteractionIcon() != null)
        {
            interactionIconObject.GetComponent<SpriteRenderer>().sprite = interactable.getInteractionIcon();
        }
        else
        {
            interactionIconObject.GetComponent<SpriteRenderer>().sprite = defaultInteractionIcon;
        }

        interactionIconObject.SetActive(true);
    }
}
