using UnityEngine;

public interface IInteractable
{
    void Interact();
    bool canInteract();
    Sprite getInteractionIcon();
}