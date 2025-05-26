using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastDirection = Vector3.down; // direction par défaut

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Met à jour la dernière direction connue (appelé depuis PlayerDirectionAnimator)
    public void UpdateLastDirection(Vector3 dir)
    {
        if (dir != Vector3.zero)
            lastDirection = dir;
    }

    // Permet de récupérer la dernière direction connue
    public Vector3 GetLastMoveDirection()
    {
        return lastDirection;
    }

    // Lance l'animation d'attaque dans la bonne direction
    public void PlayAttackAnimation(Vector3 dir)
    {
        UpdateLastDirection(dir);
        animator.SetFloat("LastX", dir.x);
        animator.SetFloat("LastY", dir.y);
        animator.SetTrigger("Attack");
    }
}
