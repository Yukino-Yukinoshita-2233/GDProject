using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 怪物基类，定义怪物的基础属性和行为
/// </summary>
public abstract class Monster : MonoBehaviour
{
    // 基本属性
    public float maxHealth = 100f;         // 最大血量
    public float currentHealth;            // 当前血量
    public float damage;                // 攻击力
    public float moveSpeed;             // 移动速度
    public float attackCooldown;           // 攻击冷却时间（秒）
    protected float lastAttackTime;        // 上一次攻击的时间

    // 路径相关
    protected Vector2Int startPos;         // 起始位置
    protected Vector2Int targetPos;        // 目标位置（通常是城堡）
    protected List<ThetaStarNode> path;    // 用于路径计算的路径列表
    protected int currentPathIndex = 0;    // 当前路径点的索引
    protected int[,] map;                  // 地图数据

    // 组件引用
    protected AttackRangeDetector attackRangeDetector; // 检测攻击范围的组件
    protected Health thisMonsterHealth;               // 血量管理组件

    // 当前怪物的攻击目标
    protected List<GameObject> currentTargets = new List<GameObject>();

    // 路径重新计算相关
    Vector2Int oldCastlePosition = Vector2Int.zero;    // 上次城堡的位置，用于判断是否需要重新计算路径

    // 怪物的状态枚举
    public enum State { Idling, Moving, Attacking, Dead }
    public State currentState;  // 当前状态

    /// <summary>
    /// 初始化怪物
    /// </summary>
    public void Initialize(Vector2Int startPos, Vector2Int targetPos, int[,] map)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;
        this.map = map;
        this.currentHealth = maxHealth; // 初始化当前血量
        path = ThetaStar.FindPath(map, startPos, targetPos); // 使用 Theta* 算法计算路径
        oldCastlePosition = targetPos; // 记录目标位置
        currentState = State.Moving;   // 初始状态为移动
        thisMonsterHealth = transform.GetComponent<Health>(); // 获取血量管理组件
    }

    /// <summary>
    /// 更新怪物的状态，由 MonsterManager 调用
    /// </summary>
    public void UpdateMonster()
    {
        // 如果血量低于等于 0，设置为死亡状态
        if (thisMonsterHealth.GetHealth() <= 0)
        {
            Debug.Log("怪物死亡：" + gameObject.name);
            HealthBarManager.Instance.RemoveHealthBar(gameObject); // 移除血条
            currentState = State.Dead;
        }

        // 更新攻击范围内的目标
        if (attackRangeDetector != null)
        {
            currentTargets = attackRangeDetector.MonsterAttackList;
            currentTargets.RemoveAll(target => target == null); // 移除已销毁的目标
        }

        // 根据当前状态执行对应逻辑
        switch (currentState)
        {
            case State.Idling:
                FindNewTarget(); // 寻找新目标（空方法，供子类实现）
                break;
            case State.Moving:
                MoveTowardsTarget(); // 执行移动逻辑
                break;
            case State.Attacking:
                Attack(); // 执行攻击逻辑
                break;
            case State.Dead:
                OnDeath(); // 执行死亡逻辑
                break;
        }
    }

    /// <summary>
    /// 移动到目标位置
    /// </summary>
    protected virtual void MoveTowardsTarget()
    {
        // 如果城堡位置发生变化，重新计算路径
        Vector2Int currentCastlePosition = WorldToGrid(MonsterManager.castle.position);
        if (targetPos != currentCastlePosition)
        {
            targetPos = currentCastlePosition;
            path = ThetaStar.FindPath(map, WorldToGrid(transform.position), targetPos);
            currentPathIndex = 0; // 重置路径点索引
        }

        // 如果当前有敌人目标，切换到攻击状态
        if (currentTargets.Count > 0)
        {
            currentState = State.Attacking;
            return;
        }

        // 如果路径为空或已经到达路径终点，切换到攻击状态
        if (path == null || currentPathIndex >= path.Count)
        {
            currentState = State.Attacking;
            return;
        }




        // 移动到当前路径点
        Vector3 targetPosition = new Vector3(path[currentPathIndex].Position.x, 1, path[currentPathIndex].Position.y);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        // 如果接近当前路径点，移动到下一个路径点
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
        }
    }

    /// <summary>
    /// 攻击目标
    /// </summary>
    protected virtual void Attack()
    {
        // 如果冷却时间未到或者没有目标，切换回移动状态
        if (currentTargets.Count == 0)
        {
            currentState = State.Moving;
            return;
        }
        else if (Time.time - lastAttackTime < attackCooldown && currentTargets.Count != 0)
        {
            currentState = State.Idling;
            return;
        }


        // 获取第一个目标并执行攻击
        GameObject target = currentTargets.FirstOrDefault();
        if (target != null)
        {
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage); // 对目标造成伤害
                Debug.Log("怪物攻击，伤害：" + damage);
            }
        }

        lastAttackTime = Time.time; // 更新最后攻击时间
    }

    /// <summary>
    /// 处理怪物死亡的逻辑
    /// </summary>
    protected virtual void OnDeath()
    {
        Destroy(gameObject); // 销毁怪物对象
    }

    /// <summary>
    /// 将世界坐标转换为网格坐标
    /// </summary>
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    /// <summary>
    /// 将网格坐标转换为世界坐标
    /// </summary>
    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }

    // 抽象方法和可重写方法（供子类实现或扩展逻辑）
    protected virtual void FindNewTarget() 
    { 
        if(currentTargets.Count != 0)
        {
            currentState = State.Attacking;
        }
        else 
        {
            currentState = State.Idling;
        }
    }
    protected virtual void HandleBuildingDetected(GameObject building) { }
    protected virtual void HandleSoldierDetected(GameObject soldier) { }
    protected virtual void HandleBuildingExit(GameObject building) { }
    protected virtual void HandleSoldierExit(GameObject soldier) { }
}
