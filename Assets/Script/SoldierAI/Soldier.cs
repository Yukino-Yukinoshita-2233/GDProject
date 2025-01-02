using System.Collections.Generic;
using UnityEngine;
using MapManagernamespace;
using System.Linq;

public enum SoldierState
{
    Idle,
    Moving,
    Attacking,
    Dead
}

public class Soldier : MonoBehaviour
{
    public SoldierState currentState = SoldierState.Idle;
    public Transform target;
    public float moveSpeed = 3.0f;
    public float attackCooldown = 1.0f;
    public float attackDamage = 10;
    public float maxHealth = 1000f;
    private float lastAttackTime = 0f;
    private List<LPAStarNode> path = new List<LPAStarNode>();
    private int currentPathIndex = 0;
    private Vector2Int currentGridPosition;
    private Vector2Int targetGridPosition;

    public int[,] gridMap;

    private bool isPathUpdate = false;
    [SerializeField]
    private GameObject currentMonsterTarget;
    private List<GameObject> detectedMonsters = new List<GameObject>();

    AttackRangeDetector attackRangeDetector;

    private void Start()
    {
        gridMap = MapManager.gridMap;
        currentGridPosition = WorldToGrid(transform.position);

        SoldierManager.Instance.RegisterNewSoldier(this);
        HealthBarManager.Instance.CreateHealthBar(this.gameObject);

        Health health = gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.SetHealth(maxHealth);
        }

        attackRangeDetector = GetComponentInChildren<AttackRangeDetector>();
        if (attackRangeDetector != null)
        {
            attackRangeDetector.OnMonsterDetected += HandleMonsterDetected;
            attackRangeDetector.OnMonsterExited += HandleMonsterExit;
        }
    }

    private void Update()
    {
        if (attackRangeDetector.SoldierAttackList != null)
        {
            detectedMonsters = attackRangeDetector.SoldierAttackList;
        }

        switch (currentState)
        {
            case SoldierState.Idle:
                HandleIdleState();
                break;
            case SoldierState.Moving:
                HandleMovingState();
                break;
            case SoldierState.Attacking:
                HandleAttackingState();
                break;
            case SoldierState.Dead:
                HandleDeadState();
                break;
        }


    }

    private void HandleIdleState()
    {
        if (detectedMonsters.Count > 0)
        {
            SwitchTarget();
        }
    }

    private void HandleMovingState()
    {
        if (target == null || gridMap == null)
        {
            currentState = SoldierState.Idle;
            return;
        }

        if (isPathUpdate)
        {
            UpdatePath();
            isPathUpdate = false;
        }

        if (WorldToGrid(target.transform.position) != currentGridPosition)
        {
            isPathUpdate = true;
        }

        if (path.Count > 0 && currentPathIndex < path.Count)
        {
            Vector3 targetPosition = GridToWorld(path[currentPathIndex].Position);
            targetPosition.y = 1.45f;

            MoveTowards(targetPosition);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
                currentGridPosition = WorldToGrid(transform.position);

                if (currentPathIndex >= path.Count)
                {
                    currentState = detectedMonsters.Count > 0 ? SoldierState.Attacking : SoldierState.Idle;
                }
            }
        }
    }

    private void UpdatePath()
    {
        targetGridPosition = WorldToGrid(target.position);
        path = LPAStar.FindPath(gridMap, currentGridPosition, targetGridPosition);
        currentPathIndex = 1;

        if (path.Count == 0)
        {
            currentState = SoldierState.Idle;
        }
    }

    private void HandleAttackingState()
    {

        if (currentMonsterTarget != null && detectedMonsters.First() != null)
        {
            currentMonsterTarget = detectedMonsters.First();
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                AttackTarget(currentMonsterTarget);
            }
        }
        else
        {

            SwitchTarget();
        }
    }

    private void AttackTarget(GameObject monster)
    {
        if (monster != null)
        {
            monster.GetComponent<Health>()?.TakeDamage(attackDamage);
        }
    }

    private void SwitchTarget()
    {
        if(detectedMonsters.First() == null)
        {
            detectedMonsters.RemoveAt(0);
        }
        if (detectedMonsters.Count > 0)
        {
            currentMonsterTarget = detectedMonsters.First();
            currentState = SoldierState.Attacking;
        }
        else
        {
            currentState = SoldierState.Idle;
        }
    }

    private void HandleDeadState()
    {
        SoldierManager.Instance.UnregisterSoldier(this);
        HealthBarManager.Instance.RemoveHealthBar(this.gameObject);
        Destroy(gameObject);
    }

    public void HandleMonsterDetected(GameObject monster)
    {
        if (!detectedMonsters.Contains(monster))
        {
            Debug.Log("Soldier Get "+ monster);
            detectedMonsters.Add(monster);
            SwitchTarget();
        }
    }

    public void HandleMonsterExit(GameObject monster)
    {
        detectedMonsters.Remove(monster);

        if (monster == currentMonsterTarget)
        {
            currentMonsterTarget = null;
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        currentState = (newTarget != null) ? SoldierState.Moving : SoldierState.Idle;
    }
}
