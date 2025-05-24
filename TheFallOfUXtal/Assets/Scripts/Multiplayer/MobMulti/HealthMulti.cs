using Unity.Netcode;
using UnityEngine;

public class MobHealthMulti : NetworkBehaviour
{
    public float Health = 100f;

    private bool isDead = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private GameObject lastAttacker; // Track the last player that hit this mob

    public void TakeDamage(float amount, GameObject player)
    {
        Debug.Log("Is taking damage");
        if (isDead)
        {
            Die();
            return;
        }
        Health -= amount;
        lastAttacker = player;

        if (Health <= 0 && !isDead)
        {
            Die();
        }
       
        
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }



    void Die()
    {
        isDead = true;
        AwardScoreTo(lastAttacker); // Only done on server
        AwardHeal(lastAttacker);
        if (IsHost) Destroy(gameObject);
        else RequestDestroyServerRpc(GetComponent<NetworkObject>().NetworkObjectId);
    }

    // Called by the mob when dying
    public void AwardScoreTo(GameObject killer)
    {
        if (!IsServer) return;

        var scoreComp = killer.GetComponent<PlayerScore>();
        Debug.Log("score is null : " + scoreComp is null);
        if (scoreComp != null)
        {
            Debug.Log("points attributed to : " + killer.name);
            scoreComp.AddScore(1); // Add reward
        }
    }

    public void AwardHeal(GameObject killer)
    {
        if (!IsServer) return;

        var playerStat = killer.GetComponent<PlayerStatsMulti>();
        if (playerStat != null)
        {
            playerStat.Heal(20); // Add reward
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestDestroyServerRpc(ulong networkObjectId)
    {
        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var netObj))
        {
            Destroy(netObj.gameObject); // or GameObject.Destroy(netObj.gameObject);
        }
    }

}
