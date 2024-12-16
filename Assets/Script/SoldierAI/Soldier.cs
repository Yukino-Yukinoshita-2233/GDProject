using System.Collections.Generic;
using UnityEngine;
using MapManagernamespace;
using System.Threading;

/// <summary>
/// ʿ��״̬ö�٣���ʾʿ����ǰ��״̬���������ƶ���������
/// </summary>
public enum SoldierState
{
    Idle,      // ����״̬
    Moving,    // �ƶ�״̬
    Attacking,  // ����״̬
    Dead        //����״̬
}

/// <summary>
/// ʿ����Ϊ�߼��࣬ʹ�� LPA* �㷨����Ѱ·��
/// </summary>
public class Soldier : MonoBehaviour//, IHealthEntity
{
    public SoldierState currentState = SoldierState.Idle; // ��ǰʿ��״̬
    public Transform target; // ��ǰĿ�꣬������λ�û����
    public float moveSpeed = 3.0f; // �ƶ��ٶ�
    public float attackRange = 3.0f; // ������Χ
    public float attackCooldown = 1.0f; // ������ȴʱ�䣨�룩
    public int attackDamage = 10; // ������ɵ��˺�ֵ
    public float maxHealth = 1000f;
    public float currentHealth;
    protected Collider AttackRange;

    private float lastAttackTime = 0f; // �ϴι���ʱ��
    private List<LPAStarNode> path = new List<LPAStarNode>(); // ��ǰ·��
    private int currentPathIndex = 0; // ��ǰ·��������
    private Vector2Int currentGridPosition; // ��ǰ����λ��
    private Vector2Int targetGridPosition; // Ŀ�����λ��

    // ��ͼ���ݣ����� 0 ��ʾ��������1 ��ʾ�ϰ��
    public int[,] gridMap;

    private bool isPathUpdate = false;  // �Ƿ���Ҫ����·��
    private float pathUpdateInterval = 1.0f; // ·�����¼��ʱ��
    private float lastPathUpdateTime = 0f;   // �ϴ�·�����µ�ʱ��

    GameObject GOmonster;
    private void Start()
    {
        gridMap = MapManager.gridMap; // ��ȡ��ͼ����
        AttackRange = transform.GetComponentInChildren<Collider>();

        // ��ʼ��ʿ����ǰ��������
        currentGridPosition = WorldToGrid(transform.position);

        // ע��ʿ����������
        SoldierManager.Instance.RegisterNewSoldier(this);
        HealthBarManager.Instance.CreateHealthBar(this.gameObject);

        Health health = gameObject.GetComponent<Health>();

        // ���������崥�����¼�
        if (health != null)
        {
            health.healthChange += TakeDamage;
        }



        // �ҵ��������ϵ�AttackRangeDetector�ű�
        AttackRangeDetector attackRangeDetector = GetComponentInChildren<AttackRangeDetector>();
        // ���������崥�����¼�
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

    // ������ײ��Monster���߼�
    public void HandleMonsterDetected(GameObject monster)
    {
        GOmonster = monster;
        // ���������ʵ�ֹ����������߼�
        currentState = SoldierState.Attacking;
    }

    private void HendleDeadState()
    {
        // �ӹ�����ע��ʿ��
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
        // ����״̬���ȴ�Ŀ��
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


        // ���·����Ч���ƶ�����ǰ·����
        if (path.Count > 0)
        {
            Vector3 targetPosition = GridToWorld(path[currentPathIndex].Position);
            targetPosition.y = 1.45f;

            MoveTowards(targetPosition);

            // ������ﵱǰ·���㣬�л�����һ��·����
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                //Debug.Log(currentPathIndex + "ǰ����һ����");

                currentPathIndex++;
                currentGridPosition = WorldToGrid(transform.position);

                // ��������յ㣬����Ƿ���Ҫ����
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
            //lastPathUpdateTime = Time.time; // ����·������ʱ��
            targetGridPosition = WorldToGrid(target.position);

            // ���¼���·��
            path = LPAStar.FindPath(gridMap, currentGridPosition, targetGridPosition);
            currentPathIndex = 1;

            if (path.Count == 0)
            {
                Debug.LogWarning("�޷��ҵ�·�����������״̬");
                currentState = SoldierState.Idle;
                return;
            }
    }
    private void HandleAttackingState(GameObject monster)
    {
        if (target != null && target.CompareTag("Monster"))
        {
            // ������˳���������Χ�������ƶ�
            if (Vector3.Distance(transform.position, target.position) > attackRange)
            {
                currentState = SoldierState.Moving;
                return;
            }

            // ���������ȴ��ɣ�ִ�й���
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                AttackTarget(monster);
            }
        }
        else
        {
            currentState = SoldierState.Idle; // û��Ŀ��ʱ����
        }
    }

    private void AttackTarget(GameObject monster)
    {
        monster.GetComponent<Health>().TakeDamage(attackDamage);

        //Debug.Log($"����Ŀ�꣺{target.name}");
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

    /// <summary>
    /// ����Ŀ��Ϊλ�û���ˡ�
    /// </summary>
    /// <param name="newTarget">Ŀ����󣬿�����λ�û���ˡ�</param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        currentState = (newTarget != null) ? SoldierState.Moving : SoldierState.Idle;
    }
}
