using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    protected float health;                // 当前血量
    protected float damage;                // 攻击力
    protected float moveSpeed;           // 移动速度
    public float attackCooldown;         // 攻击冷却时间（秒）

    protected Vector2Int startPos;       // 起始位置
    protected Vector2Int targetPos;      // 目标位置（通常是城堡）
    protected List<ThetaStarNode> path;  // 用于路径计算的路径列表
    protected int currentPathIndex = 0;  // 当前路径点的索引
    protected int[,] map;               // 地图数据
    public float maxHealth = 100f;       // 最大血量
    public float currentHealth;          // 当前血量
    protected float lastAttackTime;  // 上一次攻击的时间

    protected AttackRangeDetector attackRangeDetector;

    // 用于存储当前怪物的攻击目标（建筑物和士兵）
    protected List<GameObject> currentTargets = new List<GameObject>();

    Vector2Int oldCastlePosition = Vector2Int.zero;  // 上次城堡的位置，用于判断是否需要重新计算路径

    public enum State { Idling, Moving, Attacking, Dead }
    public State currentState;  // 当前状态

    // 初始化怪物
    public void Initialize(Vector2Int startPos, Vector2Int targetPos, int[,] map)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;
        this.map = map;
        this.currentHealth = maxHealth;
        path = ThetaStar.FindPath(map, startPos, targetPos);  // 通过 A* 算法计算路径
        currentState = State.Moving;
        oldCastlePosition = targetPos;

    }

    // 更新怪物的状态
    public void UpdateMonster()
    {
        if(attackRangeDetector != null)
        {
            currentTargets = attackRangeDetector.MonsterAttackList;
        }
        switch (currentState)
        {
            case State.Idling:
                FindNewTarget();
                break;
            case State.Moving:
                MoveTowardsTarget();
                break;
            case State.Attacking:
                Attack();
                break;
            case State.Dead:
                OnDeath();
                break;
        }
    }

    // 移动到目标位置
    protected virtual void MoveTowardsTarget()
    {
        if (path == null || currentPathIndex >= path.Count)
        {
            Debug.Log("玩路径");

            currentState = State.Attacking;  // 如果没有路径或路径已走完，进入攻击状态
            return;
        }

        // 如果城堡位置发生变化，重新计算路径
        if (targetPos != WorldToGrid(MonsterManager.castle.position))
        {
            targetPos = WorldToGrid(MonsterManager.castle.position);
            path = ThetaStar.FindPath(map, WorldToGrid(transform.position), targetPos);
            currentPathIndex = 0;
        }

        if (currentTargets.Count != 0)
        {
            currentState = State.Attacking;  // 如果有敌人，进入攻击状态
            return;

        }

        Vector3 targetPosition = new Vector3(path[currentPathIndex].Position.x, 1, path[currentPathIndex].Position.y);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            currentPathIndex++;  // 到达当前目标点，移动到下一个路径点


    }

    // 处理建筑物被检测到的逻辑
    protected virtual void HandleBuildingDetected(GameObject building)
    {
        // 基类可以为空，子类可以重写此方法来处理建筑物检测到后的行为
    }

    // 处理士兵被检测到的逻辑
    protected virtual void HandleSoldierDetected(GameObject soldier)
    {
        // 基类可以为空，子类可以重写此方法来处理士兵检测到后的行为
    }

    // 寻找新的目标（这里只是一个空的示例方法，可以在子类中实现具体的目标选择逻辑）
    protected virtual void FindNewTarget()
    {
        // 基类可以为空，子类可以重写此方法来实现目标选择的逻辑
    }

    protected virtual void HandleBuildingExit(GameObject building)
    {

    }

    protected virtual void HandleSoldierExit(GameObject soldier)
    {

    }

    // 攻击逻辑（攻击当前目标）
    protected virtual void Attack()
    {
        ////子类覆写
        // 检查是否符合攻击冷却条件
        if (Time.time - lastAttackTime >= attackCooldown && currentTargets.Count != 0)
        {
            GameObject target = currentTargets.First();
            if (target != null)
            {
                Health targetHealth = target.GetComponent<Health>();
                if (targetHealth != null)
                {
                    Debug.Log("RockDragon,Damage:" + damage);
                    targetHealth.TakeDamage(damage);  // 对每个目标造成伤害
                }
            }

            lastAttackTime = Time.time;  // 更新最后攻击的时间
        }
        else
        {
            currentState = State.Moving;
        }
    }

    // 处理怪物死亡的逻辑
    protected virtual void OnDeath()
    {
        HealthBarManager.Instance.RemoveHealthBar(gameObject);
        Destroy(gameObject);  // 销毁怪物对象
    }

    // 将世界坐标转换为网格坐标
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    // 将网格坐标转换为世界坐标
    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }
}
