using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IHealthEntity
{
    // �����ͨ������
    protected int health;
    protected int damage;
    protected float moveSpeed;
    protected Vector2Int startPos;
    protected Vector2Int targetPos;
    protected List<ThetaStarNode> path;
    protected int currentPathIndex = 0; // ��ǰ·������
    protected int[,] map;

    // ����״̬ö�٣��ƶ�������������
    public enum State { FindTarger, Moving, Attacking, Dead }
    public State currentState;

    public float MaxHealth { get; private set; } = 100f;
    public float CurrentHealth { get; private set; } = 100f;

    // ��ʼ�����������ʼ��Ŀ��λ���Լ���ͼ
    public void Initialize(Vector2Int startPos, Vector2Int targetPos, int[,] map)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;
        this.map = map;
        path = ThetaStar.FindPath(map, startPos, targetPos); // ʹ��A*�㷨����·��
        currentState = State.Moving;
    }

    // ���¹���״̬�����ڵ�ǰ״ִ̬�в���
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
        //����Ѱ�ҹ���Ŀ��
    }
    
    // �ƶ���Ŀ��λ��
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
            //Debug.Log("Monster����Ŀ���,��ʼ����!!!");
            currentState = State.Attacking;
        }
    }

    // �����������鷽���ɱ�������д��
    protected virtual void Attack()
    {
        //Debug.Log($"{GetType().Name} attacking at position: " + targetPos);
    }

    // �����˺�����
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            currentState = State.Dead;
        }
    }

    // ��������
    protected virtual void OnDeath()
    {
        HealthBarManager.Instance.RemoveHealthBar(gameObject);
        Destroy(gameObject);
        Debug.Log($"{GetType().Name} has died.");
    }
}
