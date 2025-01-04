using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ������࣬�������Ļ������Ժ���Ϊ
/// </summary>
public abstract class Monster : MonoBehaviour
{
    // ��������
    public float maxHealth = 100f;         // ���Ѫ��
    public float currentHealth;            // ��ǰѪ��
    public float damage;                // ������
    public float moveSpeed;             // �ƶ��ٶ�
    public float attackCooldown;           // ������ȴʱ�䣨�룩
    protected float lastAttackTime;        // ��һ�ι�����ʱ��

    // ·�����
    protected Vector2Int startPos;         // ��ʼλ��
    protected Vector2Int targetPos;        // Ŀ��λ�ã�ͨ���ǳǱ���
    protected List<ThetaStarNode> path;    // ����·�������·���б�
    protected int currentPathIndex = 0;    // ��ǰ·���������
    protected int[,] map;                  // ��ͼ����

    // �������
    protected AttackRangeDetector attackRangeDetector; // ��⹥����Χ�����
    protected Health thisMonsterHealth;               // Ѫ���������

    // ��ǰ����Ĺ���Ŀ��
    protected List<GameObject> currentTargets = new List<GameObject>();

    // ·�����¼������
    Vector2Int oldCastlePosition = Vector2Int.zero;    // �ϴγǱ���λ�ã������ж��Ƿ���Ҫ���¼���·��

    // �����״̬ö��
    public enum State { Idling, Moving, Attacking, Dead }
    public State currentState;  // ��ǰ״̬

    /// <summary>
    /// ��ʼ������
    /// </summary>
    public void Initialize(Vector2Int startPos, Vector2Int targetPos, int[,] map)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;
        this.map = map;
        this.currentHealth = maxHealth; // ��ʼ����ǰѪ��
        path = ThetaStar.FindPath(map, startPos, targetPos); // ʹ�� Theta* �㷨����·��
        oldCastlePosition = targetPos; // ��¼Ŀ��λ��
        currentState = State.Moving;   // ��ʼ״̬Ϊ�ƶ�
        thisMonsterHealth = transform.GetComponent<Health>(); // ��ȡѪ���������
    }

    /// <summary>
    /// ���¹����״̬���� MonsterManager ����
    /// </summary>
    public void UpdateMonster()
    {
        // ���Ѫ�����ڵ��� 0������Ϊ����״̬
        if (thisMonsterHealth.GetHealth() <= 0)
        {
            Debug.Log("����������" + gameObject.name);
            HealthBarManager.Instance.RemoveHealthBar(gameObject); // �Ƴ�Ѫ��
            currentState = State.Dead;
        }

        // ���¹�����Χ�ڵ�Ŀ��
        if (attackRangeDetector != null)
        {
            currentTargets = attackRangeDetector.MonsterAttackList;
            currentTargets.RemoveAll(target => target == null); // �Ƴ������ٵ�Ŀ��
        }

        // ���ݵ�ǰ״ִ̬�ж�Ӧ�߼�
        switch (currentState)
        {
            case State.Idling:
                FindNewTarget(); // Ѱ����Ŀ�꣨�շ�����������ʵ�֣�
                break;
            case State.Moving:
                MoveTowardsTarget(); // ִ���ƶ��߼�
                break;
            case State.Attacking:
                Attack(); // ִ�й����߼�
                break;
            case State.Dead:
                OnDeath(); // ִ�������߼�
                break;
        }
    }

    /// <summary>
    /// �ƶ���Ŀ��λ��
    /// </summary>
    protected virtual void MoveTowardsTarget()
    {
        // ����Ǳ�λ�÷����仯�����¼���·��
        Vector2Int currentCastlePosition = WorldToGrid(MonsterManager.castle.position);
        if (targetPos != currentCastlePosition)
        {
            targetPos = currentCastlePosition;
            path = ThetaStar.FindPath(map, WorldToGrid(transform.position), targetPos);
            currentPathIndex = 0; // ����·��������
        }

        // �����ǰ�е���Ŀ�꣬�л�������״̬
        if (currentTargets.Count > 0)
        {
            currentState = State.Attacking;
            return;
        }

        // ���·��Ϊ�ջ��Ѿ�����·���յ㣬�л�������״̬
        if (path == null || currentPathIndex >= path.Count)
        {
            currentState = State.Attacking;
            return;
        }




        // �ƶ�����ǰ·����
        Vector3 targetPosition = new Vector3(path[currentPathIndex].Position.x, 1, path[currentPathIndex].Position.y);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        // ����ӽ���ǰ·���㣬�ƶ�����һ��·����
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
        }
    }

    /// <summary>
    /// ����Ŀ��
    /// </summary>
    protected virtual void Attack()
    {
        // �����ȴʱ��δ������û��Ŀ�꣬�л����ƶ�״̬
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


        // ��ȡ��һ��Ŀ�겢ִ�й���
        GameObject target = currentTargets.FirstOrDefault();
        if (target != null)
        {
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage); // ��Ŀ������˺�
                Debug.Log("���﹥�����˺���" + damage);
            }
        }

        lastAttackTime = Time.time; // ������󹥻�ʱ��
    }

    /// <summary>
    /// ��������������߼�
    /// </summary>
    protected virtual void OnDeath()
    {
        Destroy(gameObject); // ���ٹ������
    }

    /// <summary>
    /// ����������ת��Ϊ��������
    /// </summary>
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    /// <summary>
    /// ����������ת��Ϊ��������
    /// </summary>
    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }

    // ���󷽷��Ϳ���д������������ʵ�ֻ���չ�߼���
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
