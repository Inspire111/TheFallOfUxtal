using System.Collections.Generic;
using UnityEngine;

public class SpearAttack : MonoBehaviour
{
    public float attackCooldown = 0.5f;

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
    private InputSystem_Actions inputActions;
    private PlayerStats stats;

    private Dictionary<string, GameObject> hitboxes;

    void Start()
    {
        playerMovement = GetComponent<Player_mvt>();
        inputActions = playerMovement.GetInputActions();
        stats = GetComponent<PlayerStats>();

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
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (inputActions.Player.Attack.WasPressedThisFrame() && cooldownTimer <= 0f)
        {
            if (stats.currentWeapon != WeaponType.Spear)
            {
                Debug.Log("Cannot attack: not in spear mode.");
                return;
            }

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
            hitbox.SetActive(true);
        }
    }

    string GetDirectionNameFromVector(Vector3 dir)
    {
        if (dir.x > 0.5f && dir.y > 0.3f) return "UpRight";
        if (dir.x < -0.5f && dir.y > 0.3f) return "UpLeft";
        if (dir.x > 0.5f && dir.y < -0.3f) return "DownRight";
        if (dir.x < -0.5f && dir.y < -0.3f) return "DownLeft";

        if (dir.y > 0.5f) return "Up";
        if (dir.y < -0.5f) return "Down";
        if (dir.x > 0.5f) return "Right";
        if (dir.x < -0.5f) return "Left";

        return "Down";
    }
}
