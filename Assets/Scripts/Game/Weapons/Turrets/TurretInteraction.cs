using UnityEngine;

public class TurretInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private int ammoPerRefill = 15;
    [SerializeField] private Sprite interactionSprite;
    private TurretShoot turretShoot;
    private Coroutine reloadCoroutine;

    void Start()
    {
        turretShoot = GetComponent<TurretShoot>();
    }

    public bool canInteract()
    {
        return !turretShoot.isReloading && !turretShoot.HasMaxAmmo;
    }

    public void Interact()
    {
        if (!canInteract()) return;
        RefillAmmo();
    }

    private void RefillAmmo()
    {
        reloadCoroutine = StartCoroutine(turretShoot.RefillAmmo(ammoPerRefill));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        turretShoot.CancelReload();
    }

    public Sprite getInteractionIcon()
    {
        return interactionSprite;
    }
}
