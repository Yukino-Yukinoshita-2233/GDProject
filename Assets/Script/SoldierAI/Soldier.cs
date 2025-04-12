using System.Collections.Generic;
using UnityEngine;
using MapManagernamespace;
using System.Linq;
using System.Collections;
using UnityEngine.Animations;
using System.Threading;
using TMPro;

public enum SoldierState
{
    Jingjie,
    Xunluo
}

public enum SoldierBaseState
{
    Idle,
    Moving,
    Attacking,
    Dead
}

public class Soldier : MonoBehaviour
{
    public SoldierState currentState = SoldierState.Jingjie;
    public SoldierBaseState currentBaseState = SoldierBaseState.Idle;
    public float moveSpeed = 3.0f;             // 移动速度
    public float attackCooldown = 1.0f;        // 攻击冷却时间
    public float attackDamage = 10;            // 攻击伤害
    public float maxHealth = 1000f;            // 最大生命值

    protected float lastAttackTime = 0f;

    public Vector3 target;                   // 寻路目标点（世界坐标）
    protected Vector2Int currentGridPosition; // 当前所在格子位置
    protected Vector2Int targetGridPosition;  // 目标点所在的格子位置
    protected float lastUpdatePathTime = 0f;
    protected List<ThetaStarNode> path = new List<ThetaStarNode>();  // 当前计算出的路径
    protected int currentPathIndex = 0;       // 当前路径点索引

    public int[,] gridMap;                    // 地图数据

    protected bool isPathUpdate = false;      // 标记是否需要重新计算路径

    [SerializeField]
    protected GameObject currentMonsterTarget;
    protected List<GameObject> detectedMonsters = new List<GameObject>();

    protected AttackRangeDetector attackRangeDetector;
    protected Health health;
    public Animator animator;

    protected virtual void Start()
    {
        gridMap = MapManager.gridMap;
        // 将当前单位位置转换为网格坐标
        currentGridPosition = WorldToGrid(transform.position);
        // 初始目标为自身位置（静止）
        target = transform.position;
        targetGridPosition = WorldToGrid(target);

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

    protected virtual void Update()
    {
        // 更新检测到的怪物列表
        if (attackRangeDetector.SoldierAttackList != null)
        {
            detectedMonsters = attackRangeDetector.SoldierAttackList;
        }

        // 当血量低于等于0时，切换为死亡状态
        if (health.GetHealth() <= 0)
        {
            Debug.Log("士兵死亡：" + gameObject.name);
            currentBaseState = SoldierBaseState.Dead;
        }
        StateHandling(); // 状态处理

    }

    void FixedUpdate()
    {
        // 锁定旋转，只允许在Y轴旋转
        Quaternion fixedRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        GetComponent<Rigidbody>().MoveRotation(fixedRotation);
    }


    public void StateHandling()
    {
        // 根据不同状态分别处理
        if (currentState == SoldierState.Jingjie)
        {
            switch (currentBaseState)
            {
                case SoldierBaseState.Idle:
                    animator.SetFloat("isRun", 0f);
                    JingJieIdleState();
                    break;
                case SoldierBaseState.Moving:
                    animator.SetFloat("isRun", 1f);
                    MovingState();
                    break;
                case SoldierBaseState.Attacking:
                    animator.SetFloat("isRun", 0f);

                    AttackingState();
                    break;
                case SoldierBaseState.Dead:
                    animator.SetFloat("isRun", 0f);

                    animator.SetBool("isDead", true);
                    DeadState();
                    break;
            }
        }
        else if (currentState == SoldierState.Xunluo)
        {
            switch (currentBaseState)
            {
                case SoldierBaseState.Idle:
                    animator.SetFloat("isRun", 0f);
                    XunLuoIdleState();
                    break;
                case SoldierBaseState.Moving:
                    animator.SetFloat("isRun", 1f);
                    MovingState();
                    break;
                case SoldierBaseState.Attacking:
                    animator.SetFloat("isRun", 0f);

                    AttackingState();
                    break;
                case SoldierBaseState.Dead:
                    animator.SetFloat("isRun", 0f);

                    animator.SetBool("isDead", true);
                    DeadState();
                    break;
            }
        }

    }


    protected virtual void JingJieIdleState()
    {
        if (detectedMonsters.Count > 0)
        {
            SwitchTarget();
        }
    }

    protected virtual void XunLuoIdleState()
    {
        if (detectedMonsters.Count > 0)
        {
            SwitchTarget();
        }
        // 随机选择一个可行走的位置作为目标点（防止while死循环建议可加入最大尝试次数）
        while (true)
        {
            int X = Random.Range(0, gridMap.GetLength(0));
            int Z = Random.Range(0, gridMap.GetLength(1));
            if (gridMap[X, Z] == 0)
            {
                Vector3 targetTest = new Vector3(X, 0, Z);
                SetTarget(targetTest);
                return;
            }
        }
    }

    protected virtual void MovingState()
    {
        if (target == null || gridMap == null)
        {
            currentBaseState = SoldierBaseState.Idle;
            return;
        }

        // 如果目标格子发生变化，则标记需要更新路径
        if (WorldToGrid(target) != targetGridPosition)
        {
            isPathUpdate = true;
        }

        // 如果需要更新路径，则以当前单位位置作为新起点重新计算路径
        if (isPathUpdate)
        {
            lastUpdatePathTime = Time.time;
            UpdatePath();
            isPathUpdate = false;
        }

        // 当有有效路径且路径点索引未超出路径列表时，进行移动
        if (path.Count > 0 && currentPathIndex < path.Count)
        {
            // 取当前路径点的世界坐标
            Vector3 targetPosition = GridToWorld(path[currentPathIndex].Position);
            targetPosition.y = 1.45f; // 固定Y值，保证角色高度

            // 调用移动函数，将单位向目标点移动
            MoveTowards(targetPosition);

            // 在 MovingState 中判断是否到达当前路径点
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
                currentGridPosition = WorldToGrid(transform.position);

                // 如果已经走完所有路径点，则状态切换为攻击或闲置，并清除路径可视化
                if (currentPathIndex >= path.Count)
                {
                    currentBaseState = detectedMonsters.Count > 0 ? SoldierBaseState.Attacking : SoldierBaseState.Idle;
                    ClearPathVisualization();
                }
            }
        }
        else
        {
            // 没有路径时切换为空闲状态
            currentBaseState = SoldierBaseState.Idle;
        }
    }

