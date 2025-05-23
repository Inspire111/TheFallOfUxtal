using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerDetectBoss : MonoBehaviour
{
    [SerializeField] private BossAI bossAI;

    private void Start()
    {
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger) col.isTrigger = true;
        if (bossAI == null) bossAI = GetComponentInParent<BossAI>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && bossAI != null)
        {
            bossAI.StartSummoning();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && bossAI != null)
        {
            bossAI.StopSummoning();
        }
    }
}