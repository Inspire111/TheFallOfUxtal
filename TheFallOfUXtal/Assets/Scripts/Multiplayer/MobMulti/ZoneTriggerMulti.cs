using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZoneTriggerMulti : MonoBehaviour
{
    [SerializeField] private SwordManAIMulti swordManAI;

    void Start()
    {
        if (!GetComponent<Collider2D>().isTrigger)
            Debug.LogWarning("ZoneTrigger collider should be set as Trigger");

        if (swordManAI == null)
            swordManAI = GetComponentInParent<SwordManAIMulti>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Player") && swordManAI != null)
        {
            swordManAI.StartChase(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && swordManAI != null)
        {
            swordManAI.StopChase();
        }
    }
}
