using UnityEngine;

public class PlayerDirectionAnimator : MonoBehaviour
{
    private Animator animator;
    private Player_mvt playerMovement;
    private PlayerAnimatorController animatorController;

    private Vector2 lastMoveDirection = Vector2.down;
    private float movementThreshold = 0.1f;

    void Start()
    {
        playerMovement = GetComponent<Player_mvt>();
        animator = GetComponentInChildren<Animator>();
        animatorController = GetComponentInChildren<PlayerAnimatorController>();
    }

    void Update()
    {
        Vector2 currentInput = playerMovement.GetInputActions().Player.Move.ReadValue<Vector2>();

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

        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("LastX", lastMoveDirection.x);
        animator.SetFloat("LastY", lastMoveDirection.y);

        // Met à jour la dernière direction connue dans PlayerAnimatorController
        if (animatorController != null)
        {
            animatorController.UpdateLastDirection(lastMoveDirection);
        }
    }
}
