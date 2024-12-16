using System.Collections.Generic;
using UnityEngine;
using MapManagernamespace;
using System.Threading;

/// <summary>
/// 士兵状态枚举，表示士兵当前的状态：待机、移动、攻击。
/// </summary>
public enum SoldierState
{
    Idle,      // 待机状态
    Moving,    // 移动状态
    Attacking,  // 攻击状态
    Dead        //死亡状态
}

/// <summary>
/// 士兵行为逻辑类，使用 LPA* 算法进行寻路。
/// </summary>
public class Soldier : MonoBehaviour//, IHealthEntity
{
    public SoldierState currentState = SoldierState.Idle; // 当前士兵状态
    public Transform target; // 当前目标，可以是位置或敌人
    public float moveSpeed = 3.0f; // 移动速度
    public float attackRange = 3.0f; // 攻击范围
    public float attackCooldown = 1.0f; // 攻击冷却时间（秒）
    public int attackDamage = 10; // 攻击造成的伤害值
    public float maxHealth = 1000f;
    public float currentHealth;
    protected Collider AttackRange;

    private float lastAttackTime = 0f; // 上次攻击时间
    private List<LPAStarNode> path = new List<LPAStarNode>(); // 当前路径
    private int currentPathIndex = 0; // 当前路径点索引
    private Vector2Int currentGridPosition; // 当前格子位置
    private Vector2Int targetGridPosition; // 目标格子位置

    // 地图数据（例如 0 表示可行区域，1 表示障碍物）
    public int[,] gridMap;

    private bool isPathUpdate = false;  // 是否需要更新路径
    private float pathUpdateInterval = 1.0f; // 路径更新间隔时间
    private float lastPathUpdateTime = 0f;   // 上次路径更新的时间

    GameObject GOmonster;
    private void Start()
    {
        gridMap = MapManager.gridMap; // 获取地图数据
        AttackRange = transform.GetComponentInChildren<Collider>();

        // 初始化士兵当前格子坐标
        currentGridPosition = WorldToGrid(transform.position);

        // 注册士兵到管理器
        SoldierManager.Instance.RegisterNewSoldier(this);
        HealthBarManager.Instance.CreateHealthBar(this.gameObject);

        Health health = gameObject.GetComponent<Health>();

        // 监听子物体触发的事件
        if (health != null)
        {
            health.healthChange += TakeDamage;
        }



        // 找到子物体上的AttackRangeDetector脚本
        AttackRangeDetector attackRangeDetector = GetComponentInChildren<AttackRangeDetector>();
        // 监听子物体触发的事件
        if (attackRangeDetector != null)
        {
            attackRangeDetector.OnMonsterDetected += HandleMonsterDetected;
        }

    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthBarManager.Instance.UpdateHealthBar(gameObject, currentHealth / maxHealth);

        if (currentHealth <= 0)
            currentState = SoldierState.Dead;
    }

    // 处理碰撞到Monster的逻辑
    public void HandleMonsterDetected(GameObject monster)
    {
        GOmonster = monster;
        // 在这里可以实现攻击或其他逻辑
        currentState = SoldierState.Attacking;
    }

    private void HendleDeadState()
    {
        // 从管理器注销士兵
        SoldierManager.Instance.UnregisterSoldier(this);
        HealthBarManager.Instance.RemoveHealthBar(this.gameObject);
    }

    private void Update()
    {

        switch (currentState)
        {
            case SoldierState.Idle:
                HandleIdleState();
                break;
            case SoldierState.Moving:
                HandleMovingState();
                break;
            case SoldierState.Attacking:
                HandleAttackingState(GOmonster);
                break;
            case SoldierState.Dead:
                HendleDeadState();
                break;
        }
    }

    private void HandleIdleState()
    {
        // 待机状态：等待目标
    }

    private void HandleMovingState()
    {
        if (target == null || gridMap == null)
        {
            currentState = SoldierState.Idle;
            return;
        }
        if (target != null && gridMap != null)
        {
            //Debug.Log(WorldToGrid(target.position));
            //Debug.Log(WorldToGrid(transform.position));
            if (WorldToGrid(target.position) != WorldToGrid(transform.position))
            {
                isPathUpdate = true;
            }
        }

        if (isPathUpdate)
        {
            UpdatePath();
            isPathUpdate = false;
        }


        // 如果路径有效，移动到当前路径点
        if (path.Count > 0)
        {
            Vector3 targetPosition = GridToWorld(path[currentPathIndex].Position);
            targetPosition.y = 1.45f;

            MoveTowards(targetPosition);

            // 如果到达当前路径点，切换到下一个路径点
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                //Debug.Log(currentPathIndex + "前往下一个点");

                currentPathIndex++;
                currentGridPosition = WorldToGrid(transform.position);

                // 如果到达终点，检查是否需要攻击
                if (currentPathIndex >= path.Count)
                {
                    if (target.CompareTag("Monster") && Vector3.Distance(transform.position, target.position) <= attackRange)
                    {
                        currentState = SoldierState.Attacking;
                    }
                    else
                    {
                        currentState = SoldierState.Idle;
                    }
                }
            }
        }

        
    }

    private void UpdatePath()
    {
            //lastPathUpdateTime = Time.time; // 更新路径更新时间
            targetGridPosition = WorldToGrid(target.position);

            // 重新计算路径
            path = LPAStar.FindPath(gridMap, currentGridPosition, targetGridPosition);
            currentPathIndex = 1;

            if (path.Count == 0)
            {
                Debug.LogWarning("无法找到路径，进入待机状态");
                currentState = SoldierState.Idle;
                return;
            }
    }
    private void HandleAttackingState(GameObject monster)
    {
        if (target != null && target.CompareTag("Monster"))
        {
            // 如果敌人超出攻击范围，重新移动
            if (Vector3.Distance(transform.position, target.position) > attackRange)
            {
                currentState = SoldierState.Moving;
                return;
            }

            // 如果攻击冷却完成，执行攻击
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                AttackTarget(monster);
            }
        }
        else
        {
            currentState = SoldierState.Idle; // 没有目标时待机
        }
    }

    private void AttackTarget(GameObject monster)
    {
        monster.GetComponent<Health>().TakeDamage(attackDamage);

        //Debug.Log($"攻击目标：{target.name}");
        //var enemyHealth = target.GetComponent<Health>();
        //if (enemyHealth != null)
        //{
        //    enemyHealth.TakeDamage(attackDamage);
        //}

    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 将世界坐标转换为网格坐标。
    /// </summary>
    /// <param name="worldPosition">世界坐标。</param>
    /// <returns>网格坐标。</returns>
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    /// <summary>
    /// 将网格坐标转换为世界坐标。
    /// </summary>
    /// <param name="gridPosition">网格坐标。</param>
    /// <returns>世界坐标。</returns>
    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }

    /// <summary>
    /// 设置目标为位置或敌人。
    /// </summary>
    /// <param name="newTarget">目标对象，可以是位置或敌人。</param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        currentState = (newTarget != null) ? SoldierState.Moving : SoldierState.Idle;
    }
}
