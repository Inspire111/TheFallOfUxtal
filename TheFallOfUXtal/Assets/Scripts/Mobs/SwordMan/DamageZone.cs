using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageZone : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";

    void Start()
    {
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger)
            Debug.LogWarning("DamageZone collider should be set as Trigger");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log("i will implement damage interaction later, but at least you see that this triggers");
        }
    }
}
