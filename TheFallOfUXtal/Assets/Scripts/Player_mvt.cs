using UnityEngine;
using Unity.Netcode;
public class Player_mvt : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private Vector3 moveDirection;

    private readonly Vector3 upLeft = new Vector3(-1, 0.5f, 0);
    private readonly Vector3 upRight = new Vector3(1, 0.5f, 0);
    private readonly Vector3 downLeft = new Vector3(-1, -0.5f, 0);
    private readonly Vector3 downRight = new Vector3(1, -0.5f, 0);

    private readonly Vector3 left = new Vector3(-1, 0, 0);
    private readonly Vector3 right = new Vector3(1, 0, 0);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
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

        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + (Vector2)(moveDirection * moveSpeed * Time.fixedDeltaTime));
    }
}
