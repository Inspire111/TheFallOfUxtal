using Unity.Netcode;
using UnityEngine;

public class MobHealthMulti : NetworkBehaviour
{
    public float health = 100f;

    private bool isDead = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private GameObject lastAttacker; // Track the last player that hit this mob

    public void TakeDamage(float amount, GameObject attacker)
    {
        if (!IsServer) return;

        health -= amount;
        lastAttacker = attacker;

        if (health <= 0 && !isDead)
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
        Destroy(gameObject);
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

}
