using UnityEngine;

public class AmmoCollectableBehaviour : MonoBehaviour, ICollectableBehaviour
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private int pistolAmmoAmount;
    [SerializeField] private int shotgunAmmoAmount;
    [SerializeField] private AudioClip ammoPickupClip;
    [SerializeField] private float ammoPickupVolume = 1f;
    private int ammoToAdd;

    public void OnCollected(GameObject player)
    {
        if (weaponType == WeaponType.Pistol)
        {
            ammoToAdd = pistolAmmoAmount;
        }
        else if (weaponType == WeaponType.Shotgun)
        {
            ammoToAdd = shotgunAmmoAmount;
        }

        SoundFXManager.instance.PlaySoundFXClip(ammoPickupClip, transform, ammoPickupVolume);
        player.GetComponent<PlayerShoot>().AddAmmo(ammoToAdd, weaponType);
    }
}
