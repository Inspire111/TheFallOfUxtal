using UnityEngine;

public class SoldierAnimator : MonoBehaviour
{
    private Animator animator;

    private Vector2 lastMoveDirection = Vector2.down; // direction par défaut
    private float movementThreshold = 0.01f; // seuil pour détecter si le soldat bouge

    private Vector3 lastPosition;
    private bool lastIsMoving = false;
    private Vector2 lastAnimatorDirection = Vector2.zero;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        lastPosition = transform.position;
        lastAnimatorDirection = lastMoveDirection;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 delta = currentPosition - lastPosition;
        Vector2 moveDir = new Vector2(delta.x, delta.y);

        bool isMoving = moveDir.magnitude > movementThreshold;

        Vector2 quantizedDir = lastMoveDirection;

        if (isMoving)
        {
            quantizedDir = Vector2.zero;
            if (Mathf.Abs(moveDir.x) > 0.1f)
                quantizedDir.x = Mathf.Sign(moveDir.x);
            if (Mathf.Abs(moveDir.y) > 0.1f)
                quantizedDir.y = Mathf.Sign(moveDir.y);

            if (quantizedDir == Vector2.zero)
            {
                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                    quantizedDir.x = Mathf.Sign(moveDir.x);
                else
                    quantizedDir.y = Mathf.Sign(moveDir.y);
            }

            lastMoveDirection = quantizedDir;
        }

        // Mise à jour seulement si changement
        if (isMoving != lastIsMoving)
        {
            animator.SetBool("IsMoving", isMoving);
            lastIsMoving = isMoving;
        }

        if (quantizedDir != lastAnimatorDirection)
        {
            animator.SetFloat("LastX", quantizedDir.x);
            animator.SetFloat("LastY", quantizedDir.y);
            lastAnimatorDirection = quantizedDir;
        }

        lastPosition = currentPosition;
    }
}
