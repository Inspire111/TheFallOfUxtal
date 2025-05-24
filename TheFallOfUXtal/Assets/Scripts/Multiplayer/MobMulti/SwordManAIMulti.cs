using UnityEngine;
using System.Collections;

public class SwordManAIMulti : MonoBehaviour
{
    private enum State { Move, Idle, Chase }
    private State currentState = State.Move;

    [Header("Zone & Targets")]
    [SerializeField] private Collider2D zoneCollider;

    [Header("Movement Settings")]
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float idleTime = 2f;

    private Vector2 targetPosition;
    private Coroutine wanderRoutine;
    private Transform chaseTarget;

    void Start()
    {
        if (zoneCollider == null)
            zoneCollider = GameObject.FindWithTag("Zone").GetComponent<Collider2D>();

        wanderRoutine = StartCoroutine(WanderRoutine());
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            currentState = State.Move;
            Bounds b = zoneCollider.bounds;
            targetPosition = new Vector2(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y)
            );
            while (Vector2.Distance(transform.position, targetPosition) > 0.1f
                   && currentState == State.Move)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position, targetPosition,
                    wanderSpeed * Time.deltaTime
                );
                yield return null;
            }
            currentState = State.Idle;
            float t = 0f;
            while (t < idleTime && currentState == State.Idle)
            {
                t += Time.deltaTime;
                yield return null;
            }
        }
    }

    void Update()
    {
        if (currentState == State.Chase && chaseTarget != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                chaseTarget.position,
                chaseSpeed * Time.deltaTime
            );
        }
    }

    // Called by ZoneTrigger: assign target and begin chase
    public void StartChase(Transform playerTransform)
    {
        if (wanderRoutine != null)
        {
            StopCoroutine(wanderRoutine);
            wanderRoutine = null;
        }
        chaseTarget = playerTransform;
        currentState = State.Chase;
    }

    // Called by ZoneTrigger: clear target and resume wander
    public void StopChase()
    {
        if (currentState == State.Chase)
        {
            chaseTarget = null;
            currentState = State.Move;
            if (wanderRoutine == null)
                wanderRoutine = StartCoroutine(WanderRoutine());
        }
    }
}