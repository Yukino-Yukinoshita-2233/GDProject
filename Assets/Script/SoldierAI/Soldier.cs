using System.Collections.Generic;
using UnityEngine;
using MapManagernamespace;
using System.Linq;
using System.Collections;
using System.Threading;

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
    Health health;
    public Animator animator;
    //PathVisualizer pathVisualizer;  // 添加路径可视化器

    private void Start()
    {
        gridMap = MapManager.gridMap;
        currentGridPosition = WorldToGrid(transform.position);

        // 获取 PathVisualizer 组件
        //pathVisualizer = GetComponent<PathVisualizer>();
        //if (pathVisualizer == null)
        //{
        //    pathVisualizer = gameObject.AddComponent<PathVisualizer>();
        //}

        SoldierManager.Instance.RegisterNewSoldier(this);
        HealthBarManager.Instance.CreateHealthBar(this.gameObject);

        health = gameObject.GetComponent<Health>();
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
        // 如果血量低于等于 0，设置为死亡状态
        if (health.GetHealth() <= 0)
        {
            Debug.Log("士兵死亡：" + gameObject.name);
            //HealthBarManager.Instance.RemoveHealthBar(gameObject); // 移除血条
            currentState = SoldierState.Dead;
        }

        switch (currentState)
        {
            case SoldierState.Idle:
                animator.SetFloat("isRun", 0f);
                HandleIdleState();
                break;
            case SoldierState.Moving:
                animator.SetFloat("isRun", 1f);
                HandleMovingState();
                break;
            case SoldierState.Attacking:
                HandleAttackingState();
                break;
            case SoldierState.Dead:
                animator.SetBool("isDead",true);
                HandleDeadState();
                break;
        }


    } 
    void FixedUpdate()
    {
        Quaternion fixedRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        GetComponent<Rigidbody>().MoveRotation(fixedRotation);
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

        // 设置路径可视化
        SetLine(path);

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
                animator.SetTrigger("isAttack01");
                StartCoroutine(DelayAttack(0.5f)); // 1秒后执行伤害
                //AttackTarget(currentMonsterTarget);
            }
        }
        else
        {

            SwitchTarget();
        }
    }


    // 创建协程来延迟执行伤害
    private IEnumerator DelayAttack(float delayTime)
    {
        // 等待指定的时间
        yield return new WaitForSeconds(delayTime);

        // 执行伤害逻辑
        AttackTarget(currentMonsterTarget);
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
        transform.LookAt(new Vector3(targetPosition.x,1.5f,targetPosition.z));

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

    /// <summary>
    /// 可视化寻路路劲
    /// </summary>
    public void SetLine(List<LPAStarNode> path)
    {
        // 获取或添加 LineRenderer 组件
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // 设置 LineRenderer 的基本属性
        lineRenderer.positionCount = path.Count;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // 设置路径点
        List<Vector3> pathPoints = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 point = new Vector3(path[i].Position.x, 1, path[i].Position.y);
            pathPoints.Add(point);
        }

        lineRenderer.SetPositions(pathPoints.ToArray());

        // 设置颜色渐变
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;

        // 设置材质
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

}
