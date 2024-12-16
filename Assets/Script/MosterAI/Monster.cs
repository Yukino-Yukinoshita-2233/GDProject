using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    protected int health;                // 当前血量
    protected int damage;                // 攻击力
    protected float moveSpeed;           // 移动速度
    public float attackCooldown; // 攻击冷却时间（秒）

    protected Vector2Int startPos;
    protected Vector2Int targetPos;
    protected List<ThetaStarNode> path;  // 路径
    protected int currentPathIndex = 0;
    protected int[,] map;
    protected Collider AttackRange;
    public float maxHealth = 100f;
    public float currentHealth;

    Vector2Int oldCastlePosition = Vector2Int.zero;

    public enum State { Idling, Moving, Attacking, Dead }
    public State currentState;

    public void Initialize(Vector2Int startPos, Vector2Int targetPos, int[,] map)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;
        this.map = map;
        this.currentHealth = maxHealth;
        path = ThetaStar.FindPath(map, startPos, targetPos);
        currentState = State.Moving;
        oldCastlePosition = targetPos;
        //AttackRange = transform.GetComponentInChildren<Collider>();
        //Debug.Log(AttackRange);

    }

    public void UpdateMonster()
    {
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

    protected virtual void MoveTowardsTarget()
    {
        if (path == null || currentPathIndex >= path.Count)
        {
            currentState = State.Attacking;
            return;
        }

        if (targetPos != WorldToGrid(MonsterManager.castle.position))
        {
            targetPos = WorldToGrid(MonsterManager.castle.position);
            path = ThetaStar.FindPath(map, WorldToGrid(transform.position), targetPos);
            currentPathIndex = 0;
        }

        Vector3 targetPosition = new Vector3(path[currentPathIndex].Position.x, 1, path[currentPathIndex].Position.y);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            currentPathIndex++;
    }

    // 处理碰撞到Building的逻辑
    public void HandleBuildingDetected(GameObject building)
    {
        currentState = State.Attacking;
        building.GetComponent<Health>().TakeDamage(damage*2);
        // 在这里可以实现攻击或其他逻辑
    }
    // 处理碰撞到Building的逻辑
    public void HandleSoldierDetected(GameObject soldier)
    {
        soldier.GetComponent<Health>().TakeDamage(damage);
        // 在这里可以实现攻击或其他逻辑
    }

    //寻找新目标
    void FindNewTarget()
    {

    }
    void updatePath()
    {

    }

    protected virtual void Attack()
    {
        // 攻击逻辑
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthBarManager.Instance.UpdateHealthBar(gameObject, currentHealth / maxHealth);

        if (currentHealth <= 0)
            currentState = State.Dead;
    }

    protected virtual void OnDeath()
    {
        HealthBarManager.Instance.RemoveHealthBar(gameObject);
        Destroy(gameObject);
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

}
