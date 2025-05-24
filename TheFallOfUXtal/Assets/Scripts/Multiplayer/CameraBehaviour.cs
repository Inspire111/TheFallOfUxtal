using Unity.Netcode;
using UnityEngine;

public class CameraBehaviour : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Camera playerCam;

    void Start()
    {
        playerCam = GetComponentInChildren<Camera>();

        if (!IsOwner)
        {
            // Disable or destroy camera for all non-local players
            if (playerCam != null)
            {
                playerCam.enabled = false;
            }
        }
        else
        {
            // Ensure the local player's camera is active
            if (playerCam != null)
            {
                playerCam.enabled = true;

            }
        }
    }
}
