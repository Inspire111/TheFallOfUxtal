using System.Collections;
using UnityEngine;

public class MeleeHitboxWithStun : MonoBehaviour
{
    public int damage = 20;
    public float stunDuration = 10.0f;
    public float activeTime = 0.1f;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        StartCoroutine(DisableAfterDelay());
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(activeTime);

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hitbox triggered with: " + other.name);

        MobHealth mob = other.GetComponentInParent<MobHealth>();
        if (mob != null)
        {
            mob.health -= damage;
            Debug.Log("Applied damage to " + other.name);

            Rigidbody2D mobRb = mob.GetComponent<Rigidbody2D>();
            if (mobRb != null)
            {
                StartCoroutine(StunCoroutine(mobRb));
                Debug.Log($"Stunned {other.name} for {stunDuration} seconds.");
            }
        }
    }

    private IEnumerator StunCoroutine(Rigidbody2D rb)
    {
        RigidbodyConstraints2D originalConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;

        yield return new WaitForSeconds(stunDuration);

        rb.constraints = originalConstraints;
    }
}

