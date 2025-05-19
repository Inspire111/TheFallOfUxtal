using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalMeleeAttack : MonoBehaviour
{
    public float attackCooldown = 0.5f;
    public float hitboxDisplayTime = 0.1f;

    [Header("Directional Hitboxes")]
    [SerializeField] private GameObject hitboxUp;
    [SerializeField] private GameObject hitboxDown;
    [SerializeField] private GameObject hitboxLeft;
    [SerializeField] private GameObject hitboxRight;
    [SerializeField] private GameObject hitboxUpLeft;
    [SerializeField] private GameObject hitboxUpRight;
    [SerializeField] private GameObject hitboxDownLeft;
    [SerializeField] private GameObject hitboxDownRight;

    private float cooldownTimer;
    private Player_mvt playerMovement;

    private Dictionary<string, GameObject> hitboxes;

    void Start()
    {
        playerMovement = GetComponent<Player_mvt>();

        hitboxes = new Dictionary<string, GameObject>
        {
            { "Up", hitboxUp },
            { "Down", hitboxDown },
            { "Left", hitboxLeft },
            { "Right", hitboxRight },
            { "UpLeft", hitboxUpLeft },
            { "UpRight", hitboxUpRight },
            { "DownLeft", hitboxDownLeft },
            { "DownRight", hitboxDownRight }
        };

        // Ensure all SpriteRenderers are disabled at start
        foreach (var kvp in hitboxes)
        {
            var sr = kvp.Value.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;
        }
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer <= 0f)
        {
            PerformAttack();
            cooldownTimer = attackCooldown;
        }
    }

    void PerformAttack()
    {
        Vector3 dir = playerMovement.GetLastMoveDirection().normalized;
        if (dir == Vector3.zero) return;

        string directionName = GetDirectionNameFromVector(dir);

        if (hitboxes.TryGetValue(directionName, out GameObject hitbox))
        {
            SpriteRenderer sr = hitbox.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                StartCoroutine(FlashSpriteRenderer(sr));
            }

            // Collider stays always enabled — it should be trigger-based
        }
    }

    IEnumerator FlashSpriteRenderer(SpriteRenderer sr)
    {
        sr.enabled = true;
        yield return new WaitForSeconds(hitboxDisplayTime);
        sr.enabled = false;
    }

    string GetDirectionNameFromVector(Vector3 dir)
    {
        // Diagonal priority
        if (dir.x > 0.5f && dir.y > 0.3f) return "UpRight";
        if (dir.x < -0.5f && dir.y > 0.3f) return "UpLeft";
        if (dir.x > 0.5f && dir.y < -0.3f) return "DownRight";
        if (dir.x < -0.5f && dir.y < -0.3f) return "DownLeft";

        if (dir.y > 0.5f) return "Up";
        if (dir.y < -0.5f) return "Down";
        if (dir.x > 0.5f) return "Right";
        if (dir.x < -0.5f) return "Left";

        return "Down"; // fallback
    }
}
