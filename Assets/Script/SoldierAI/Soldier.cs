using System.Collections.Generic;
using UnityEngine;
using MapManagernamespace;
using System.Linq;
using System.Collections;
using System.Threading;
using static UnityEngine.GraphicsBuffer;

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
    public Vector3 target;
    public float moveSpeed = 3.0f;
    public float attackCooldown = 1.0f;
    public float attackDamage = 10;
    public float maxHealth = 1000f;

    protected  float lastAttackTime = 0f;
    protected  float lastUpdatePathTime = 0f;
    protected  List<LPAStarNode> path = new List<LPAStarNode>();
    protected  int currentPathIndex = 0;
    protected  Vector2Int currentGridPosition;
    protected Vector2Int targetGridPosition;

    public int[,] gridMap;

    protected bool isPathUpdate = false;
    [SerializeField]
    protected  GameObject currentMonsterTarget;
    protected List<GameObject> detectedMonsters = new List<GameObject>();

    protected AttackRangeDetector attackRangeDetector;
    protected Health health;
    public Animator animator;
    //PathVisualizer pathVisualizer;  // ���·�����ӻ���

    protected virtual void Start()
    {
        gridMap = MapManager.gridMap;
        currentGridPosition = WorldToGrid(transform.position);
        //Debug.Log("Soldiertarget:" + WorldToGrid(target.transform.position));
        target = transform.position;
        targetGridPosition = WorldToGrid(target);

        // ��ȡ PathVisualizer ���
        //pathVisualizer = GetComponent<PathVisualizer>();
        //if (pathVisualizer == null)
        //{
        //    pathVisualizer = gameObject.AddComponent<PathVisualizer>();
        //}

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
        if (attackRangeDetector.SoldierAttackList != null)
        {
            detectedMonsters = attackRangeDetector.SoldierAttackList;
        }
        // ���Ѫ�����ڵ��� 0������Ϊ����״̬
        if (health.GetHealth() <= 0)
        {
            Debug.Log("ʿ��������" + gameObject.name);
            //HealthBarManager.Instance.RemoveHealthBar(gameObject); // �Ƴ�Ѫ��
            currentBaseState = SoldierBaseState.Dead;
        }

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
                    AttackingState();
                    break;
                case SoldierBaseState.Dead:
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
                    AttackingState();
                    break;
                case SoldierBaseState.Dead:
                    animator.SetBool("isDead", true);
                    DeadState();
                    break;
            }


        }
    } 





    void FixedUpdate()
    {
        Quaternion fixedRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        GetComponent<Rigidbody>().MoveRotation(fixedRotation);
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
        if (WorldToGrid(target) != targetGridPosition)
        {
            isPathUpdate = true;
        }

        if (isPathUpdate)// && Time.time - lastUpdatePathTime >= 1f
        {
            lastUpdatePathTime = Time.time;

            UpdatePath();
            isPathUpdate = false;
        }



        if (path.Count > 0 && currentPathIndex < path.Count)
        {
            Vector3 targetPosition = GridToWorld(path[currentPathIndex].Position);
            targetPosition.y = 1.45f;

            MoveTowards(targetPosition);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
                currentGridPosition = WorldToGrid(transform.position);

                if (currentPathIndex >= path.Count)
                {
                    currentBaseState = detectedMonsters.Count > 0 ? SoldierBaseState.Attacking : SoldierBaseState.Idle;
                }
            }
        }
    }

    private void UpdatePath()
    {
        Debug.Log(gameObject.name + "�޸�·��");
        targetGridPosition = WorldToGrid(target);
        path = LPAStar.FindPath(gridMap, currentGridPosition, targetGridPosition);
        currentPathIndex = 1;

        // ����·�����ӻ�
        SetLine(path);

        //if (path.Count == 0)
        //{
        //    currentBaseState = SoldierBaseState.Idle;
        //}
    }

    protected virtual void AttackingState()
    {

        if (currentMonsterTarget != null && detectedMonsters.FirstOrDefault() != null)
        {
            currentMonsterTarget = detectedMonsters.FirstOrDefault();
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                animator.SetTrigger("isAttack01");
                StartCoroutine(DelayAttack(0.5f)); // 1���ִ���˺�
                //AttackTarget(currentMonsterTarget);
            }
        }
        else
        {

            SwitchTarget();
        }
    }


    // ����Э�����ӳ�ִ���˺�
    private IEnumerator DelayAttack(float delayTime)
    {
        // �ȴ�ָ����ʱ��
        yield return new WaitForSeconds(delayTime);

        // ִ���˺��߼�
        AttackTarget(currentMonsterTarget);
    }

    protected void AttackTarget(GameObject monster)
    {
        if (monster != null)
        {
            monster.GetComponent<Health>()?.TakeDamage(attackDamage);
        }
    }

    protected void SwitchTarget()
    {
        if (detectedMonsters.Count == 0) // �ȼ���б��Ƿ�Ϊ��
        {
            currentBaseState = SoldierBaseState.Idle;
            return;
        }

        if (detectedMonsters.First() == null) // ȷ�� First() ��Ԫ�ؿ�ȡ
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
            Debug.Log("Soldier Get "+ monster);
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

    private void MoveTowards(Vector3 targetPosition)
    {
        // ����Ŀ�귽�򣨺��� Y �ᣬ��ֹ��ɫ��б��
        Vector3 direction = new Vector3(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z).normalized;

        if (direction != Vector3.zero) // ��������������
        {
            // ����Ŀ����ת�Ƕȣ����� XZ ƽ����ת��
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // ִ���ƶ������� Y ��߶Ȳ��䣩
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, targetPosition.z), step);

        // ����Ƿ񵽴�Ŀ��㣬���⸡������ס��ɫ
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++; // ǰ������һ��·����
            currentGridPosition = WorldToGrid(transform.position);
        }
    }


    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }

    public virtual void SetTarget(Vector3 newTarget)
    {
        target = newTarget;
        currentBaseState = (newTarget != null) ? SoldierBaseState.Moving : SoldierBaseState.Idle;
    }

    /// <summary>
    /// ���ӻ�Ѱ··��
    /// </summary>
    public void SetLine(List<LPAStarNode> path)
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

}
