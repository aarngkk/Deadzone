using UnityEngine;

public class ShotgunCollectableBehaviour : MonoBehaviour, ICollectableBehaviour
{
    [SerializeField] private AudioClip shotgunPickupClip;
    [SerializeField] private float shotgunPickupVolume = 1f;

    public void OnCollected(GameObject player)
    {
        SoundFXManager.instance.PlaySoundFXClip(shotgunPickupClip, transform, shotgunPickupVolume);
        player.GetComponent<PlayerLoadout>().UnlockWeapon(WeaponType.Shotgun);
    }
}
