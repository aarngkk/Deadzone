using UnityEngine;

public class AmmoCollectableBehaviour : MonoBehaviour, ICollectableBehaviour
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private int ammoAmount;
    [SerializeField] private AudioClip ammoPickupClip;
    [SerializeField] private float ammoPickupVolume = 1f;

    public void OnCollected(GameObject player)
    {
        SoundFXManager.instance.PlaySoundFXClip(ammoPickupClip, transform, ammoPickupVolume);
        player.GetComponent<PlayerShoot>().AddAmmo(ammoAmount, weaponType);
    }
}
