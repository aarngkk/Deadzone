using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text ammoExtrusionText;
    [SerializeField] private PlayerShoot playerShoot;
    [SerializeField] private PlayerLoadout playerLoadout;

    public void UpdateAmmoUI()
    {
        if (playerShoot.IsReloading && playerLoadout.equippedWeapon != WeaponType.Shotgun)
        {
            ammoText.text = "Reloading...";
            ammoExtrusionText.text = "Reloading...";
        }
        else
        {
            switch (playerLoadout.equippedWeapon)
            {
                case WeaponType.Pistol:
                    ammoText.text = $"Ammo: {playerShoot._pistolCurrentAmmo}/{playerShoot._pistolAmmo}";
                    ammoExtrusionText.text = $"Ammo: {playerShoot._pistolCurrentAmmo}/{playerShoot._pistolAmmo}";
                    break;
                case WeaponType.Shotgun:
                    ammoText.text = $"Ammo: {playerShoot._shotgunCurrentAmmo}/{playerShoot._shotgunAmmo}";
                    ammoExtrusionText.text = $"Ammo: {playerShoot._shotgunCurrentAmmo}/{playerShoot._shotgunAmmo}";
                    break;
                case WeaponType.RocketLauncher:
                    ammoText.text = $"Ammo: {playerShoot._rocketLauncherCurrentAmmo}/{playerShoot._rocketLauncherAmmo}";
                    ammoExtrusionText.text = $"Ammo: {playerShoot._rocketLauncherCurrentAmmo}/{playerShoot._rocketLauncherAmmo}";
                    break;
                default:
                    ammoText.text = "No Weapon";
                    ammoExtrusionText.text = "No Weapon";
                    break;
            }
        }
    }
}
