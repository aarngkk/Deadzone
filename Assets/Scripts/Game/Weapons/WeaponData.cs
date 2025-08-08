using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public int maxAmmo;
    public float timeBetweenShots;
    public float reloadDuration;
    public AudioClip fireSound;
    public float fireVolume = 1f;
    public Vector2 bulletOffset;
}
