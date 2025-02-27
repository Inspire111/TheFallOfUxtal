using UnityEngine;
using System.Collections.Generic;

public class GridCombatManager : MonoBehaviour
{
    public int gridSize = 20; // Size of the diamond grid
    public float tileSize = 1.5f;
    public GameObject walkableTilePrefab;
    public GameObject obstacleTilePrefab;
    public Transform gridParent;

    public GameObject player;
    private Vector2Int playerSpawnPosition;
    public GameObject[,] gridArray;
    private List<Vector2Int> possibleSpawns = new List<Vector2Int>();

    private bool arenaGenerated = false; // Only generate when player presses "E"

    void Update()
    {
        // Wait for player to press "E" to generate the arena
        if (!arenaGenerated && Input.GetKeyDown(KeyCode.E))
        {
            GenerateDiamondArena();
            PlacePlayer();
            arenaGenerated = true;
        }
    }

    void GenerateDiamondArena()
    {
        gridArray = new GameObject[gridSize, gridSize];
        Vector3 gridOrigin = gridParent.position;

        for (int y = 0; y < gridSize; y++)
        {
            int rowWidth = gridSize - Mathf.Abs(y - (gridSize / 2));
            int xOffset = (gridSize - rowWidth) / 2;

            for (int x = 0; x < rowWidth; x++)
            {
                Vector2Int tilePosition = new Vector2Int(x - xOffset, y);
                Vector3 worldPosition = new Vector3(
                    gridOrigin.x + (x - xOffset) * tileSize,
                    gridOrigin.y + y * tileSize,
                    0
                );

                GameObject tile = Instantiate(walkableTilePrefab, worldPosition, Quaternion.identity, gridParent);
                gridArray[x, y] = tile;

                if ((y == 0 && x == 0) || (y == 0 && x == rowWidth - 1) ||
                    (y == gridSize - 1 && x == 0) || (y == gridSize - 1 && x == rowWidth - 1))
                {
                    possibleSpawns.Add(tilePosition);
                }
            }
        }

        PlaceObstacles();
    }

    void PlaceObstacles()
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (gridArray[x, y] != null || possibleSpawns.Contains(position))
                    continue;

                Vector3 worldPosition = new Vector3(
                    gridParent.position.x + (x - gridSize / 2) * tileSize,
                    gridParent.position.y + (y - gridSize / 2) * tileSize,
                    0
                );

                Instantiate(obstacleTilePrefab, worldPosition, Quaternion.identity, gridParent);
            }
        }
    }

    void PlacePlayer()
    {
        playerSpawnPosition = possibleSpawns[Random.Range(0, possibleSpawns.Count)];

        Vector3 playerWorldPos = new Vector3(
            gridParent.position.x + (playerSpawnPosition.x - gridSize / 2) * tileSize,
            gridParent.position.y + (playerSpawnPosition.y - gridSize / 2) * tileSize,
            0
        );

        player.transform.position = playerWorldPos;
    }
}