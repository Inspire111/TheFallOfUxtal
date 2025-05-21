using Unity.Netcode;
using UnityEngine;

public class Player_mvt : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private Vector3 moveDirection;

    private float dashTimeRemaining;
    private float dashCooldownRemaining;
    private bool isDashing;

    private readonly Vector3 upLeft = new Vector3(-1, 0.5f, 0);
    private readonly Vector3 upRight = new Vector3(1, 0.5f, 0);
    private readonly Vector3 downLeft = new Vector3(-1, -0.5f, 0);
    private readonly Vector3 downRight = new Vector3(1, -0.5f, 0);
    private readonly Vector3 left = new Vector3(-1, 0, 0);
    private readonly Vector3 right = new Vector3(1, 0, 0);

    public Vector3 lastMoveDirection { get; private set; }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleWASDMovement();
        HandleDashInput();
        UpdateDashTimers();
    }


    void HandleWASDMovement()
    {
        moveDirection = Vector3.zero;

        bool w = Input.GetKey(KeyCode.W);
        bool a = Input.GetKey(KeyCode.A);
        bool s = Input.GetKey(KeyCode.S);
        bool d = Input.GetKey(KeyCode.D);

        if (w && !a && !d) moveDirection += Vector3.up;
        if (s && !a && !d) moveDirection += Vector3.down;
        if (w && a) moveDirection += upLeft;
        if (w && d) moveDirection += upRight;
        if (s && a) moveDirection += downLeft;
        if (s && d) moveDirection += downRight;
        if (a && !w && !s) moveDirection += left;
        if (d && !w && !s) moveDirection += right;

        if (moveDirection != Vector3.zero)
            lastMoveDirection = moveDirection;

    }

    void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownRemaining <= 0f && moveDirection != Vector3.zero)
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

    void FixedUpdate()
    {

        float currentSpeed = isDashing ? dashSpeed : moveSpeed;
        rb.MovePosition(rb.position + (Vector2)(moveDirection * currentSpeed * Time.fixedDeltaTime));
    }

    public Vector3 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }

}

