using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZoneTrigger : MonoBehaviour
{
    [SerializeField] private SwordManAI swordManAI;

    void Start()
    {
        if (!GetComponent<Collider2D>().isTrigger)
            Debug.LogWarning("ZoneTrigger collider should be set as Trigger");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            swordManAI.StartChase();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            swordManAI.StopChase();
        }
    }
}
