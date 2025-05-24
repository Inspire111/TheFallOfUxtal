using UnityEngine;

public class FootstepZone : MonoBehaviour
{
    public enum FootstepType { Outdoor, Indoor }

    public FootstepType zoneType = FootstepType.Indoor;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FootstepSound footstepSound = collision.GetComponent<FootstepSound>();
            if (footstepSound != null)
            {
                if (zoneType == FootstepType.Indoor)
                    footstepSound.SetFootstepClip(footstepSound.indoorFootsteps);
                else
                    footstepSound.SetFootstepClip(footstepSound.outdoorFootsteps);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FootstepSound footstepSound = collision.GetComponent<FootstepSound>();
            if (footstepSound != null)
            {
                // Remet le son par défaut à la sortie de la zone
                footstepSound.SetFootstepClip(footstepSound.outdoorFootsteps);
            }
        }
    }
}
