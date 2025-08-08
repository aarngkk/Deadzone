using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public UnityEvent OnAmmoUIUpdate;

    [SerializeField] private AmmoUI ammoUI;

    [Header("Pistol")]
    [SerializeField] private GameObject pistolBulletPrefab;
    [SerializeField] private float pistolBulletSpeed;
    [SerializeField] private Transform gunOffset;
    [SerializeField] private float pistolFireRate;
    [SerializeField] private float pistolReloadDuration;
    [SerializeField] private int pistolAmmo;
    [SerializeField] private int pistolMagSize = 15;
    [SerializeField] private int pistolCurrentAmmo = 15;
    public int _pistolAmmo => pistolAmmo;
    public int _pistolCurrentAmmo => pistolCurrentAmmo;

    [Header("Shotgun")]
    [SerializeField] private GameObject shotgunBulletPrefab;
    [SerializeField] private float shotgunBulletSpeed;
    [SerializeField] private Transform shotgunOffset;
    [SerializeField] private float shotgunFireRate;
    [SerializeField] private float shotgunReloadDuration;
    [SerializeField] private float shotgunReloadFireDelay;
    [SerializeField] private int shotgunAmmo;
    [SerializeField] private int shotgunMagSize = 6;
    [SerializeField] private int shotgunCurrentAmmo = 6;
    [SerializeField] private int shotgunPellets = 3;
    [SerializeField] private int shotgunMinAngle, shotgunMaxAngle;
    private float lastShotgunReloadTime = 0f;
    public int _shotgunAmmo => shotgunAmmo;
    public int _shotgunCurrentAmmo => shotgunCurrentAmmo;
    private bool isShotgunPumping = false;

    [Header("Sound Effects")]
    private AudioSource currentReloadAudio;
    [SerializeField] private AudioClip pistolShotClip;
    [SerializeField] private float pistolShotVolume = 1f;
    [SerializeField] private AudioClip pistolReloadAudioClip;
    [SerializeField] private float pistolReloadVolume = 1f;
    [SerializeField] private AudioClip shotgunReloadAudioClip;
    [SerializeField] private float shotgunReloadVolume = 1f;
    [SerializeField] private AudioClip shotgunShotClip;
    [SerializeField] private float shotgunShotVolume = 1f;
    [SerializeField] private AudioClip shotgunPumpClip;
    [SerializeField] private float shotgunPumpVolume = 1f;

    private Animator animator;
    private bool fireContinuously;
    private bool fireSingle;
    private float lastFireTime;
    private PlayerLoadout playerLoadout;
    public Coroutine reloadCoroutine;
    private Coroutine shotgunPumpCoroutine;

    private bool isReloading = false;
    private WeaponType equippedWeapon;
    public bool IsReloading => isReloading;

    private void Awake()
    {
        playerLoadout = GetComponent<PlayerLoadout>();
        animator = GetComponent<Animator>();
        pistolCurrentAmmo = pistolMagSize;
        OnAmmoUIUpdate?.Invoke();
    }

    void Update()
    {
        equippedWeapon = playerLoadout.equippedWeapon;

        if (equippedWeapon == WeaponType.Pistol)
        {
            if ((fireContinuously || fireSingle) && pistolCurrentAmmo > 0 && !isReloading)
            {
                float timeSinceLastFire = Time.time - lastFireTime;

                if (timeSinceLastFire > pistolFireRate)
                {
                    FirePistol();

                    lastFireTime = Time.time;
                    fireSingle = false;
                    pistolCurrentAmmo--;
                    OnAmmoUIUpdate.Invoke();
                }
            }
        }

        else if (equippedWeapon == WeaponType.Shotgun)
        {
            if ((fireContinuously || fireSingle) && shotgunCurrentAmmo > 0 && !isShotgunPumping)
            {
                float timeSinceLastFire = Time.time - lastFireTime;
                float timeSinceLastReload = Time.time - lastShotgunReloadTime;

                if (timeSinceLastFire > shotgunFireRate && timeSinceLastReload > shotgunReloadFireDelay)
                {
                    FireShotgun();

                    lastFireTime = Time.time;
                    fireSingle = false;
                    shotgunCurrentAmmo--;
                    OnAmmoUIUpdate.Invoke();
                }
            }
        }

        if (((equippedWeapon == WeaponType.Pistol && pistolCurrentAmmo == 0) || (equippedWeapon == WeaponType.Shotgun && shotgunCurrentAmmo == 0)) && !isReloading && !isShotgunPumping)
        {
            if (reloadCoroutine == null)
            {
                reloadCoroutine = StartCoroutine(Reload());
            }
        }
    }

    private void FirePistol()
    {
        SoundFXManager.instance.PlaySoundFXClip(pistolShotClip, transform, pistolShotVolume);

        GameObject bullet = Instantiate(pistolBulletPrefab, gunOffset.position, transform.rotation);

        Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();
        rigidbody.linearVelocity = pistolBulletSpeed * transform.up;
    }

    private void FireShotgun()
    {
        for (int i = 0; i < shotgunPellets; i++)
        {
            int angle = Random.Range(shotgunMinAngle, shotgunMaxAngle);

            GameObject bullet = Instantiate(shotgunBulletPrefab, shotgunOffset.position, transform.rotation * Quaternion.Euler(0, 0, angle));
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = shotgunBulletSpeed * bullet.transform.up;
        }

        SoundFXManager.instance.PlaySoundFXClip(shotgunShotClip, transform, shotgunShotVolume);
        isShotgunPumping = true;
        shotgunPumpCoroutine = StartCoroutine(ShotgunPumpCoroutine());
    }

    private IEnumerator Reload()
    {
        if (equippedWeapon == WeaponType.Pistol)
        {
            if (pistolCurrentAmmo < pistolMagSize && pistolAmmo > 0)
            {
                isReloading = true;
                animator.SetTrigger("IsReloading");

                currentReloadAudio = SoundFXManager.instance.PlaySoundFXClip(pistolReloadAudioClip, transform, pistolReloadVolume);

                OnAmmoUIUpdate.Invoke();
                yield return new WaitForSeconds(pistolReloadDuration);
                AmmoUpdate();
                isReloading = false;
                OnAmmoUIUpdate.Invoke();
            }
        }

        else if (equippedWeapon == WeaponType.Shotgun && !isShotgunPumping)
        {
            isReloading = true;

            while (shotgunCurrentAmmo < shotgunMagSize && shotgunAmmo > 0)
            {
                animator.SetTrigger("IsReloading");

                yield return new WaitForSeconds(shotgunReloadDuration / 1.3f);
                currentReloadAudio = SoundFXManager.instance.PlaySoundFXClip(shotgunReloadAudioClip, transform, shotgunReloadVolume);

                yield return new WaitForSeconds(shotgunReloadDuration / 4.333f);
                AmmoUpdate();
                lastShotgunReloadTime = Time.time;
                OnAmmoUIUpdate.Invoke();

                if (fireSingle || fireContinuously)
                {
                    break;
                }
            }

            isReloading = false;
            OnAmmoUIUpdate.Invoke();
        }

        StopCoroutine(reloadCoroutine);
        reloadCoroutine = null;
    }

    IEnumerator ShotgunPumpCoroutine()
    {
        yield return new WaitForSeconds(shotgunShotClip.length / 2);
        animator.SetTrigger("IsShotgunPump");
        SoundFXManager.instance.PlaySoundFXClip(shotgunPumpClip, transform, shotgunPumpVolume);
        yield return new WaitForSeconds(shotgunPumpClip.length);
        isShotgunPumping = false;
    }

    public void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        if (currentReloadAudio != null)
        {
            currentReloadAudio.Stop();
            Destroy(currentReloadAudio);
            currentReloadAudio = null;
        }

        if (shotgunPumpCoroutine != null)
        {
            StopCoroutine(shotgunPumpCoroutine);
            isShotgunPumping = false;
        }

        isReloading = false;

        animator.ResetTrigger("IsReloading");
        animator.SetTrigger("Idle");
    }

    private void OnFire(InputValue inputValue)
    {
        fireContinuously = inputValue.isPressed;

        if (inputValue.isPressed)
        {
            fireSingle = true;

            if (isReloading && equippedWeapon == WeaponType.Shotgun && shotgunCurrentAmmo != 0)
            {
                CancelReload();
            }
        }
    }

    private void OnReload(InputValue inputValue)
    {
        if (!isReloading)
        {
            if (reloadCoroutine == null)
            {
                reloadCoroutine = StartCoroutine(Reload());
            }
        }
    }

    private void AmmoUpdate()
    {
        if (equippedWeapon == WeaponType.Pistol)
        {
            int bulletsToLoad = pistolMagSize - pistolCurrentAmmo;

            if (pistolAmmo >= bulletsToLoad)
            {
                pistolCurrentAmmo += bulletsToLoad;
                pistolAmmo -= bulletsToLoad;
            }

            else
            {
                pistolCurrentAmmo += pistolAmmo;
                pistolAmmo = 0;
            }
        }

        else if (equippedWeapon == WeaponType.Shotgun)
        {
            shotgunCurrentAmmo++;
            shotgunAmmo--;
        }
    }

    public void AddAmmo(int amountToAdd, WeaponType weaponType)
    {
        if (weaponType == WeaponType.Pistol)
        {
            pistolAmmo += amountToAdd;
            Debug.Log("Picked up pistol ammo.");
            OnAmmoUIUpdate.Invoke();
        }

        if (weaponType == WeaponType.Shotgun)
        {
            shotgunAmmo += amountToAdd;
            Debug.Log("Picked up shotgun ammo.");
            OnAmmoUIUpdate.Invoke();
        }
    }

    public void CancelFireInputBuffering()
    {
        fireSingle = false;
    }
}
