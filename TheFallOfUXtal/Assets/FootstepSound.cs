using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioSource footstepAudio;

    public AudioClip outdoorFootsteps;  // pas sur feuilles
    public AudioClip indoorFootsteps;   // pas sur bois

    private AudioClip currentClip;

    private void Start()
    {
        // On commence avec les pas extérieur par défaut
        SetFootstepClip(outdoorFootsteps);
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        bool isMoving = (moveX != 0 || moveY != 0);

        if (isMoving && !footstepAudio.isPlaying)
        {
            footstepAudio.Play();
        }
        else if (!isMoving && footstepAudio.isPlaying)
        {
            footstepAudio.Pause();
        }
    }

    public void SetFootstepClip(AudioClip newClip)
    {
        if (currentClip == newClip) return; // pas besoin de changer si c’est déjà le bon

        currentClip = newClip;
        footstepAudio.clip = currentClip;

        // Si on est en train de marcher, on relance le son avec le nouveau clip
        if (footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
            footstepAudio.Play();
        }
    }
}
