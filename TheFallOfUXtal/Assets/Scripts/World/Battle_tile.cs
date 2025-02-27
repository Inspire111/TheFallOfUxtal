using UnityEngine;

public class TP_Fight : MonoBehaviour
{
    private bool isPlayerInRange = false;
    public static bool isFightTileActivated = false;

    [SerializeField] private Transform fightArenaPosition;

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
            if (player != null && fightArenaPosition != null)
            {
                player.transform.position = fightArenaPosition.position;
                Debug.Log("Player teleported to the fight arena.");
            }
            else
            {
                Debug.LogError("Player or fightArenaPosition not assigned!");
            }
        }
    }
}