    /// <summary>
    /// 更新路径计算
    /// 以当前单位位置为起点，目标格子为终点计算新路径，并更新路径可视化
    /// </summary>
    private void UpdatePath()
    {
        Debug.Log(gameObject.name + " 修改路径");
        // 更新起点为当前单位位置
        currentGridPosition = WorldToGrid(transform.position);
        targetGridPosition = WorldToGrid(target);
        path = ThetaStar.FindPath(gridMap, currentGridPosition, targetGridPosition);
        // 这里约定 ThetaStar 返回的路径第一节点为起点，因此从索引1开始移动
        currentPathIndex = (path.Count > 1) ? 1 : 0;

        // 更新路径可视化
        SetLine(path);
    }

    protected virtual void AttackingState()
    {
        if (detectedMonsters.FirstOrDefault() != null)
        {
            // 确保currentMonsterTarget被正确赋值
            currentMonsterTarget = detectedMonsters.FirstOrDefault();


            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                animator.SetTrigger("isAttack01");

                StartCoroutine(DelayAttack(0.5f)); // 延迟攻击执行
            }
        }
        else
        {
            Debug.LogWarning("未检测到敌人，切换目标");
            SwitchTarget();
        }
    }

    private IEnumerator DelayAttack(float delayTime)
    {

        yield return new WaitForSeconds(delayTime);
        AttackTarget(currentMonsterTarget);
    }

    protected void AttackTarget(GameObject monster)
    {
        if (monster != null)
        {
            monster.GetComponent<Health>()?.TakeDamage(attackDamage);
        }
    }

    /// <summary>
    /// 立即旋转面向目标，仅调整Y轴
    /// </summary>
    protected void RotateTowardsTarget(GameObject target)
    {
        if (target == null) return;

        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0; // 锁定Y轴，不让士兵倾斜

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation; // 立即旋转，不使用Slerp
        }
    }

    /// <summary>
    /// 切换目标
    /// </summary>
    protected void SwitchTarget()
    {
        if (detectedMonsters.Count == 0)
        {
            currentBaseState = SoldierBaseState.Idle;
            return;
        }
        if (detectedMonsters.First() == null)
        {
            detectedMonsters.RemoveAt(0);
        }
        if (detectedMonsters.Count > 0)
        {
            currentMonsterTarget = detectedMonsters.FirstOrDefault();
            currentBaseState = SoldierBaseState.Attacking;
        }
        else
        {
            currentBaseState = SoldierBaseState.Idle;
        }
    }

    protected virtual void DeadState()
    {
        SoldierManager.Instance.UnregisterSoldier(this);
        HealthBarManager.Instance.RemoveHealthBar(this.gameObject);
        Destroy(gameObject);
    }

    protected virtual void HandleMonsterDetected(GameObject monster)
    {
        if (!detectedMonsters.Contains(monster))
        {
            Debug.Log("Soldier Get " + monster);
            detectedMonsters.Add(monster);
            SwitchTarget();
        }
    }

    protected virtual void HandleMonsterExit(GameObject monster)
    {
        detectedMonsters.Remove(monster);
        if (monster == currentMonsterTarget)
        {
            currentMonsterTarget = null;
        }
    }

    /// <summary>
    /// 移动函数，仅负责移动单位，不处理路径索引更新（在 MovingState 中统一处理）
    /// </summary>
    private void MoveTowards(Vector3 targetPosition)
    {
        // 计算目标方向（忽略 Y 轴）
        Vector3 direction = new Vector3(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z).normalized;
        if (direction != Vector3.zero)
        {
            // 计算目标旋转角度，并平滑旋转
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        // 以固定步长移动到目标位置
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, targetPosition.z), step);
    }

    /// <summary>
    /// 将世界坐标转换为网格坐标
    /// </summary>
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    /// <summary>
    /// 将网格坐标转换为世界坐标（忽略 Y 轴）
    /// </summary>
    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }

    /// <summary>
    /// 设置新的目标点，切换状态为移动
    /// </summary>
    public virtual void SetTarget(Vector3 newTarget)
    {
        target = newTarget;
        currentBaseState = (newTarget != null) ? SoldierBaseState.Moving : SoldierBaseState.Idle;
    }

    /// <summary>
    /// 可视化路径：使用 LineRenderer 展示当前路径
    /// </summary>
    public void SetLine(List<ThetaStarNode> path)
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = path.Count;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        List<Vector3> pathPoints = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 point = new Vector3(path[i].Position.x, 1, path[i].Position.y);
            pathPoints.Add(point);
        }
        lineRenderer.SetPositions(pathPoints.ToArray());
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    /// <summary>
    /// 清除路径可视化（到达目标后调用）
    /// </summary>
    private void ClearPathVisualization()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }
}
