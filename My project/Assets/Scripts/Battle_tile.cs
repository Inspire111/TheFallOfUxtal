using UnityEngine;

public class TP_Fight : MonoBehaviour
{
    [SerializeField] private Vector2[] fightTargetPositions;
    private bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered the trigger area.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player exited the trigger area.");
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed, attempting teleport...");
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        if (fightTargetPositions.Length > 0)
        {
            int randomIndex = Random.Range(0, fightTargetPositions.Length);
            Vector2 selectedTargetPosition = fightTargetPositions[randomIndex];

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Debug.Log("Teleporting player to position: " + selectedTargetPosition);
                player.transform.position = selectedTargetPosition;
            }
            else
            {
                Debug.LogError("Player GameObject not found!");
            }
        }
        else
        {
            Debug.LogError("No teleport target positions assigned!");
        }
    }
}
