using System.Collections;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public int damage = 20;
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

        MobHealth mob = other.GetComponent<MobHealth>();
        if (mob != null)
        {
            mob.health -= damage;
            Debug.Log("Applied damage to " + other.name);
        }
    }

}
