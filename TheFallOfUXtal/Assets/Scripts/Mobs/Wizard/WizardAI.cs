using UnityEngine;
using System.Collections;

public class WizardAI : MonoBehaviour
{
    private enum State { Move, Idle, Firing }
    private State currentState = State.Move;

    [Header("Patrol Settings")]
    [SerializeField] private Collider2D patrolZone;
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private float retreatDistance = 1f; // Distance to move away after firing

    [Header("Firing Settings")]
    [SerializeField] private GameObject magicBallPrefab;
    [SerializeField] private float firingInterval = 3f;

    private Coroutine wanderRoutine;
    private Coroutine fireRoutine;
    private Coroutine retreatRoutine;
    private Transform targetPlayer;

    void Start()
    {
        if (patrolZone == null)
        {
            var zone = transform.Find("PatrolZone");
            if (zone != null) patrolZone = zone.GetComponent<Collider2D>();
        }
        wanderRoutine = StartCoroutine(WanderRoutine());
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            currentState = State.Move;
            Bounds bounds = patrolZone.bounds;
            Vector2 target = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
            while (Vector2.Distance(transform.position, target) > 0.1f && currentState == State.Move)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    target,
                    wanderSpeed * Time.deltaTime
                );
                yield return null;
            }
            currentState = State.Idle;
            float timer = 0f;
            while (timer < idleTime && currentState == State.Idle)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            if (targetPlayer != null)
            {
                Vector2 firePosition = targetPlayer.position;
                var arrowGO = Instantiate(
                    magicBallPrefab,
                    transform.position,
                    Quaternion.identity
                );
                var arrow = arrowGO.GetComponent<MagicBall>();
                arrow.Initialize(firePosition);

                // Start retreat movement
                if (retreatRoutine != null) StopCoroutine(retreatRoutine);
                Vector2 retreatDir = ((Vector2)transform.position - firePosition).normalized;
                retreatRoutine = StartCoroutine(RetreatRoutine(retreatDir));
            }
            yield return new WaitForSeconds(firingInterval);
        }
    }

    private IEnumerator RetreatRoutine(Vector2 retreatDir)
    {
        float moved = 0f;
        Bounds bounds = patrolZone.bounds;
        while (moved < retreatDistance)
        {
            float step = wanderSpeed * Time.deltaTime;
            Vector2 newPos = (Vector2)transform.position + retreatDir * step;
            newPos.x = Mathf.Clamp(newPos.x, bounds.min.x, bounds.max.x);
            newPos.y = Mathf.Clamp(newPos.y, bounds.min.y, bounds.max.y);
            moved += Vector2.Distance(transform.position, newPos);
            transform.position = newPos;
            yield return null;
        }
    }

    public void StartFiring(Transform playerTransform)
    {
        if (wanderRoutine != null)
        {
            StopCoroutine(wanderRoutine);
            wanderRoutine = null;
        }
        targetPlayer = playerTransform;
        if (fireRoutine == null)
        {
            currentState = State.Firing;
            fireRoutine = StartCoroutine(FireRoutine());
        }
    }

    public void StopFiring()
    {
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }
        if (retreatRoutine != null)
        {
            StopCoroutine(retreatRoutine);
            retreatRoutine = null;
        }
        targetPlayer = null;
        currentState = State.Move;
        if (wanderRoutine == null)
        {
            wanderRoutine = StartCoroutine(WanderRoutine());
        }
    }
}
