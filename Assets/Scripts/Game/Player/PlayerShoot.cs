using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunOffset;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float reloadDuration;

    private bool fireContinuously;
    private bool fireSingle;
    private float lastFireTime;
    private bool isReloading = false;

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
            }
        }

        else if (currentAmmo == 0 && !isReloading)
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
            yield return new WaitForSeconds(reloadDuration);
            currentAmmo = maxAmmo;
            isReloading = false;
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
}
