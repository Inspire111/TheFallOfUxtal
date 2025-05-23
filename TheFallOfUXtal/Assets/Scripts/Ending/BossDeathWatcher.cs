using UnityEngine;

public class BossDeathWatcher : MonoBehaviour
{
    [Tooltip("Assign your Boss GameObject here (or find by tag).")]
    [SerializeField] private MobHealth bossHealth;

    [SerializeField] private EndSequenceController endController;

    void Start()
    {

        // If you didn’t assign the bossHealth in inspector, try finding by tag:
        if (bossHealth == null)
        {
            var boss = GameObject.FindGameObjectWithTag("Boss");
            if (boss != null)
                bossHealth = boss.GetComponent<MobHealth>();
        }

        // Start polling once per second, beginning after 1s
        InvokeRepeating(nameof(CheckBossDeath), 1f, 1f);
    }

    void CheckBossDeath()
    {
        // If the bossHealth reference is gone (destroyed), or its health ≤ 0, trigger end
        if (bossHealth == null || bossHealth.health <= 0f)
        {
            // Stop further checks
            CancelInvoke(nameof(CheckBossDeath));

            // Trigger the fade-and-menu sequence
            if (endController != null)
                endController.TriggerEndSequence();
            else
                Debug.LogError("No EndSequenceController found in scene!");
        }
    }
}
