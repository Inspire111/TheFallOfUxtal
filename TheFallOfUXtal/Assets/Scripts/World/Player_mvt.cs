using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_mvt : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private TrailRenderer trail;
    private InputSystem_Actions inputActions;

    private Vector3 moveDirection;
    public Vector3 lastMoveDirection { get; private set; }

    private float dashTimeRemaining;
    private float dashCooldownRemaining;
    private bool isDashing;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

        trail.enabled = false;
        if (trail.material == null)
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = Color.white;
            trail.material = mat;
        }

        trail.time = 0.3f;
        trail.startWidth = 0.2f;
        trail.endWidth = 0.1f;
    }

    void Update()
    {
        HandleMovementInput();
        HandleDashInput();
        UpdateDashTimers();
    }

    void FixedUpdate()
    {
        float speed = isDashing ? dashSpeed : moveSpeed;
        Vector2 velocity = moveDirection.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + velocity);
    }

    void HandleMovementInput()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        moveDirection = new Vector3(input.x, input.y, 0f);

        if (moveDirection != Vector3.zero)
            lastMoveDirection = moveDirection;
    }

    void HandleDashInput()
    {
        if (inputActions.Player.Sprint.WasPressedThisFrame() && dashCooldownRemaining <= 0f && moveDirection != Vector3.zero)
        {
            isDashing = true;
            dashTimeRemaining = dashDuration;
            dashCooldownRemaining = dashCooldown;
            trail.enabled = true;
        }
    }

    void UpdateDashTimers()
    {
        if (isDashing)
        {
            dashTimeRemaining -= Time.deltaTime;
            if (dashTimeRemaining <= 0f)
            {
                isDashing = false;
                trail.enabled = false;
            }
        }

        if (dashCooldownRemaining > 0f)
        {
            dashCooldownRemaining -= Time.deltaTime;
        }
    }

    public Vector3 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }

    public InputSystem_Actions GetInputActions()
    {
        return inputActions;
    }
}
