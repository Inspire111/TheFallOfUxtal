using UnityEngine;

public class MusicZoneVolumeControl : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioClip zoneClip;

    private AudioSource zoneAudioSource;

    private void Start()
    {
        zoneAudioSource = GetComponent<AudioSource>();
        zoneAudioSource.playOnAwake = false;
        zoneAudioSource.loop = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (backgroundMusic != null)
            backgroundMusic.volume = 0f;

        if (zoneClip != null)
        {
            zoneAudioSource.clip = zoneClip;
            zoneAudioSource.Play();
        }
    }

}
