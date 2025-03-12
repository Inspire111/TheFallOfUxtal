using UnityEngine;


public class TP_Fight : MonoBehaviour
{
    private bool isPlayerInRange = false;
    public static bool isFightTileActivated = false;

    // Offset value to place player slightly above the tile
    [SerializeField] private float heightOffset = 0.5f;

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
            if (!GridCombatManager.arenaReady)
            {
                Debug.LogWarning("Arena is not ready yet! Please generate the arena first.");
                return;
            }

            isFightTileActivated = true;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 spawnPositionWithOffset = GridCombatManager.playerSpawnPosition + new Vector3(0, heightOffset, 0);
                player.transform.position = spawnPositionWithOffset;
                Debug.Log($"Player teleported to spawn position: {spawnPositionWithOffset}");
            }
            else
            {
                Debug.LogError("Player not found in scene!");
            }
        }
    }
}
