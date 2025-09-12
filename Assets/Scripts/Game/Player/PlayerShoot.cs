using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public UnityEvent OnAmmoUIUpdate;

    [SerializeField] private AmmoUI ammoUI;
    [SerializeField] private float firingOverrideDistance = 2f;

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
    [SerializeField] private float shotgunMinAngle, shotgunMaxAngle;
    private float lastShotgunReloadTime = 0f;
    public int _shotgunAmmo => shotgunAmmo;
    public int _shotgunCurrentAmmo => shotgunCurrentAmmo;
    private bool isShotgunPumping = false;

    [Header("Assault Rifle")]
    [SerializeField] private GameObject assaultRifleBulletPrefab;
    [SerializeField] private float assaultRifleBulletSpeed;
    [SerializeField] private Transform assaultRifleOffset;
    [SerializeField] private float assaultRifleFireRate;
    [SerializeField] private float assaultRifleReloadDuration;
    [SerializeField] private int assaultRifleAmmo;
    [SerializeField] private int assaultRifleMagSize = 30;
    [SerializeField] private int assaultRifleCurrentAmmo = 30;
    [SerializeField] private float assaultRifleMaxSpread;
    [SerializeField] private float assaultRifleSpreadStep;
    public float assaultRifleSpread = 0;
    public int _assaultRifleAmmo => assaultRifleAmmo;
    public int _assaultRifleCurrentAmmo => assaultRifleCurrentAmmo;



    [Header("Rocket Launcher")]
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private float rocketSpeed;
    [SerializeField] private Transform rocketLauncherOffset;
    [SerializeField] private float rocketLauncherFireRate;
    [SerializeField] private float rocketLauncherReloadDuration;
    [SerializeField] private int rocketLauncherAmmo;
    [SerializeField] private int rocketLauncherMagSize = 1;
    [SerializeField] private int rocketLauncherCurrentAmmo = 1;
    public int _rocketLauncherAmmo => rocketLauncherAmmo;
    public int _rocketLauncherCurrentAmmo => rocketLauncherCurrentAmmo;


    [Header("Sound Effects")]    
    [SerializeField] private AudioClip pistolShotClip;
    [SerializeField] private float pistolShotVolume = 1f;
    [SerializeField] private float pistolShotAudibleDistance;
    [SerializeField] private AudioClip pistolReloadAudioClip;
    [SerializeField] private float pistolReloadVolume = 1f;
    [SerializeField] private AudioClip shotgunReloadAudioClip;
    [SerializeField] private float shotgunReloadVolume = 1f;
    [SerializeField] private AudioClip shotgunShotClip;
    [SerializeField] private float shotgunShotVolume = 1f;
    [SerializeField] private float shotgunShotAudibleDistance;
    [SerializeField] private AudioClip shotgunPumpClip;
    [SerializeField] private float shotgunPumpVolume = 1f;
    [SerializeField] private AudioClip assaultRifleShotClip;
    [SerializeField] private float assaultRifleShotVolume = 1f;
    [SerializeField] private float assaultRifleShotAudibleDistance;
    [SerializeField] private AudioClip assaultRifleReloadClip;
    [SerializeField] private float assaultRifleReloadVolume = 1f;
    [SerializeField] private AudioClip rocketLauncherShotClip;
    [SerializeField] private float rocketLauncherShotVolume = 1f;
    [SerializeField] private AudioClip rocketLauncherReloadAudioClip;
    [SerializeField] private float rocketLauncherReloadVolume = 1f;

    private AudioSource currentReloadAudio;
    private Animator animator;
    private bool fireContinuously;
    private bool fireSingle;
    private float lastFireTime;
    private PlayerLoadout playerLoadout;
    public Coroutine reloadCoroutine;
    private Coroutine shotgunPumpCoroutine;
    private HealthController healthController;
    private bool reducingSpread;

    private bool isReloading = false;
    private WeaponType equippedWeapon;
    public bool IsReloading => isReloading;

    private void Awake()
    {
        playerLoadout = GetComponent<PlayerLoadout>();
        animator = GetComponent<Animator>();
        healthController = GetComponent<HealthController>();
        lastFireTime = -Mathf.Infinity;
        pistolCurrentAmmo = pistolMagSize;
        OnAmmoUIUpdate?.Invoke();
    }

    void Update()
    {
        HandleFiring();
        HandleAutoReload();
    }

    private void HandleFiring()
    {
        if (healthController.isStunned) return;

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

        else if (equippedWeapon == WeaponType.AssaultRifle)
        {
            if ((fireContinuously || fireSingle) && assaultRifleCurrentAmmo > 0 && !isReloading)
            {
                float timeSinceLastFire = Time.time - lastFireTime;

                if (timeSinceLastFire > assaultRifleFireRate)
                {
                    FireAssaultRifle();

                    lastFireTime = Time.time;
                    fireSingle = false;
                    assaultRifleCurrentAmmo--;
                    OnAmmoUIUpdate.Invoke();
                }
            }
        }

        else if (equippedWeapon == WeaponType.RocketLauncher)
        {
            if ((fireContinuously || fireSingle) && rocketLauncherCurrentAmmo > 0 && !isReloading)
            {
                float timeSinceLastFire = Time.time - lastFireTime;

                if (timeSinceLastFire > rocketLauncherFireRate)
                {
                    FireRocket();

                    lastFireTime = Time.time;
                    fireSingle = false;
                    rocketLauncherCurrentAmmo--;
                    OnAmmoUIUpdate?.Invoke();
                }
            }
        }
    }

    private void HandleAutoReload()
    {
        if (((equippedWeapon == WeaponType.Pistol && pistolCurrentAmmo == 0) || (equippedWeapon == WeaponType.Shotgun && shotgunCurrentAmmo == 0) || (equippedWeapon == WeaponType.AssaultRifle && assaultRifleCurrentAmmo == 0) || (equippedWeapon == WeaponType.RocketLauncher && rocketLauncherCurrentAmmo == 0) && !isReloading && !isShotgunPumping))
        {
            if (reloadCoroutine == null)
            {
                reloadCoroutine = StartCoroutine(Reload());
            }
        }
    }

    private (Vector2 direction, Quaternion rotation) AimAtMouse(Transform firePoint, float spriteRotationOffset = -90f)
    {
        Vector2 spawnPos = firePoint.position;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorldPos - spawnPos).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle + spriteRotationOffset);

        return (direction, rotation);
    }

    private bool IsAimingClose()
    {
        return Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position) <= firingOverrideDistance;
    }

    private void FireStraightForward(GameObject bulletPrefab, Transform gunOffset, float bulletSpeed, Quaternion rotation, Vector2 direction)
    {
            GameObject bullet = Instantiate(bulletPrefab, gunOffset.position, rotation);

            Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();
            rigidbody.linearVelocity = bulletSpeed * direction;
    }

    private void FirePistol()
    {
        SoundFXManager.instance.PlaySoundFXClip(pistolShotClip, transform, pistolShotVolume, pistolShotAudibleDistance);            

        if (IsAimingClose())
        {
            FireStraightForward(pistolBulletPrefab, gunOffset, pistolBulletSpeed, transform.rotation, transform.up);
        }        
        else
        {
            var (direction, rotation) = AimAtMouse(gunOffset);

            GameObject bullet = Instantiate(pistolBulletPrefab, gunOffset.position, rotation);

            Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();
            rigidbody.linearVelocity = direction * pistolBulletSpeed;
        }
    }

    private void FireShotgun()
    {
        SoundFXManager.instance.PlaySoundFXClip(shotgunShotClip, transform, shotgunShotVolume, shotgunShotAudibleDistance);

        if (IsAimingClose())
        {
            for (int i = 0; i < shotgunPellets; i++)
            {
                float spread = Random.Range(shotgunMinAngle, shotgunMaxAngle);

                Quaternion pelletRotation = transform.rotation * Quaternion.Euler(0, 0, spread);
                Vector2 pelletDirection = pelletRotation * Vector2.up;

                FireStraightForward(shotgunBulletPrefab, shotgunOffset, shotgunBulletSpeed, pelletRotation, pelletDirection);
            }
        }
        else
        {
            var (baseDirection, baseRotation) = AimAtMouse(shotgunOffset);

            for (int i = 0; i < shotgunPellets; i++)
            {
                float spread = Random.Range(shotgunMinAngle, shotgunMaxAngle);

                Quaternion pelletRotation = baseRotation * Quaternion.Euler(0, 0, spread);
                Vector2 pelletDirection = pelletRotation * Vector2.up;

                GameObject bullet = Instantiate(shotgunBulletPrefab, shotgunOffset.position, pelletRotation);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.linearVelocity = shotgunBulletSpeed * pelletDirection;
            }
        }



        isShotgunPumping = true;
        shotgunPumpCoroutine = StartCoroutine(ShotgunPumpCoroutine());
    }

    private void FireAssaultRifle()
    {
        SoundFXManager.instance.PlaySoundFXClip(assaultRifleShotClip, transform, assaultRifleShotVolume, assaultRifleShotAudibleDistance);

        if (IsAimingClose())
        {
            float spread = Random.Range(assaultRifleSpread, -assaultRifleSpread);

            Quaternion bulletRotation = transform.rotation * Quaternion.Euler(0, 0, spread);
            Vector2 bulletDirection = bulletRotation * Vector2.up;

            FireStraightForward(assaultRifleBulletPrefab, assaultRifleOffset, assaultRifleBulletSpeed, bulletRotation, bulletDirection);
        }
        else
        {
            var (baseDirection, baseRotation) = AimAtMouse(assaultRifleOffset);

            float spread = Random.Range(assaultRifleSpread, -assaultRifleSpread);

            Quaternion bulletRotation = baseRotation * Quaternion.Euler(0, 0, spread);
            Vector2 bulletDirection = bulletRotation * Vector2.up;

            GameObject bullet = Instantiate(assaultRifleBulletPrefab, assaultRifleOffset.position, bulletRotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = assaultRifleBulletSpeed * bulletDirection;
        }

        if (assaultRifleSpread < assaultRifleMaxSpread) assaultRifleSpread += assaultRifleSpreadStep;
        if (!reducingSpread) StartCoroutine(ReduceSpread());
    }

    private IEnumerator ReduceSpread()
    {
        reducingSpread = true;

        while (assaultRifleSpread > 0)
        {
            yield return new WaitForSeconds(assaultRifleFireRate * 3);
            assaultRifleSpread -= assaultRifleSpreadStep;
        }

        reducingSpread = false;
    }

    private void FireRocket()
    {
        SoundFXManager.instance.PlaySoundFXClip(rocketLauncherShotClip, transform, rocketLauncherShotVolume);

        if (IsAimingClose())
        {
            FireStraightForward(rocketPrefab, rocketLauncherOffset, rocketSpeed, transform.rotation, transform.up);
        }
        else
        {
            var (direction, rotation) = AimAtMouse(rocketLauncherOffset);

            GameObject rocket = Instantiate(rocketPrefab, rocketLauncherOffset.position, rotation);

            Rigidbody2D rigidbody = rocket.GetComponent<Rigidbody2D>();
            rigidbody.linearVelocity = rocketSpeed * direction;
        }
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

        else if (equippedWeapon == WeaponType.AssaultRifle)
        {
            if (assaultRifleCurrentAmmo < assaultRifleMagSize && assaultRifleAmmo > 0)
            {
                isReloading = true;
                animator.SetTrigger("IsReloading");

                currentReloadAudio = SoundFXManager.instance.PlaySoundFXClip(assaultRifleReloadClip, transform, assaultRifleReloadVolume);

                OnAmmoUIUpdate.Invoke();
                yield return new WaitForSeconds(assaultRifleReloadDuration);
                AmmoUpdate();
                isReloading = false;
                OnAmmoUIUpdate.Invoke();
            }
        }

        else if (equippedWeapon == WeaponType.RocketLauncher)
        {
            isReloading = true;

            while (rocketLauncherCurrentAmmo < rocketLauncherMagSize && rocketLauncherAmmo > 0)
            {
                animator.SetTrigger("IsReloading");

                currentReloadAudio = SoundFXManager.instance.PlaySoundFXClip(rocketLauncherReloadAudioClip, transform, rocketLauncherReloadVolume);

                OnAmmoUIUpdate.Invoke();
                yield return new WaitForSeconds(rocketLauncherReloadDuration);
                AmmoUpdate();
                OnAmmoUIUpdate.Invoke();

                if (fireSingle || fireContinuously) break;
            }

            isReloading = false;
            animator.SetTrigger("Idle");
            OnAmmoUIUpdate.Invoke();
        }

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
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

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            fireContinuously = true;
            fireSingle = true;

            if (isReloading && equippedWeapon == WeaponType.Shotgun && shotgunCurrentAmmo != 0)
            {
                CancelReload();
            }
        }

        else if (context.canceled)
        {
            fireContinuously = false;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed && !isReloading)
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

        else if (equippedWeapon == WeaponType.AssaultRifle)
        {
            int bulletsToLoad = assaultRifleMagSize - assaultRifleCurrentAmmo;

            if (assaultRifleAmmo >= bulletsToLoad)
            {
                assaultRifleCurrentAmmo += bulletsToLoad;
                assaultRifleAmmo -= bulletsToLoad;
            }

            else
            {
                assaultRifleCurrentAmmo += assaultRifleAmmo;
                assaultRifleAmmo = 0;
            }
        }

        else if (equippedWeapon == WeaponType.RocketLauncher)
        {
            rocketLauncherCurrentAmmo++;
            rocketLauncherAmmo--;
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

        if (weaponType == WeaponType.AssaultRifle)
        {
            assaultRifleAmmo += amountToAdd;
            Debug.Log("Picked up rifle ammo.");
            OnAmmoUIUpdate.Invoke();
        }

        if (weaponType == WeaponType.RocketLauncher)
        {
            rocketLauncherAmmo += amountToAdd;
            OnAmmoUIUpdate.Invoke();
        }
    }

    public void CancelFireInputBuffering()
    {
        fireSingle = false;
    }

    public int ReloadTurret(int reloadAmount)
    {
        if (pistolAmmo >= reloadAmount)
        {
            pistolAmmo -= reloadAmount;
            OnAmmoUIUpdate.Invoke();
            return reloadAmount;
        }
        else
        {
            int ammoGiven = pistolAmmo;
            pistolAmmo = 0;
            OnAmmoUIUpdate.Invoke();
            return ammoGiven;
        }
    }
}
