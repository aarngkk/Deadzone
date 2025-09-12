using UnityEngine;

public class WeaponCollectableBehaviour : MonoBehaviour, ICollectableBehaviour
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private AudioClip weaponPickupClip;
    [SerializeField] private float weaponPickupVolume = 1f;

    public void OnCollected(GameObject player)
    {
        SoundFXManager.instance.PlaySoundFXClip(weaponPickupClip, transform, weaponPickupVolume);
        player.GetComponent<PlayerLoadout>().UnlockWeapon(weaponType);
    }
}
