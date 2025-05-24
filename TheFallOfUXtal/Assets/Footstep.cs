using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody2D))]
public class Footstep : MonoBehaviour
{
    public AudioClip footstepSound;
    public float stepRate = 0.4f; // intervalle entre les sons
    private float stepTimer;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        stepTimer = stepRate;
    }

    void Update()
    {
        float speed = rb.velocity.magnitude;

        if (speed > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                audioSource.PlayOneShot(footstepSound);
                stepTimer = stepRate;
            }
        }
        else
        {
            stepTimer = stepRate; // reset si le joueur s’arrête
        }
    }
}
