using UnityEngine;

public class MobHealth : MonoBehaviour
{
    public float health = 100f;

    private bool isDead = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

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

        Destroy(gameObject);
    }
}
