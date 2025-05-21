using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DamageZone : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float initialDelay = 0.2f;
    [SerializeField] private float damageInterval = 0.4f;

    private Coroutine damageCoroutine;

    private void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            Debug.LogWarning("DamageZone: Collider2D should be set to 'Is Trigger'.");
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            PlayerStats player = other.GetComponent<PlayerStats>();
            if (player != null)
            {
                damageCoroutine = StartCoroutine(ApplyDamageOverTime(player));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator ApplyDamageOverTime(PlayerStats player)
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            player.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}

