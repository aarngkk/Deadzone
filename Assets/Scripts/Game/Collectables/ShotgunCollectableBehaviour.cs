using UnityEngine;

public class ShotgunCollectableBehaviour : MonoBehaviour, ICollectableBehaviour
{
    public void OnCollected(GameObject player)
    {
        player.GetComponent<PlayerLoadout>().UnlockWeapon(WeaponType.Shotgun);
    }
}
