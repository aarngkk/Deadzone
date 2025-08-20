using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    public enum DoorSide
    {
        Left,
        Right
    }

    [SerializeField] private DoorSide doorSide;
    [SerializeField] private bool isOpen;    
    [SerializeField] private bool isLocked;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float lockedDoorShakeIntensity;
    [SerializeField] private int lockedDoorShakes;
    [SerializeField] private Sprite interactionSprite;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip[] doorOpeningAudioClips;
    [SerializeField] private float doorOpeningVolume = 1f;
    [SerializeField] private AudioClip[] doorClosingAudioClips;
    [SerializeField] private float doorClosingVolume = 1f;
    [SerializeField] private AudioClip[] doorLockedAudioClips;
    [SerializeField] private float doorLockedVolume = 1f;
    private AudioSource doorAudioSource;

    private Coroutine doorCoroutine;
    private Transform playerTransform;

    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        if (isOpen && !isLocked)
        {
            if (doorSide == DoorSide.Left) transform.rotation = Quaternion.Euler(0f, 0f, 120f);
            else if (doorSide == DoorSide.Right) transform.rotation = Quaternion.Euler(0f, 0f, -120f);
        }
    }

    public bool canInteract()
    {
        return true;
    }

    public Sprite getInteractionIcon()
    {
        return interactionSprite;
    }

    public void Interact()
    {
        if (!canInteract()) return;
        ToggleDoor();
    }

    private void ToggleDoor()
    {
        if (doorAudioSource != null)
        {
            doorAudioSource.Stop();
            doorAudioSource = null;
        }

        if (isLocked)
        {
            StartCoroutine(ShakeLockedDoor());
            doorAudioSource = SoundFXManager.instance.PlayNonRepeatingRandomClip(doorLockedAudioClips, transform, doorLockedVolume, gameObject.name + "_Locked");
            return;
        }

        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }

        float targetAngle = 0f;
        if (!isOpen)
        {
            Vector3 playerPosition = playerTransform.position;
            bool playerAboveDoor = playerPosition.y > transform.position.y;

            if (playerAboveDoor)
            {
                targetAngle = (doorSide == DoorSide.Left) ? -120f : 120f;
            }
            else if (!playerAboveDoor)
            {
                targetAngle = (doorSide == DoorSide.Left) ? 120f : -120f;
            }                
        }

        if (targetAngle == 0f)
        {            
            doorAudioSource = SoundFXManager.instance.PlayNonRepeatingRandomClip(doorClosingAudioClips, transform, doorClosingVolume, gameObject.name + "_Closing");
        }
        else
        {
            doorAudioSource = SoundFXManager.instance.PlayNonRepeatingRandomClip(doorOpeningAudioClips, transform, doorOpeningVolume, gameObject.name + "_Opening");
        }

        doorCoroutine = StartCoroutine(RotateDoor(targetAngle));
        isOpen = !isOpen;
    }

    private IEnumerator RotateDoor(float targetAngle)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0f, 0f, targetAngle);
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed /  animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
    }

    private IEnumerator ShakeLockedDoor()
    {
        for (int i = 0; i < lockedDoorShakes; i++)
        {
            transform.position += new Vector3(0f, lockedDoorShakeIntensity, 0f);
            yield return new WaitForSeconds(0.1f);
            transform.position += new Vector3(0f, -lockedDoorShakeIntensity * 2, 0f);
            yield return new WaitForSeconds(0.1f);
            transform.position += new Vector3(0f, lockedDoorShakeIntensity * 2, 0f);
            yield return new WaitForSeconds(0.1f);
            transform.position += new Vector3(0f, -lockedDoorShakeIntensity, 0f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
