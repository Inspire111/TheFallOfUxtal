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

    public List<Vector3> walkablePositions = new List<Vector3>();  // Stores all walkable positions

    private GameObject[][] walkableTiles;
    private bool arenaGenerated = false;
    private Vector3 playerPosition;
    private List<Vector3> enemyPositions = new List<Vector3>();

    void Update()
    {
        if (!arenaGenerated && Input.GetKeyDown(KeyCode.E))
        {
            GenerateDiamondArena();
            arenaGenerated = true;
        }
    }

    void GenerateDiamondArena()
    {
        Vector3 gridOrigin = gridParent.position;
        int totalRows = gridSize * 2 - 1;
        walkableTiles = new GameObject[totalRows][];

        List<Vector3> availablePositions = new List<Vector3>();
        Vector3 topCenterPosition = Vector3.zero;

        for (int y = 0; y < totalRows; y++)
        {
            int rowWidth = (y < gridSize) ? y + 1 : totalRows - y;
            walkableTiles[y] = new GameObject[rowWidth];

            for (int x = 0; x < rowWidth; x++)
            {
                float isoX = (x - rowWidth / 2f) * tileWidth;
                float isoY = -y * 0.5f * tileHeight;
                Vector3 worldPosition = new Vector3(gridOrigin.x + isoX, gridOrigin.y + isoY, 0);

                GameObject walkableTile = Instantiate(walkableTilePrefab, worldPosition, Quaternion.identity, gridParent);
                walkableTiles[y][x] = walkableTile;

                availablePositions.Add(worldPosition);

                // Explicitly set player spawn position at the **top-center**
                if (y == 0 && x == rowWidth / 2)
                {
                    topCenterPosition = worldPosition;
                }
            }
        }

        // Store all positions as walkable initially
        walkablePositions.AddRange(availablePositions);

        // Place player at the top-center
        playerPosition = topCenterPosition;
        Instantiate(playerSpawnTilePrefab, playerPosition, Quaternion.identity, gridParent);
        availablePositions.Remove(playerPosition); // Still walkable but not available for obstacles

        // Place enemies in the **bottom half**
        for (int i = 0; i < numberOfEnemies && availablePositions.Count > 0; i++)
        {
            int randomIndex = Random.Range(availablePositions.Count / 2, availablePositions.Count);
            Vector3 enemyPosition = availablePositions[randomIndex];
            Instantiate(enemySpawnTilePrefab, enemyPosition, Quaternion.identity, gridParent);
            enemyPositions.Add(enemyPosition);
            availablePositions.RemoveAt(randomIndex); // Still walkable but reserved for enemies
        }

        // Place obstacles while ensuring connectivity
        PlaceObstacles(availablePositions);
    }

    void PlaceObstacles(List<Vector3> availablePositions)
    {
        List<Vector3> placedObstacles = new List<Vector3>();

        for (int i = 0; i < numberOfObstacles && availablePositions.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector3 obstaclePosition = availablePositions[randomIndex];

            // Temporarily place obstacle and check connectivity
            placedObstacles.Add(obstaclePosition);

            if (IsArenaConnected(playerPosition, enemyPositions, placedObstacles))
            {
                Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity, gridParent);
                availablePositions.RemoveAt(randomIndex);
                walkablePositions.Remove(obstaclePosition); // Remove from walkable list
            }
            else
            {
                // If the obstacle disconnects the arena, remove it
                placedObstacles.Remove(obstaclePosition);
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

            // Check if all enemies are reachable
            if (targets.TrueForAll(enemy => visited.Contains(enemy)))
            {
                return true;
            }

            // Check neighbors (assuming a hexagonal-like grid movement)
            foreach (Vector3 neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && !obstacles.Contains(neighbor) && walkablePositions.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return false; // Not all enemies are reachable
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