using UnityEngine;

public class HealthCollectableBehaviour : MonoBehaviour, ICollectableBehaviour
{
    [SerializeField] private float healthAmount;
    [SerializeField] private AudioClip healthPickupClip;
    [SerializeField] private float healthPickupVolume = 1f;

    public void OnCollected(GameObject player)
    {
        SoundFXManager.instance.PlaySoundFXClip(healthPickupClip, transform, healthPickupVolume);
        player.GetComponent<HealthController>().AddHealth(healthAmount);
    }
}
