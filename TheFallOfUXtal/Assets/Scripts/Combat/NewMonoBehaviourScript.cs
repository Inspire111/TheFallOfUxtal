using UnityEngine;
using System.Collections.Generic;

public class GridCombatManager : MonoBehaviour
{
    public int gridSize = 10;
    public float tileWidth = 2f;
    public float tileHeight = 1f;
    public GameObject walkableTilePrefab;
    public GameObject playerSpawnTilePrefab;
    public GameObject enemySpawnTilePrefab;
    public GameObject obstaclePrefab;
    public int numberOfEnemies = 3;
    public int numberOfObstacles = 5;
    public Transform gridParent;

    public static Vector3 playerSpawnPosition = Vector3.zero; // Static player spawn position
    public static bool arenaReady = false;  // Static flag to check if arena is generated

    private List<Vector3> walkablePositions = new List<Vector3>();  // All walkable tiles
    private List<Vector3> enemyPositions = new List<Vector3>();    // Enemies' positions

    private bool arenaGenerated = false;

    // Offset value for enemy height (same as the player)
    [SerializeField] private float heightOffset = 0.5f;

    // Reference to the Enemy GameObject (to spawn)
    public GameObject enemyPrefab;

    public TurnManager turnManager;  // Reference to the TurnManager to notify when the player is in the arena

    void Update()
    {
        // Press E to generate arena
        if (!arenaGenerated && Input.GetKeyDown(KeyCode.E))
        {
            GenerateDiamondArena();
            arenaGenerated = true;
            arenaReady = true;  // Mark arena as ready
            Debug.Log("Arena generated and ready!");

            // Notify the TurnManager that the player is now in the arena
            if (turnManager != null)
            {
                turnManager.SetPlayerInArena(true);
            }
        }
    }

    void GenerateDiamondArena()
    {
        Vector3 gridOrigin = gridParent.position;
        int totalRows = gridSize * 2 - 1;

        List<Vector3> availablePositions = new List<Vector3>();
        Vector3 topCenterPosition = Vector3.zero;

        for (int y = 0; y < totalRows; y++)
        {
            int rowWidth = (y < gridSize) ? y + 1 : totalRows - y;

            for (int x = 0; x < rowWidth; x++)
            {
                float isoX = (x - rowWidth / 2f) * tileWidth;
                float isoY = -y * 0.5f * tileHeight;
                Vector3 worldPosition = new Vector3(gridOrigin.x + isoX, gridOrigin.y + isoY, 0);

                Instantiate(walkableTilePrefab, worldPosition, Quaternion.identity, gridParent);
                availablePositions.Add(worldPosition);

                if (y == 0 && x == rowWidth / 2)
                {
                    topCenterPosition = worldPosition;
                }
            }
        }

        walkablePositions.AddRange(availablePositions);

        // Set and spawn player at top-center
        playerSpawnPosition = topCenterPosition;
        Instantiate(playerSpawnTilePrefab, playerSpawnPosition, Quaternion.identity, gridParent);
        availablePositions.Remove(playerSpawnPosition);

        // Place enemies in enemy spawn positions (where enemySpawnTilePrefab is placed)
        PlaceEnemies(availablePositions);

        // Place obstacles
        PlaceObstacles(availablePositions);
    }

    void PlaceEnemies(List<Vector3> availablePositions)
    {
        for (int i = 0; i < numberOfEnemies && availablePositions.Count > 0; i++)
        {
            int randomIndex = Random.Range(availablePositions.Count / 2, availablePositions.Count);
            Vector3 enemyPos = availablePositions[randomIndex];

            // Spawn the enemy spawn tile
            Instantiate(enemySpawnTilePrefab, enemyPos, Quaternion.identity, gridParent);

            // Directly spawn enemies at each enemy spawn tile with offset
            SpawnEnemyAtTile(enemyPos);

            // Store enemy position for reference (if needed for other logic)
            enemyPositions.Add(enemyPos);

            availablePositions.RemoveAt(randomIndex);
        }
    }

    // Method to spawn an enemy at a given tile position with offset
    void SpawnEnemyAtTile(Vector3 position)
    {
        // Apply the same height offset used for the player spawn
        Vector3 spawnPositionWithOffset = position + new Vector3(0, heightOffset, 0);

        // Check if the enemyPrefab is assigned
        if (enemyPrefab != null)
        {
            // Instantiate the enemy at the spawn position with offset
            Instantiate(enemyPrefab, spawnPositionWithOffset, Quaternion.identity, gridParent);
            Debug.Log($"Enemy spawned at: {spawnPositionWithOffset}");
        }
        else
        {
            Debug.LogError("Enemy prefab not assigned!");
        }
    }

    void PlaceObstacles(List<Vector3> availablePositions)
    {
        List<Vector3> placedObstacles = new List<Vector3>();

        for (int i = 0; i < numberOfObstacles && availablePositions.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector3 obstaclePos = availablePositions[randomIndex];

            placedObstacles.Add(obstaclePos);

            if (IsArenaConnected(playerSpawnPosition, enemyPositions, placedObstacles))
            {
                Instantiate(obstaclePrefab, obstaclePos, Quaternion.identity, gridParent);
                availablePositions.RemoveAt(randomIndex);
                walkablePositions.Remove(obstaclePos);
            }
            else
            {
                placedObstacles.Remove(obstaclePos);
            }
        }
    }

    bool IsArenaConnected(Vector3 start, List<Vector3> targets, List<Vector3> obstacles)
    {
        HashSet<Vector3> visited = new HashSet<Vector3>();
        Queue<Vector3> queue = new Queue<Vector3>();
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector3 current = queue.Dequeue();

            if (targets.TrueForAll(enemy => visited.Contains(enemy)))
            {
                return true;
            }

            foreach (Vector3 neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && !obstacles.Contains(neighbor) && walkablePositions.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return false;
    }

    List<Vector3> GetNeighbors(Vector3 position)
    {
        float xOffset = tileWidth / 2;
        float yOffset = tileHeight / 2;

        return new List<Vector3>
        {
            position + new Vector3(tileWidth, 0, 0),
            position + new Vector3(-tileWidth, 0, 0),
            position + new Vector3(xOffset, yOffset, 0),
            position + new Vector3(-xOffset, yOffset, 0),
            position + new Vector3(xOffset, -yOffset, 0),
            position + new Vector3(-xOffset, -yOffset, 0)
        };
    }
}
