using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    private TMP_Text ammoText;

    private void Awake()
    {
        ammoText = GetComponent<TMP_Text>();
    }

    public void UpdateAmmoUI(PlayerShoot playerShoot)
    {
        if (playerShoot.IsReloading)
        {
            ammoText.text = "Reloading...";
        }
        else
        {
            ammoText.text = $"Ammo: {playerShoot._currentAmmo}/{playerShoot._maxAmmo}";
        }
    }
}
