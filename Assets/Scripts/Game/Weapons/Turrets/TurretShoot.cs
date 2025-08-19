using System.Collections;
using UnityEngine;
using TMPro;

public class TurretShoot : MonoBehaviour
{
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Transform gunOffset;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private TMP_Text ammoText;
    private PlayerShoot playerShoot;

    [Header("Stats")]
    [SerializeField] private float fireRate;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int maxAmmo;
    [SerializeField] private float reloadDuration;
    public bool HasMaxAmmo => currentAmmo >= maxAmmo;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip turretShotClip;
    [SerializeField] private float turretShotVolume = 1f;
    [SerializeField] private AudioClip turretReloadClip;
    [SerializeField] private float turretReloadVolume = 1f;
    [SerializeField] private AudioClip turretDryFireClip;
    [SerializeField] private float turretDryFireVolume = 1f;
    [SerializeField] private int maxDryFirePlays = 5;
    private int dryFireCount = 0;
    private AudioSource turretReloadAudioSource;

    private float lastFireTime;
    private TurretAim turretAim;
    public bool isReloading { get; private set; }

    private void Start()
    {
        turretAim = GetComponent<TurretAim>();
        playerShoot = GameObject.FindWithTag("Player").GetComponent<PlayerShoot>();
        lastFireTime = -Mathf.Infinity;
        UpdateAmmoUI();
    }

    void Update()
    {
        HandleFiring();
    }

    private void HandleFiring()
    {
        float timeSinceLastFire = Time.time - lastFireTime;

        if (timeSinceLastFire > fireRate && turretAim.hasTarget && !isReloading)
        {
            if (currentAmmo > 0)
            {
                Fire();
                dryFireCount = 0;
            }
            else if (dryFireCount < maxDryFirePlays)
            {
                DryFire();
                dryFireCount++;
            }
        }
    }

    private void Fire()
    {
        SoundFXManager.instance.PlaySoundFXClip(turretShotClip, transform, turretShotVolume);

        GameObject bullet = Instantiate(bulletPrefab, gunOffset.position, gunTransform.rotation * Quaternion.Euler(0, 0, -90));

        Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();
        rigidbody.linearVelocity = bulletSpeed * gunTransform.right;

        currentAmmo--;
        UpdateAmmoUI();
        lastFireTime = Time.time;
    }

    private void DryFire()
    {
        SoundFXManager.instance.PlaySoundFXClip(turretDryFireClip, transform, turretDryFireVolume);
        lastFireTime = Time.time;
    }

    public IEnumerator RefillAmmo(int reloadAmount)
    {
        turretReloadAudioSource = SoundFXManager.instance.PlaySoundFXClip(turretReloadClip, transform, turretReloadVolume);
        isReloading = true;
        yield return new WaitForSeconds(reloadDuration);

        int requiredAmmo = maxAmmo - currentAmmo;
        int ammoToGive = Mathf.Min(requiredAmmo, reloadAmount);

        if (playerShoot) playerShoot.ReloadTurret(ammoToGive);
        currentAmmo += ammoToGive;
        UpdateAmmoUI();

        isReloading = false;
    }

    public void CancelReload()
    {
        if (turretReloadAudioSource)
        {
            turretReloadAudioSource.Stop();
            Destroy(turretReloadAudioSource);
            turretReloadAudioSource = null;
        }
    
        if (isReloading)
        {
            isReloading = false;
            Debug.Log("Reload cancelled.");
        }
    }

    private void UpdateAmmoUI()
    {
        ammoText.text = currentAmmo + "/" + maxAmmo;
    }
}
