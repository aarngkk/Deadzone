using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    private void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    }
}
