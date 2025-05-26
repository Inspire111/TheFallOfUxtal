using UnityEngine;

public class RespawnTile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.SetRespawnPoint(transform);
                Debug.Log("New respawn point set!");
            }
        }
    }
}
