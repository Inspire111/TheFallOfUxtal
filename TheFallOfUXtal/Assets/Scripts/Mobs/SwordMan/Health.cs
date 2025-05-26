using UnityEngine;

public class MobHealth : MonoBehaviour
{
    public float health = 100f;

    private bool isDead = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinCount = 4;
    [SerializeField] private float coinExplosionForce = 5f;
    [SerializeField] private float coinSpreadRadius = 0.6f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isDead && health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (coinPrefab != null)
        {
            for (int i = 0; i < coinCount; i++)
            {
                Vector2 direction = GetRandomIsometricDirection();
                Vector3 spawnOffset = (Vector3)(direction * coinSpreadRadius);
                GameObject coin = Instantiate(coinPrefab, transform.position + spawnOffset, Quaternion.identity);

                Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
                if (coinRb != null)
                {
                    coinRb.AddForce(direction * coinExplosionForce, ForceMode2D.Impulse);
                }
            }
        }

        Destroy(gameObject);
    }

    Vector2 GetRandomIsometricDirection()
    {
        // Simulate isometric angles
        float angle = Random.Range(0f, 360f);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);
        return new Vector2(x + y, y - x).normalized;
    }
}

