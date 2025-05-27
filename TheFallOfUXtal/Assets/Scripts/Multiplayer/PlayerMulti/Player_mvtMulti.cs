using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_mvtMulti : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
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
        moveDirection = new Vector3(0f, 0f, 0f);
        
    }

    void Update()
    {
        if (!IsOwner) return;
        HandleMovementInput();
        HandleDashInput();
        UpdateDashTimers();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
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
