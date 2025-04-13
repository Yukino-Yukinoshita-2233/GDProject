using System.Collections.Generic;
using UnityEngine;
using MapManagernamespace;
using System.Linq;
using System.Collections;
using UnityEngine.Animations;
using System.Threading;
using TMPro;

public enum SoldierState
{
    Jingjie,
    Xunluo
}

public enum SoldierBaseState
{
    Idle,
    Moving,
    Attacking,
    Dead
}

public class Soldier : MonoBehaviour
{
    public SoldierState currentState = SoldierState.Jingjie;
    public SoldierBaseState currentBaseState = SoldierBaseState.Idle;
    public float moveSpeed = 3.0f;             // �ƶ��ٶ�
    public float attackCooldown = 1.0f;        // ������ȴʱ��
    public float attackDamage = 10;            // �����˺�
    public float maxHealth = 1000f;            // �������ֵ

    protected float lastAttackTime = 0f;

    public Vector3 target;                   // Ѱ·Ŀ��㣨�������꣩
    protected Vector2Int currentGridPosition; // ��ǰ���ڸ���λ��
    protected Vector2Int targetGridPosition;  // Ŀ������ڵĸ���λ��
    protected float lastUpdatePathTime = 0f;
    protected List<ThetaStarNode> path = new List<ThetaStarNode>();  // ��ǰ�������·��
    protected int currentPathIndex = 0;       // ��ǰ·��������

    public int[,] gridMap;                    // ��ͼ����

    protected bool isPathUpdate = false;      // ����Ƿ���Ҫ���¼���·��

    [SerializeField]
    protected GameObject currentMonsterTarget;
    protected List<GameObject> detectedMonsters = new List<GameObject>();

    protected AttackRangeDetector attackRangeDetector;
    protected Health health;
    public Animator animator;

    protected virtual void Start()
    {
        gridMap = MapManager.gridMap;
        // ����ǰ��λλ��ת��Ϊ��������
        currentGridPosition = WorldToGrid(transform.position);
        // ��ʼĿ��Ϊ����λ�ã���ֹ��
        target = transform.position;
        targetGridPosition = WorldToGrid(target);

        SoldierManager.Instance.RegisterNewSoldier(this);
        HealthBarManager.Instance.CreateHealthBar(this.gameObject);

        health = gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.SetHealth(maxHealth);
        }

