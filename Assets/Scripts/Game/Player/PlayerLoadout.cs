using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public enum WeaponType { Pistol, Shotgun, AssaultRifle, RocketLauncher }

public class PlayerLoadout : MonoBehaviour
{
    public UnityEvent OnAmmoUIUpdate;

    private HashSet<WeaponType> unlockedWeaponTypes = new HashSet<WeaponType>();
    public WeaponType equippedWeapon { get; private set; }
    private PlayerShoot playerShoot;
    private Animator animator;
    [SerializeField] private bool unlockShotgun = false;
    [SerializeField] private bool unlockAssaultRifle = false;
    [SerializeField] private bool unlockRocketLauncher = false;
    [SerializeField] private CollectableSpawner collectableSpawner;
    [SerializeField] private AnimatorOverrideController pistolAOC;
    [SerializeField] private AnimatorOverrideController shotgunAOC;
    [SerializeField] private AnimatorOverrideController assaultRifleAOC;
    [SerializeField] private AnimatorOverrideController rocketLauncherAOC;

    [Header("Sound Effects")]
    private AudioSource weaponDeployAudio;
    [SerializeField] private AudioClip pistolRackClip;
    [SerializeField] private float pistolRackVolume = 1f;
    [SerializeField] private AudioClip shotgunRackClip;
    [SerializeField] private float shotgunRackVolume = 1f;
    [SerializeField] private AudioClip assaultRifleClip;
    [SerializeField] private float assaultRifleVolume = 1f;
    [SerializeField] private AudioClip rocketLauncherDeployClip;
    [SerializeField] private float rocketLauncherDeployVolume = 1f;

    [Header("Shotgun Ammo Collectable")]
    [SerializeField] private GameObject shotgunAmmoPrefab;
    [SerializeField] private float shotgunAmmoDropChance;

    [Header("Assault Rifle Ammo Collectable")]
    [SerializeField] private GameObject assaultRifleAmmoPrefab;
    [SerializeField] private float assaultRifleAmmoDropChance;

    [Header("Rocket Launcher Ammo Collectable")]
    [SerializeField] private GameObject rocketAmmoPrefab;
    [SerializeField] private float rocketAmmoDropChance;

    private void Start()
    {
        playerShoot = GetComponent<PlayerShoot>();
        animator = GetComponent<Animator>();
        
        unlockedWeaponTypes.Add(WeaponType.Pistol);
        if (unlockShotgun) UnlockWeapon(WeaponType.Shotgun);
        if (unlockAssaultRifle) UnlockWeapon(WeaponType.AssaultRifle);
        if (unlockRocketLauncher) UnlockWeapon(WeaponType.RocketLauncher);
        equippedWeapon = WeaponType.Pistol;
        SoundFXManager.instance.PlaySoundFXClip(pistolRackClip, transform, pistolRackVolume);
    }

    private void Update()
    {
        SwitchWeapons();
    }

    public void UnlockWeapon(WeaponType type)
    {
        unlockedWeaponTypes.Add(type);
        
        if (type == WeaponType.Shotgun)
        {
            collectableSpawner.AddAmmoCollectableDrop(WeaponType.Shotgun, shotgunAmmoPrefab, shotgunAmmoDropChance);
        }
        if (type == WeaponType.AssaultRifle)
        {
            collectableSpawner.AddAmmoCollectableDrop(WeaponType.AssaultRifle, assaultRifleAmmoPrefab, assaultRifleAmmoDropChance);
        }
        if (type == WeaponType.RocketLauncher)
        {
            collectableSpawner.AddAmmoCollectableDrop(WeaponType.RocketLauncher, rocketAmmoPrefab, rocketAmmoDropChance);
        }
    }

    public bool HasWeapon(WeaponType type)
    {
        return unlockedWeaponTypes.Contains(type);
    }

    private void SwitchWeapons()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && equippedWeapon != WeaponType.Pistol)
        {
            playerShoot.CancelReload();
            CancelWeaponDeploy();
            animator.SetInteger("WeaponType", 0);
            equippedWeapon = WeaponType.Pistol;
            animator.runtimeAnimatorController = pistolAOC;
            OnAmmoUIUpdate.Invoke();

            weaponDeployAudio = SoundFXManager.instance.PlaySoundFXClip(pistolRackClip, transform, pistolRackVolume);

            Debug.Log("Equipped pistol.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && HasWeapon(WeaponType.Shotgun) && equippedWeapon != WeaponType.Shotgun)
        {
            playerShoot.CancelReload();
            CancelWeaponDeploy();
            animator.SetInteger("WeaponType", 1);
            equippedWeapon = WeaponType.Shotgun;
            animator.runtimeAnimatorController = shotgunAOC;
            OnAmmoUIUpdate.Invoke();

            animator.SetTrigger("IsShotgunPump");
            weaponDeployAudio = SoundFXManager.instance.PlaySoundFXClip(shotgunRackClip, transform, shotgunRackVolume);

            Debug.Log("Equipped shotgun.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && HasWeapon(WeaponType.AssaultRifle) && equippedWeapon != WeaponType.AssaultRifle)
        {
            playerShoot.CancelReload();
            CancelWeaponDeploy();
            animator.SetInteger("WeaponType", 2);
            equippedWeapon = WeaponType.AssaultRifle;
            animator.runtimeAnimatorController = assaultRifleAOC;
            OnAmmoUIUpdate.Invoke();

            weaponDeployAudio = SoundFXManager.instance.PlaySoundFXClip(assaultRifleClip, transform, assaultRifleVolume);

            Debug.Log("Equipped assault rifle.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && HasWeapon(WeaponType.RocketLauncher) && equippedWeapon != WeaponType.RocketLauncher)
        {
            playerShoot.CancelReload();
            CancelWeaponDeploy();
            animator.SetInteger("WeaponType", 3);
            equippedWeapon = WeaponType.RocketLauncher;
            animator.runtimeAnimatorController = rocketLauncherAOC;
            OnAmmoUIUpdate.Invoke();
            weaponDeployAudio = SoundFXManager.instance.PlaySoundFXClip(rocketLauncherDeployClip, transform, rocketLauncherDeployVolume);
            Debug.Log("Equipped rocket launcher.");
        }

        playerShoot.CancelFireInputBuffering();
    }

    private void CancelWeaponDeploy()
    {
        if (weaponDeployAudio != null)
        {
            weaponDeployAudio.Stop();
            Destroy(weaponDeployAudio);
            weaponDeployAudio = null;
        }
    }
}
