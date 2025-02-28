using UnityEngine;

public class TP_Fight : MonoBehaviour
{
    private bool isPlayerInRange = false;
    public static bool isFightTileActivated = false;

    [SerializeField] private GameObject playerspawnTile;  // Serialized GameObject for player's spawn tile

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered the fight trigger area.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player exited the fight trigger area.");
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            isFightTileActivated = true;  // Disable WASD movement

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Teleport the player to the fixed position (-100, -100)
                player.transform.position = new Vector3(-100f, -100f, player.transform.position.z);
                Debug.Log("Player teleported to position (-100, -100).");
            }
            else
            {
                Debug.LogError("Player not assigned!");
            }
        }
    }
}

