using UnityEngine;

public class AmmoCollectableBehaviour : MonoBehaviour, ICollectableBehaviour
{
    [SerializeField] private int pistolAmmoAmount;
    [SerializeField] private int shotgunAmmoAmount;
    private WeaponType weaponType;
    private int ammoToAdd;

    public void OnCollected(GameObject player)
    {
        weaponType = (WeaponType)Random.Range(0, 2);

        if (weaponType == WeaponType.Pistol)
        {
            ammoToAdd = pistolAmmoAmount;
        }
        else if (weaponType == WeaponType.Shotgun)
        {
            ammoToAdd = shotgunAmmoAmount;
        }
            player.GetComponent<PlayerShoot>().AddAmmo(ammoToAdd, weaponType);
    }
}
