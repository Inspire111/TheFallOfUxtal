using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPathfinding : MonoBehaviour
{
    public GameObject player;
    public float movementSpeed = 2f;
    public float tileWidth = 2f;
    public float tileHeight = 1f;

    private Vector3 targetPosition;
    private List<Vector3> path = new List<Vector3>();
    private bool isPathSelected = false;
    private bool isFollowingPath = false;

    public Color pathHighlightColor = Color.yellow;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = player.GetComponent<PlayerStats>();  // Reference to the player's stats
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isFollowingPath)
        {
            SelectTargetTile();
        }

        if (isPathSelected && Input.GetMouseButtonDown(1))  // Right-click to start movement
        {
            StartCoroutine(FollowPath());
        }
    }

    // Allow the player to select a target tile
    private void SelectTargetTile()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("WalkableTile") && !hit.collider.CompareTag("Obstacle"))
            {
                targetPosition = hit.collider.transform.position;
                path = FindPath(player.transform.position, targetPosition);

                if (path.Count > 0)
                {
                    HighlightPath(path);
                    isPathSelected = true;
                }
            }
        }
    }

    private List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        List<Vector3> foundPath = new List<Vector3>();
        Vector3 current = start;

        while (Vector3.Distance(current, end) > 0.1f)
        {
            Vector3 bestNeighbor = GetBestNeighbor(current, end);
            foundPath.Add(bestNeighbor);
            current = bestNeighbor;
        }

        foundPath.Add(end);
        return foundPath;
    }


    private Vector3 GetBestNeighbor(Vector3 current, Vector3 end)
    {
        List<Vector3> neighbors = GetNeighbors(current);
        Vector3 bestNeighbor = neighbors[0];
        float bestDistance = Vector3.Distance(bestNeighbor, end);

        foreach (Vector3 neighbor in neighbors)
        {
            float distance = Vector3.Distance(neighbor, end);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestNeighbor = neighbor;
            }
        }

        return bestNeighbor;
    }

    private List<Vector3> GetNeighbors(Vector3 position)
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

    private void HighlightPath(List<Vector3> path)
    {
        foreach (Vector3 position in path)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("WalkableTile"))
                {
                    collider.GetComponent<SpriteRenderer>().color = pathHighlightColor;
                }
            }
        }
    }

    private IEnumerator FollowPath()
    {
        if (playerStats.currentEnergy <= 0)
        {
            Debug.LogWarning("Not enough energy to move.");
            yield break;
        }

        isFollowingPath = true;
        int energyRequired = path.Count;

        if (playerStats.currentEnergy < energyRequired)
        {
            Debug.LogWarning("Not enough energy to move the entire path.");
            isFollowingPath = false;
            yield break;
        }

        foreach (Vector3 waypoint in path)
        {
            float distanceToWaypoint = Vector3.Distance(player.transform.position, waypoint);

            while (distanceToWaypoint > 0.1f)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, waypoint, movementSpeed * Time.deltaTime);
                distanceToWaypoint = Vector3.Distance(player.transform.position, waypoint);
                yield return null;
            }

            playerStats.UseEnergy(1); 

            ResetHighlightedPathTile(waypoint);
        }

        isFollowingPath = false;
    }

    private void ResetHighlightedPathTile(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("WalkableTile"))
            {
                collider.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}

