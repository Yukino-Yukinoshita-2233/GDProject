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
    //protected PathVisualizer pathVisualizer;

    // ��ǰ����Ĺ���Ŀ��
    protected List<GameObject> currentTargets = new List<GameObject>();

    // ·�����¼������
    Vector2Int oldCastlePosition = Vector2Int.zero;    // �ϴγǱ���λ�ã������ж��Ƿ���Ҫ���¼���·��

    // �����״̬ö��
    public enum State { Idling, Moving, Attacking, Dead }
    public State currentState;  // ��ǰ״̬
    float NTime;
    public Animator animator;
    /// <summary>
    /// ��ʼ������
    /// </summary>
    public void Initialize(Vector2Int startPos, Vector2Int targetPos, int[,] map)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;
        this.map = map;
        this.currentHealth = maxHealth;

        // ʹ�� Theta* �㷨����·��
        path = ThetaStar.FindPath(map, startPos, targetPos);
        oldCastlePosition = targetPos;

        if(path == null) { currentState = State.Dead; }
        // ����·�����ӻ�
        SetLine(path);

        currentState = State.Moving;
        thisMonsterHealth = transform.GetComponent<Health>();

        NTime = 0f;

    }

    /// <summary>
    /// ���¹����״̬���� MonsterManager ����
    /// </summary>
    public void UpdateMonster()
    {
        // ���Ѫ�����ڵ��� 0������Ϊ����״̬
        if (thisMonsterHealth.GetHealth() <= 0)
        {
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
                animator.SetFloat("isRun", 0f);
                FindNewTarget(); // Ѱ����Ŀ�꣨�շ�����������ʵ�֣�
                break;
            case State.Moving:
                animator.SetFloat("isRun", 1f);
                MoveTowardsTarget(); // ִ���ƶ��߼�
                break;
            case State.Attacking:
                Attack(); // ִ�й����߼�
                break;
            case State.Dead:
                animator.SetBool("isDead", true);
                OnDeath(); // ִ�������߼�
                break;
        }

        if (NTime > 5 && (path == null || currentPathIndex >= path.Count) && currentTargets.Count == 0)
        {
            currentState = State.Dead;
            return;
        }
        else { NTime += Time.deltaTime; }
    }

    public void StateHandling()
    {

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

            // ����·�����ӻ�
            //SetLine(path);
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
        transform.LookAt(targetPosition);


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
                animator.SetTrigger("isAttack01");
                // ����Э�̣��ȴ�����������Ϻ�ִ���˺�
                StartCoroutine(DelayAttack(targetHealth, 0.5f)); // 1���ִ���˺�
                //targetHealth.TakeDamage(damage); // ��Ŀ������˺�
                //Debug.Log("���﹥�����˺���" + damage);
            }
        }

        lastAttackTime = Time.time; // ������󹥻�ʱ��
    }

    // ����Э�����ӳ�ִ���˺�
    private IEnumerator DelayAttack(Health targetHealth, float delayTime)
    {
        // �ȴ�ָ����ʱ��
        yield return new WaitForSeconds(delayTime);

        // ִ���˺��߼�
        targetHealth.TakeDamage(damage);
        Debug.Log("���﹥�����˺���" + damage);
    }

    /// <summary>
    /// ��������������߼�
    /// </summary>
    protected virtual void OnDeath()
    {
        Debug.Log("����������" + gameObject.name);
        HealthBarManager.Instance.RemoveHealthBar(gameObject); // �Ƴ�Ѫ��
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

    /// <summary>
    /// ���ӻ�Ѱ··��
    /// </summary>
    public void SetLine(List<ThetaStarNode> path)
    {
        // ��ȡ����� LineRenderer ���
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // ���� LineRenderer �Ļ�������
        lineRenderer.positionCount = path.Count;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        // ����·����
        List<Vector3> pathPoints = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 point = new Vector3(path[i].Position.x, 1, path[i].Position.y);
            pathPoints.Add(point);
        }

        lineRenderer.SetPositions(pathPoints.ToArray());

        // ������ɫ����
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;

        // ���ò���
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
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
            currentState = State.Moving;
        }
    }
    protected virtual void HandleBuildingDetected(GameObject building) { }
    protected virtual void HandleSoldierDetected(GameObject soldier) { }
    protected virtual void HandleBuildingExit(GameObject building) { }
    protected virtual void HandleSoldierExit(GameObject soldier) { }
}