        attackRangeDetector = GetComponentInChildren<AttackRangeDetector>();
        if (attackRangeDetector != null)
        {
            attackRangeDetector.OnMonsterDetected += HandleMonsterDetected;
            attackRangeDetector.OnMonsterExited += HandleMonsterExit;
        }
    }

    protected virtual void Update()
    {
        // ���¼�⵽�Ĺ����б�
        if (attackRangeDetector.SoldierAttackList != null)
        {
            detectedMonsters = attackRangeDetector.SoldierAttackList;
        }

        // ��Ѫ�����ڵ���0ʱ���л�Ϊ����״̬
        if (health.GetHealth() <= 0)
        {
            Debug.Log("ʿ��������" + gameObject.name);
            currentBaseState = SoldierBaseState.Dead;
        }
        StateHandling(); // ״̬����

    }

    void FixedUpdate()
    {
        // ������ת��ֻ������Y����ת
        Quaternion fixedRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        GetComponent<Rigidbody>().MoveRotation(fixedRotation);
    }


    public void StateHandling()
    {
        // ���ݲ�ͬ״̬�ֱ���
        if (currentState == SoldierState.Jingjie)
        {
            switch (currentBaseState)
            {
                case SoldierBaseState.Idle:
                    animator.SetFloat("isRun", 0f);
                    JingJieIdleState();
                    break;
                case SoldierBaseState.Moving:
                    animator.SetFloat("isRun", 1f);
                    MovingState();
                    break;
                case SoldierBaseState.Attacking:
                    animator.SetFloat("isRun", 0f);

                    AttackingState();
                    break;
                case SoldierBaseState.Dead:
                    animator.SetFloat("isRun", 0f);

                    animator.SetBool("isDead", true);
                    DeadState();
                    break;
            }
        }
        else if (currentState == SoldierState.Xunluo)
        {
            switch (currentBaseState)
            {
                case SoldierBaseState.Idle:
                    animator.SetFloat("isRun", 0f);
                    XunLuoIdleState();
                    break;
                case SoldierBaseState.Moving:
                    animator.SetFloat("isRun", 1f);
                    MovingState();
                    break;
                case SoldierBaseState.Attacking:
                    animator.SetFloat("isRun", 0f);

                    AttackingState();
                    break;
                case SoldierBaseState.Dead:
                    animator.SetFloat("isRun", 0f);

                    animator.SetBool("isDead", true);
                    DeadState();
                    break;
            }
        }

    }


    protected virtual void JingJieIdleState()
    {
        if (detectedMonsters.Count > 0)
        {
            SwitchTarget();
        }
    }

    protected virtual void XunLuoIdleState()
    {
        if (detectedMonsters.Count > 0)
        {
            SwitchTarget();
        }
        // ���ѡ��һ�������ߵ�λ����ΪĿ��㣨��ֹwhile��ѭ������ɼ�������Դ�����
        while (true)
        {
            int X = Random.Range(0, gridMap.GetLength(0));
            int Z = Random.Range(0, gridMap.GetLength(1));
            if (gridMap[X, Z] == 0)
            {
                Vector3 targetTest = new Vector3(X, 0, Z);
                SetTarget(targetTest);
                return;
            }
        }
    }

    protected virtual void MovingState()
    {
        if (target == null || gridMap == null)
        {
            currentBaseState = SoldierBaseState.Idle;
            return;
        }

        // ���Ŀ����ӷ����仯��������Ҫ����·��
        if (WorldToGrid(target) != targetGridPosition)
        {
            isPathUpdate = true;
        }

        // �����Ҫ����·�������Ե�ǰ��λλ����Ϊ��������¼���·��
        if (isPathUpdate)
        {
            lastUpdatePathTime = Time.time;
            UpdatePath();
            isPathUpdate = false;
        }

        // ������Ч·����·��������δ����·���б�ʱ�������ƶ�
        if (path.Count > 0 && currentPathIndex < path.Count)
        {
            // ȡ��ǰ·�������������
            Vector3 targetPosition = GridToWorld(path[currentPathIndex].Position);
            targetPosition.y = 1.45f; // �̶�Yֵ����֤��ɫ�߶�

            // �����ƶ�����������λ��Ŀ����ƶ�
            MoveTowards(targetPosition);

            // �� MovingState ���ж��Ƿ񵽴ﵱǰ·����
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
                currentGridPosition = WorldToGrid(transform.position);

                // ����Ѿ���������·���㣬��״̬�л�Ϊ���������ã������·�����ӻ�
                if (currentPathIndex >= path.Count)
                {
                    currentBaseState = detectedMonsters.Count > 0 ? SoldierBaseState.Attacking : SoldierBaseState.Idle;
                    ClearPathVisualization();
                }
            }
        }
        else
        {
            // û��·��ʱ�л�Ϊ����״̬
            currentBaseState = SoldierBaseState.Idle;
        }
    }

    /// <summary>
    /// ����·������
    /// �Ե�ǰ��λλ��Ϊ��㣬Ŀ�����Ϊ�յ������·����������·�����ӻ�
    /// </summary>
    private void UpdatePath()
    {
        Debug.Log(gameObject.name + " �޸�·��");
        // �������Ϊ��ǰ��λλ��
        currentGridPosition = WorldToGrid(transform.position);
        targetGridPosition = WorldToGrid(target);
        path = ThetaStar.FindPath(gridMap, currentGridPosition, targetGridPosition);
        // ����Լ�� ThetaStar ���ص�·����һ�ڵ�Ϊ��㣬��˴�����1��ʼ�ƶ�
        currentPathIndex = (path.Count > 1) ? 1 : 0;

        // ����·�����ӻ�
        SetLine(path);
    }

    protected virtual void AttackingState()
    {
        if (detectedMonsters.FirstOrDefault() != null)
        {
            // ȷ��currentMonsterTarget����ȷ��ֵ
            currentMonsterTarget = detectedMonsters.FirstOrDefault();


            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                animator.SetTrigger("isAttack01");

                StartCoroutine(DelayAttack(0.5f)); // �ӳٹ���ִ��
            }
        }
        else
        {
            Debug.LogWarning("δ��⵽���ˣ��л�Ŀ��");
            SwitchTarget();
        }
    }

    private IEnumerator DelayAttack(float delayTime)
    {

        yield return new WaitForSeconds(delayTime);
        AttackTarget(currentMonsterTarget);
    }

    protected void AttackTarget(GameObject monster)
    {
        if (monster != null)
        {
            monster.GetComponent<Health>()?.TakeDamage(attackDamage);
        }
    }

    /// <summary>
    /// ������ת����Ŀ�꣬������Y��
    /// </summary>
    protected void RotateTowardsTarget(GameObject target)
    {
        if (target == null) return;

        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0; // ����Y�ᣬ����ʿ����б

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation; // ������ת����ʹ��Slerp
        }
    }

    /// <summary>
    /// �л�Ŀ��
    /// </summary>
    protected void SwitchTarget()
    {
        if (detectedMonsters.Count == 0)
        {
            currentBaseState = SoldierBaseState.Idle;
            return;
        }
        if (detectedMonsters.First() == null)
        {
            detectedMonsters.RemoveAt(0);
        }
        if (detectedMonsters.Count > 0)
        {
            currentMonsterTarget = detectedMonsters.FirstOrDefault();
            currentBaseState = SoldierBaseState.Attacking;
        }
        else
        {
            currentBaseState = SoldierBaseState.Idle;
        }
    }

    protected virtual void DeadState()
    {
        SoldierManager.Instance.UnregisterSoldier(this);
        HealthBarManager.Instance.RemoveHealthBar(this.gameObject);
        Destroy(gameObject);
    }

    protected virtual void HandleMonsterDetected(GameObject monster)
    {
        if (!detectedMonsters.Contains(monster))
        {
            Debug.Log("Soldier Get " + monster);
            detectedMonsters.Add(monster);
            SwitchTarget();
        }
    }

    protected virtual void HandleMonsterExit(GameObject monster)
    {
        detectedMonsters.Remove(monster);
        if (monster == currentMonsterTarget)
        {
            currentMonsterTarget = null;
        }
    }

    /// <summary>
    /// �ƶ��������������ƶ���λ��������·���������£��� MovingState ��ͳһ����
    /// </summary>
    private void MoveTowards(Vector3 targetPosition)
    {
        // ����Ŀ�귽�򣨺��� Y �ᣩ
        Vector3 direction = new Vector3(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z).normalized;
        if (direction != Vector3.zero)
        {
            // ����Ŀ����ת�Ƕȣ���ƽ����ת
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        // �Թ̶������ƶ���Ŀ��λ��
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, targetPosition.z), step);
    }

    /// <summary>
    /// ����������ת��Ϊ��������
    /// </summary>
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    /// <summary>
    /// ����������ת��Ϊ�������꣨���� Y �ᣩ
    /// </summary>
    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }

    /// <summary>
    /// �����µ�Ŀ��㣬�л�״̬Ϊ�ƶ�
    /// </summary>
    public virtual void SetTarget(Vector3 newTarget)
    {
        target = newTarget;
        currentBaseState = (newTarget != null) ? SoldierBaseState.Moving : SoldierBaseState.Idle;
    }

    /// <summary>
    /// ���ӻ�·����ʹ�� LineRenderer չʾ��ǰ·��
    /// </summary>
    public void SetLine(List<ThetaStarNode> path)
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = path.Count;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        List<Vector3> pathPoints = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 point = new Vector3(path[i].Position.x, 1, path[i].Position.y);
            pathPoints.Add(point);
        }
        lineRenderer.SetPositions(pathPoints.ToArray());
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    /// <summary>
    /// ���·�����ӻ�������Ŀ�����ã�
    /// </summary>
    private void ClearPathVisualization()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }
}
