using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;  // The camera that follows the player
    [SerializeField] private Camera arenaCamera;   // The camera that stays fixed on the arena

    private void Update()
    {
        // If the fight is activated, switch to the arena camera
        if (TP_Fight.isFightTileActivated)
        {
            // Make the arena camera active, and the player camera inactive
            playerCamera.gameObject.SetActive(false);
            arenaCamera.gameObject.SetActive(true);
        }
        else
        {
            // Make the player camera active, and the arena camera inactive
            playerCamera.gameObject.SetActive(true);
            arenaCamera.gameObject.SetActive(false);
        }
    }
}
