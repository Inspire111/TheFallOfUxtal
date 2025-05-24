using System.Collections;
using UnityEngine;

public class MeleeHitboxMulti : MonoBehaviour
{
    public int damage = 20;
    public float activeTime = 0.1f;

    public GameObject player;

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

        MobHealthMulti mob = other.GetComponentInParent<MobHealthMulti>();
        if (mob != null)
        {
            mob.TakeDamage(damage, player);
            Debug.Log("Applied damage to " + other.name);
        }
    }

}
