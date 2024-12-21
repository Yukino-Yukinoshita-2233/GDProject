using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    protected float health;                // ��ǰѪ��
    protected float damage;                // ������
    protected float moveSpeed;           // �ƶ��ٶ�
    public float attackCooldown;         // ������ȴʱ�䣨�룩

    protected Vector2Int startPos;       // ��ʼλ��
    protected Vector2Int targetPos;      // Ŀ��λ�ã�ͨ���ǳǱ���
    protected List<ThetaStarNode> path;  // ����·�������·���б�
    protected int currentPathIndex = 0;  // ��ǰ·���������
    protected int[,] map;               // ��ͼ����
    public float maxHealth = 100f;       // ���Ѫ��
    public float currentHealth;          // ��ǰѪ��
    protected float lastAttackTime;  // ��һ�ι�����ʱ��

    protected AttackRangeDetector attackRangeDetector;

    // ���ڴ洢��ǰ����Ĺ���Ŀ�꣨�������ʿ����
    protected List<GameObject> currentTargets = new List<GameObject>();

    Vector2Int oldCastlePosition = Vector2Int.zero;  // �ϴγǱ���λ�ã������ж��Ƿ���Ҫ���¼���·��

    public enum State { Idling, Moving, Attacking, Dead }
    public State currentState;  // ��ǰ״̬

    // ��ʼ������
    public void Initialize(Vector2Int startPos, Vector2Int targetPos, int[,] map)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;
        this.map = map;
        this.currentHealth = maxHealth;
        path = ThetaStar.FindPath(map, startPos, targetPos);  // ͨ�� A* �㷨����·��
        currentState = State.Moving;
        oldCastlePosition = targetPos;

    }

    // ���¹����״̬
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

    // �ƶ���Ŀ��λ��
    protected virtual void MoveTowardsTarget()
    {
        if (path == null || currentPathIndex >= path.Count)
        {
            Debug.Log("��·��");

            currentState = State.Attacking;  // ���û��·����·�������꣬���빥��״̬
            return;
        }

        // ����Ǳ�λ�÷����仯�����¼���·��
        if (targetPos != WorldToGrid(MonsterManager.castle.position))
        {
            targetPos = WorldToGrid(MonsterManager.castle.position);
            path = ThetaStar.FindPath(map, WorldToGrid(transform.position), targetPos);
            currentPathIndex = 0;
        }

        if (currentTargets.Count != 0)
        {
            currentState = State.Attacking;  // ����е��ˣ����빥��״̬
            return;

        }

        Vector3 targetPosition = new Vector3(path[currentPathIndex].Position.x, 1, path[currentPathIndex].Position.y);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            currentPathIndex++;  // ���ﵱǰĿ��㣬�ƶ�����һ��·����


    }

    // �������ﱻ��⵽���߼�
    protected virtual void HandleBuildingDetected(GameObject building)
    {
        // �������Ϊ�գ����������д�˷��������������⵽�����Ϊ
    }

    // ����ʿ������⵽���߼�
    protected virtual void HandleSoldierDetected(GameObject soldier)
    {
        // �������Ϊ�գ����������д�˷���������ʿ����⵽�����Ϊ
    }

    // Ѱ���µ�Ŀ�꣨����ֻ��һ���յ�ʾ��������������������ʵ�־����Ŀ��ѡ���߼���
    protected virtual void FindNewTarget()
    {
        // �������Ϊ�գ����������д�˷�����ʵ��Ŀ��ѡ����߼�
    }

    protected virtual void HandleBuildingExit(GameObject building)
    {

    }

    protected virtual void HandleSoldierExit(GameObject soldier)
    {

    }

    // �����߼���������ǰĿ�꣩
    protected virtual void Attack()
    {
        ////���าд
        // ����Ƿ���Ϲ�����ȴ����
        if (Time.time - lastAttackTime >= attackCooldown && currentTargets.Count != 0)
        {
            GameObject target = currentTargets.First();
            if (target != null)
            {
                Health targetHealth = target.GetComponent<Health>();
                if (targetHealth != null)
                {
                    Debug.Log("RockDragon,Damage:" + damage);
                    targetHealth.TakeDamage(damage);  // ��ÿ��Ŀ������˺�
                }
            }

            lastAttackTime = Time.time;  // ������󹥻���ʱ��
        }
        else
        {
            currentState = State.Moving;
        }
    }

    // ��������������߼�
    protected virtual void OnDeath()
    {
        HealthBarManager.Instance.RemoveHealthBar(gameObject);
        Destroy(gameObject);  // ���ٹ������
    }

    // ����������ת��Ϊ��������
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    // ����������ת��Ϊ��������
    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }
}
