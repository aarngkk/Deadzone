using UnityEngine;

public class StationSecurityButtonInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite interactionSprite;
    [SerializeField] private GameObject[] stationDoors;
    [SerializeField] private AudioClip sirenAlarmClip;
    [SerializeField] private float sirenAlarmVolume = 1f;
    [SerializeField] private float sirenAudibleDistance = 24f;
    [SerializeField] private Transform policeStationEntrance;
    public bool isPressed;

    public bool canInteract()
    {
        return !isPressed;
    }

    public Sprite getInteractionIcon()
    {
        return interactionSprite;
    }

    public void Interact()
    {
        if (!canInteract()) return;
        OpenStationDoors();
        PlaySirenAlarm();
    }

    private void OpenStationDoors()
    {
        foreach (GameObject stationDoor in stationDoors)
        {
            DoorInteraction[] doorInteractions = stationDoor.GetComponentsInChildren<DoorInteraction>();
            foreach (DoorInteraction door in doorInteractions)
            {
                door.UnlockDoor();
                door.OpenDoor();
            }
        }
    }

    private void PlaySirenAlarm()
    {
        SoundFXManager.instance.PlaySoundFXClip(sirenAlarmClip, policeStationEntrance, sirenAlarmVolume, sirenAudibleDistance);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(policeStationEntrance.position, sirenAudibleDistance);
    }
}
