using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public UnityEvent OnAmmoUIUpdate;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunOffset;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float reloadDuration;
    [SerializeField] private AmmoUI ammoUI;
    [SerializeField] private int maxAmmo = 15;
    [SerializeField] private AudioClip reloadAudioClip;
    [SerializeField] private float reloadVolume = 1f;
    public int _maxAmmo => maxAmmo;
    [SerializeField] private int currentAmmo = 15;
    public int _currentAmmo => currentAmmo;

    private Animator animator;
    private bool fireContinuously;
    private bool fireSingle;
    private float lastFireTime;
    private bool isReloading = false;
    public bool IsReloading => isReloading;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if ((fireContinuously || fireSingle) && currentAmmo > 0 && !isReloading)
        {
            float timeSinceLastFire = Time.time - lastFireTime;

            if (timeSinceLastFire > timeBetweenShots)
            {
                FireBullet();
                
                lastFireTime = Time.time;
                fireSingle = false;
                currentAmmo--;
                OnAmmoUIUpdate.Invoke();
            }
        }

        if (currentAmmo == 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    private void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, gunOffset.position, transform.rotation);
        Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();

        rigidbody.linearVelocity = bulletSpeed * transform.up;
    }

    private IEnumerator Reload()
    {
        if (currentAmmo < maxAmmo)
        {
            isReloading = true;
            animator.SetTrigger("IsReloading");

            SoundFXManager.instance.PlaySoundFXClip(reloadAudioClip, transform, reloadVolume);

            OnAmmoUIUpdate.Invoke();
            yield return new WaitForSeconds(reloadDuration);
            currentAmmo = maxAmmo;
            isReloading = false;
            OnAmmoUIUpdate.Invoke();
        }
    }

    private void OnFire(InputValue inputValue)
    {
        fireContinuously = inputValue.isPressed;

        if (inputValue.isPressed)
        {
            fireSingle = true;
        }
    }

    private void OnReload(InputValue inputValue)
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }
}
