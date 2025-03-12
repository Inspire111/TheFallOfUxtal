using UnityEngine;

public class TP_Fight : MonoBehaviour
{
    private bool isPlayerInRange = false;
    public static bool isFightTileActivated = false;

    [SerializeField] private float heightOffset = 0.5f;

    private GridCombatManager gridCombatManager;

    private void Start()
    {
        gridCombatManager = FindObjectOfType<GridCombatManager>(); // Find reference in scene
    }

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
                gridCombatManager.GenerateArena(); // ✅ Generate arena when pressing E on FightTile
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
