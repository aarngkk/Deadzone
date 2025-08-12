using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public enum WeaponType { Pistol, Shotgun }

public class PlayerLoadout : MonoBehaviour
{
    public UnityEvent OnAmmoUIUpdate;

    private HashSet<WeaponType> unlockedWeaponTypes = new HashSet<WeaponType>();
    public WeaponType equippedWeapon { get; private set; }
    private PlayerShoot playerShoot;
    private SpriteRenderer playerSpriteRenderer;
    private Animator animator;
    [SerializeField] private bool unlockShotgun = false;
    [SerializeField] private CollectableSpawner collectableSpawner;
    [SerializeField] private AnimatorOverrideController pistolAOC;
    [SerializeField] private AnimatorOverrideController shotgunAOC;

    [Header("Sound Effects")]
    private AudioSource weaponDeployAudio;
    [SerializeField] private AudioClip pistolRackClip;
    [SerializeField] private float pistolRackVolume = 1f;
    [SerializeField] private AudioClip shotgunRackClip;
    [SerializeField] private float shotgunRackVolume = 1f;

    [Header("Shotgun Ammo Collectable")]
    [SerializeField] private GameObject shotgunAmmoPrefab;
    [SerializeField] private float shotgunAmmoDropChance;

    private void Start()
    {
        playerShoot = GetComponent<PlayerShoot>();
        playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        unlockedWeaponTypes.Add(WeaponType.Pistol);
        if (unlockShotgun) unlockedWeaponTypes.Add(WeaponType.Shotgun);
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
