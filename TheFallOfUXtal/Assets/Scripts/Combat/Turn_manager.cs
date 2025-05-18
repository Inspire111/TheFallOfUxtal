using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    private bool isPlayerInArena = false;  // Flag to check if player is in arena
    private bool isPlayerTurn = true;      // Flag to manage whose turn it is

    private Renderer playerRenderer;  // Reference to the player's Renderer
    private Color originalColor;     // Store the original color of the player

    private float enemyTurnDuration = 5f; // Duration of enemy's turn in seconds
    private float enemyTurnTimer = 0f;   // Timer for enemy's turn

    void Start()
    {
        // Find the player's Renderer (assuming the player has a tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerRenderer = player.GetComponent<Renderer>();
            originalColor = playerRenderer.material.color;  // Store the original color of the player
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is in the arena before processing turns
        if (!isPlayerInArena)
        {
            return;  // Skip turn management if the player is not in the arena
        }

        // Player's turn
        if (isPlayerTurn)
        {
            if (Input.GetKeyDown(KeyCode.Return))  // End player's turn
            {
                EndPlayerTurn();
            }
        }
        // Enemy's turn
        else
        {
            EnemyTurn();
        }
    }

    // Called when player enters the arena
    public void SetPlayerInArena(bool isInArena)
    {
        isPlayerInArena = isInArena;
    }

    // Ends the player's turn and starts the enemy's turn
    void EndPlayerTurn()
    {
        isPlayerTurn = false;

        // Change the player's color to red when it's the enemy's turn
        if (playerRenderer != null)
        {
            playerRenderer.material.color = Color.red;
        }

        // Start the enemy turn timer
        enemyTurnTimer = enemyTurnDuration;
        StartCoroutine(EnemyTurnTimer());
    }

    // Simulate enemy behavior (currently just logs the enemy's turn)
    void EnemyTurn()
    {
        // You can add additional logic here for the enemy's behavior (if needed later)
    }

    // Starts the player's turn again after enemies finish their actions
    void StartPlayerTurn()
    {
        // Reset the player's color to its original color when it's the player's turn again
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
        }

        isPlayerTurn = true;
    }

    // Coroutine to handle the enemy's turn timer
    private IEnumerator EnemyTurnTimer()
    {
        while (enemyTurnTimer > 0)
        {
            Debug.Log($"Enemy's turn. Time remaining: {enemyTurnTimer:F1} seconds.");
            enemyTurnTimer -= Time.deltaTime;  // Decrease the timer
            yield return null;  // Wait until the next frame
        }

        // After the timer runs out, end the enemy's turn and switch to the player
        Debug.Log("Enemy's turn ended.");
        StartPlayerTurn();
    }
}