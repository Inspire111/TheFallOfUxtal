using System.Collections.Generic;
using UnityEngine;

public class DirectionalMeleeAttack : MonoBehaviour
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
    private PlayerAnimatorController animatorController;

    private Dictionary<string, GameObject> hitboxes;

    void Start()
    {
        playerMovement = GetComponent<Player_mvt>();
        inputActions = playerMovement.GetInputActions();
        animatorController = GetComponentInChildren<PlayerAnimatorController>();

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
            PerformAttack();
            cooldownTimer = attackCooldown;
        }
    }

    void PerformAttack()
    {
        Vector3 dir = playerMovement.GetLastMoveDirection().normalized;

        // Si aucune direction de mouvement, utiliser la dernière direction connue
        if (dir == Vector3.zero)
        {
            dir = animatorController.GetLastMoveDirection();
        }

        animatorController.PlayAttackAnimation(dir);

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
