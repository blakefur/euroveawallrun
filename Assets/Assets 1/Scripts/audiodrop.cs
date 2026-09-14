using UnityEngine;

public class audiodrop : MonoBehaviour
{
    // Assign this in the Inspector to the object you want to monitor
    public GameObject objectToWatch;

    // Assign this in the Inspector with the sound to play
    public AudioSource dropAudioSource;

    // Tag your ground object as "Ground" in the Unity Editor
    private void OnCollisionEnter(Collision collision)
    {
        if (objectToWatch != null && collision.gameObject.CompareTag("Ground"))
        {
            if (dropAudioSource != null && !dropAudioSource.isPlaying)
            {
                dropAudioSource.Play();
            }
        }
    }
}