using Unity.Netcode;
using UnityEngine;

public class PlayerDirectionAnimatorMulti : NetworkBehaviour
{
    private Animator animator;
    private Player_mvtMulti playerMovement;
    private PlayerAnimatorController animatorController;

    private Vector2 lastMoveDirection = Vector2.down;
    private float movementThreshold = 0.1f;

    private Vector2 currentInput;

    void Start()
    {
        playerMovement = GetComponent<Player_mvtMulti>();
        animator = GetComponentInChildren<Animator>();
        animatorController = GetComponentInChildren<PlayerAnimatorController>();
    }

    void Update()
    {
        // ✅ Only the owner reads input
        if (IsOwner)
        {
            currentInput = playerMovement.GetInputActions().Player.Move.ReadValue<Vector2>();
        }

        bool isMoving = currentInput.magnitude > movementThreshold;

        if (isMoving)
        {
            Vector2 rawInput = currentInput.normalized;
            Vector2 quantizedDir = Vector2.zero;

            if (Mathf.Abs(rawInput.x) > 0.5f)
                quantizedDir.x = Mathf.Sign(rawInput.x);
            if (Mathf.Abs(rawInput.y) > 0.5f)
                quantizedDir.y = Mathf.Sign(rawInput.y);

            lastMoveDirection = quantizedDir;
        }

        // ✅ This runs for everyone (host/client)
        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("LastX", lastMoveDirection.x);
        animator.SetFloat("LastY", lastMoveDirection.y);

        if (animatorController != null)
        {
            animatorController.UpdateLastDirection(lastMoveDirection);
        }
    }
}
