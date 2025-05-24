using UnityEngine;

public class BowAttack : MonoBehaviour
{
    [Header("Bow Settings")]
    public GameObject arrowPrefab;
    public float maxChargeTime = 1f;     // Reduced charge time for faster charging
    public float minArrowSpeed = 5f;
    public float maxArrowSpeed = 20f;

    private float currentChargeTime = 0f;
    private bool isCharging = false;

    private Player_mvt playerMovement;
    private InputSystem_Actions inputActions;
    private PlayerStats stats;

    void Start()
    {
        playerMovement = GetComponent<Player_mvt>();
        inputActions = playerMovement.GetInputActions();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (stats.currentWeapon != WeaponType.Bow) return;

        if (inputActions.Player.Attack.IsPressed())
        {
            if (!isCharging)
            {
                isCharging = true;
                currentChargeTime = 0f;
            }

            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Min(currentChargeTime, maxChargeTime);
        }

        if (isCharging && inputActions.Player.Attack.WasReleasedThisFrame())
        {
            ShootArrow();
            isCharging = false;
            currentChargeTime = 0f;
        }
    }

    void ShootArrow()
    {
        Vector2 direction = playerMovement.GetLastMoveDirection().normalized;
        if (direction == Vector2.zero) direction = Vector2.down;

        float chargePercent = currentChargeTime / maxChargeTime;
        float expCharge = Mathf.Pow(chargePercent, 2f);

        float arrowSpeed = Mathf.Lerp(minArrowSpeed, maxArrowSpeed, expCharge);

        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        ArrowProjectile arrowProj = arrow.GetComponent<ArrowProjectile>();
        if (arrowProj != null)
        {
            arrowProj.Initialize(arrowSpeed, chargePercent, direction);
        }
        else
        {
            Debug.LogError("Arrow prefab missing ArrowProjectile script!");
        }
    }
}
