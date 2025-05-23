using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    private enum State { Move, Idle, Firing, Summoning }
    private State currentState = State.Move;

    [Header("Patrol Settings")]
    [SerializeField] private Collider2D patrolZone;
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private float retreatDistance = 1f;

    [Header("Orb Barrage Settings")]
    [SerializeField] private GameObject orbPrefab;
    [SerializeField] private int orbCount = 10;
    [SerializeField] private float barrageInterval = 3f;

    [Header("Summon Settings")]
    [SerializeField] private GameObject wizardPrefab;
    [SerializeField] private GameObject swordManPrefab;
    [SerializeField] private float summonInterval = 8f;

    private Coroutine wanderRoutine;
    private Coroutine fireRoutine;
    private Coroutine retreatRoutine;
    private Coroutine summonRoutine;

    private void Start()
    {
        if (patrolZone == null)
        {
            var zone = transform.Find("PatrolZone");
            if (zone != null) patrolZone = zone.GetComponent<Collider2D>();
        }
        wanderRoutine = StartCoroutine(WanderRoutine());
        fireRoutine = StartCoroutine(FireBarrageRoutine());
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

    private IEnumerator FireBarrageRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(barrageInterval);
            // Fire orbs in a circle
            for (int i = 0; i < orbCount; i++)
            {
                float angle = i * 360f / orbCount;
                Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
                Vector2 spawnTarget = (Vector2)transform.position + dir * patrolZone.bounds.extents.magnitude;
                GameObject orbGO = Instantiate(orbPrefab, transform.position, Quaternion.identity);
                var arrow = orbGO.GetComponent<MagicBall>();
                arrow.Initialize(spawnTarget);
            }
            // Retreat opposite to last fired direction (optional)
            if (retreatRoutine != null) StopCoroutine(retreatRoutine);
            Vector2 retreatDir = (transform.position - (Vector3)Vector2.zero).normalized; // retreat outward
            retreatRoutine = StartCoroutine(RetreatRoutine(retreatDir));
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

    public void StartSummoning()
    {
        if (summonRoutine == null)
        {
            summonRoutine = StartCoroutine(SummonRoutine());
        }
    }

    public void StopSummoning()
    {
        if (summonRoutine != null)
        {
            StopCoroutine(summonRoutine);
            summonRoutine = null;
        }
    }

    private IEnumerator SummonRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(summonInterval);
            // Randomly spawn Wizard or SwordMan
            GameObject prefab = Random.value < 0.5f ? wizardPrefab : swordManPrefab;
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
}