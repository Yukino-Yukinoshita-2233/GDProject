using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    // �����ͨ������
    protected int health;
    protected int damage;
    protected float moveSpeed;
    protected Vector2Int startPos;
    protected Vector2Int targetPos;
    protected List<Node> path;
    protected int currentPathIndex = 0; // ��ǰ·������
    protected int[,] map;

    // ����״̬ö�٣��ƶ�������������
    public enum State { Moving, Attacking, Dead }
    public State currentState;

    // ��ʼ�����������ʼ��Ŀ��λ���Լ���ͼ
    public void Initialize(Vector2Int startPos, Vector2Int targetPos, int[,] map)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;
        this.map = map;
        path = AStarPathfinding.FindPath(map, startPos, targetPos); // ʹ��A*�㷨����·��
        currentState = State.Moving;
    }

    // ���¹���״̬�����ڵ�ǰ״ִ̬�в���
    public void UpdateMonster()
    {
        switch (currentState)
        {
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

    // �ƶ���Ŀ��λ��
    protected virtual void MoveTowardsTarget()
    {
        if (path == null || currentPathIndex >= path.Count)
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

        if (currentPathIndex >= path.Count - 1 && Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            currentState = State.Attacking;
        }
    }

    // �����������鷽���ɱ�������д��
    protected virtual void Attack()
    {
        Debug.Log($"{GetType().Name} attacking at position: " + targetPos);
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
        Destroy(gameObject);
        Debug.Log($"{GetType().Name} has died.");
    }
}