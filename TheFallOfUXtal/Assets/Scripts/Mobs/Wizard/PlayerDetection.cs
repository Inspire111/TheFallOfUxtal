using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerDetect : MonoBehaviour
{
    [SerializeField] private WizardAI wizardAI;

    void Start()
    {
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            Debug.LogWarning("PlayerDetect: Collider2D should be set to 'Is Trigger'.");
            col.isTrigger = true;
        }
        if (wizardAI == null)
        {
            wizardAI = GetComponentInParent<WizardAI>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && wizardAI != null)
        {
            wizardAI.StartFiring(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && wizardAI != null)
        {
            wizardAI.StopFiring();
        }
    }
}
