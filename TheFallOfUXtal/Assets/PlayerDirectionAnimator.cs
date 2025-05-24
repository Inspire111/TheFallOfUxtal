using UnityEngine;

public class PlayerDirectionAnimator : MonoBehaviour
{
    private Animator animator;
    private Player_mvt playerMovement;

    private Vector2 lastMoveDirection = Vector2.down;
    private float movementThreshold = 0.1f;

    void Start()
    {
        playerMovement = GetComponent<Player_mvt>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Vector2 currentInput = playerMovement.GetInputActions().Player.Move.ReadValue<Vector2>();

        bool isMoving = currentInput.magnitude > movementThreshold;

        // Si le joueur bouge, on met à jour la direction avec une direction "quantifiée"
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

        // Animation parameters
        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("LastX", lastMoveDirection.x);
        animator.SetFloat("LastY", lastMoveDirection.y);
    }
}
