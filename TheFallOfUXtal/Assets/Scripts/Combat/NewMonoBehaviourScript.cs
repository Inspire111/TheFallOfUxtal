using UnityEngine;
using System.Collections.Generic;

public class GridCombatManager : MonoBehaviour
{
    public int gridSize = 20;
    public float tileSize = 1.5f;
    public GameObject walkableTilePrefab;
    public GameObject obstacleTilePrefab;
    public Transform gridParent;

    private GameObject[,] gridArray;
    private List<Vector2Int> obstaclePositions = new List<Vector2Int>();

    private bool arenaGenerated = false;

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
        gridArray = new GameObject[gridSize, gridSize];
        Vector3 gridOrigin = gridParent.position;

        int halfGridSize = gridSize / 2;

        for (int y = 0; y < gridSize; y++)
        {
            int rowWidth = gridSize - Mathf.Abs(y - halfGridSize);
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

                if (Random.Range(0, 10) < 3 && !IsBorderTile(tilePosition))
                {
                    obstaclePositions.Add(tilePosition);
                    ReplaceWithObstacleTile(tile, worldPosition);
                }
            }
        }
    }

    void ReplaceWithObstacleTile(GameObject tile, Vector3 worldPosition)
    {
        Destroy(tile);
        Instantiate(obstacleTilePrefab, worldPosition, Quaternion.identity, gridParent);
    }

    bool IsBorderTile(Vector2Int position)
    {
        return position.x == 0 || position.y == 0 || position.x == gridSize - 1 || position.y == gridSize - 1;
    }
}
