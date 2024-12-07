using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IHealthEntity
{
    // 怪物的通用属性
    protected int health;
    protected int damage;
    protected float moveSpeed;
    protected Vector2Int startPos;
    protected Vector2Int targetPos;
    protected List<ThetaStarNode> path;
    protected int currentPathIndex = 0; // 当前路径索引
    protected int[,] map;

    // 怪物状态枚举：移动、攻击、死亡
    public enum State { FindTarger, Moving, Attacking, Dead }
    public State currentState;

    public float MaxHealth { get; private set; } = 100f;
    public float CurrentHealth { get; private set; } = 100f;

    // 初始化怪物，传入起始和目标位置以及地图
    public void Initialize(Vector2Int startPos, Vector2Int targetPos, int[,] map)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;
        this.map = map;
        path = ThetaStar.FindPath(map, startPos, targetPos); // 使用A*算法生成路径
        currentState = State.Moving;
    }

    // 更新怪物状态，基于当前状态执行操作
    public void UpdateMonster()
    {
        switch (currentState)
        {
            case State.FindTarger:
                FindTarger();
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
    protected virtual void FindTarger()
    {
        //用来寻找攻击目标
    }
    
    // 移动到目标位置
    protected virtual void MoveTowardsTarget()
    {
        if (path == null)
        {
            currentState = State.Dead;
        }
        if (currentPathIndex >= path.Count)
        {
            currentState = State.Attacking;
            return;
        }


        Vector3 targetPosition = new Vector3(path[currentPathIndex].Position.x, 1, path[currentPathIndex].Position.y);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
        }

        if (currentPathIndex >= path.Count && Vector3.Distance(transform.position, targetPosition) < 2f)
        {
            //Debug.Log(targetPosition);
            //Debug.Log(currentPathIndex >= path.Count - 1);
            //Debug.Log(Vector3.Distance(transform.position, targetPosition) < 1f);
            //Debug.Log("Monster到达目标点,开始攻击!!!");
            currentState = State.Attacking;
        }
    }

    // 攻击方法（虚方法可被子类重写）
    protected virtual void Attack()
    {
        //Debug.Log($"{GetType().Name} attacking at position: " + targetPos);
    }

    // 承受伤害方法
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            currentState = State.Dead;
        }
    }

    // 死亡方法
    protected virtual void OnDeath()
    {
        HealthBarManager.Instance.RemoveHealthBar(gameObject);
        Destroy(gameObject);
        Debug.Log($"{GetType().Name} has died.");
    }
}
