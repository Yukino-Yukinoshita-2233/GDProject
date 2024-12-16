using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    protected int health;                // ��ǰѪ��
    protected int damage;                // ������
    protected float moveSpeed;           // �ƶ��ٶ�
    public float attackCooldown; // ������ȴʱ�䣨�룩

    protected Vector2Int startPos;
    protected Vector2Int targetPos;
    protected List<ThetaStarNode> path;  // ·��
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

    // ������ײ��Building���߼�
    public void HandleBuildingDetected(GameObject building)
    {
        currentState = State.Attacking;
        building.GetComponent<Health>().TakeDamage(damage*2);
        // ���������ʵ�ֹ����������߼�
    }
    // ������ײ��Building���߼�
    public void HandleSoldierDetected(GameObject soldier)
    {
        soldier.GetComponent<Health>().TakeDamage(damage);
        // ���������ʵ�ֹ����������߼�
    }

    //Ѱ����Ŀ��
    void FindNewTarget()
    {

    }
    void updatePath()
    {

    }

    protected virtual void Attack()
    {
        // �����߼�
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
    /// ����������ת��Ϊ�������ꡣ
    /// </summary>
    /// <param name="worldPosition">�������ꡣ</param>
    /// <returns>�������ꡣ</returns>
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    /// <summary>
    /// ����������ת��Ϊ�������ꡣ
    /// </summary>
    /// <param name="gridPosition">�������ꡣ</param>
    /// <returns>�������ꡣ</returns>
    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }

}
