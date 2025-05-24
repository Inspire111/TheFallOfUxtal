using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArrowProjectile : MonoBehaviour
{
    public float maxSpeed = 20f;             // Speed of a fully charged arrow
    public int minDamage = 5;                // Minimum damage
    public int maxDamage = 50;               // Maximum damage
    public float minLiveSpeed = 0.5f;        // Speed threshold for despawning
    public float checkInterval = 0.1f;       // How often to check speed

    private Rigidbody2D rb;
    private float currentDamage;
    private Vector2 initialVelocity;

    // Initialize the arrow with speed, charge %, and direction
    public void Initialize(float speed, float chargePercent, Vector2 direction)
    {
        float expCharge = Mathf.Pow(chargePercent, 2f);  // exponential charge curve
        currentDamage = Mathf.Lerp(minDamage, maxDamage, expCharge);

        initialVelocity = direction.normalized * speed;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found on arrow!");
            Destroy(gameObject);
            return;
        }

        rb.linearVelocity = initialVelocity;

        InvokeRepeating(nameof(CheckVelocity), checkInterval, checkInterval);
    }

    private void CheckVelocity()
    {
        if (rb.linearVelocity.magnitude < minLiveSpeed)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
            MobHealth mob = other.GetComponentInParent<MobHealth>();
            if (mob != null)
            {
                mob.health -= Mathf.RoundToInt(currentDamage);
                Debug.Log($"Arrow hit {other.name}, dealt {currentDamage:F1} damage.");
            }

            Destroy(gameObject);
        
    }

}
