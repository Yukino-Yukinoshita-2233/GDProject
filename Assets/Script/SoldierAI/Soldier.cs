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

    private float lastAttackTime = 0f;
    private float lastUpdatePathTime = 0f;
    private List<LPAStarNode> path = new List<LPAStarNode>();
    private int currentPathIndex = 0;
    private Vector2Int currentGridPosition;
    private Vector2Int targetGridPosition;

    public int[,] gridMap;

    private bool isPathUpdate = false;
    [SerializeField]
    private GameObject currentMonsterTarget;
    private List<GameObject> detectedMonsters = new List<GameObject>();

    AttackRangeDetector attackRangeDetector;
    Health health;
    public Animator animator;
    //PathVisualizer pathVisualizer;  // 添加路径可视化器

    private void Start()
    {
        gridMap = MapManager.gridMap;
        currentGridPosition = WorldToGrid(transform.position);
        //Debug.Log("Soldiertarget:" + WorldToGrid(target.transform.position));
        target = transform.position;
        targetGridPosition = WorldToGrid(target);

        // 获取 PathVisualizer 组件
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

    private void Update()
    {
        if (attackRangeDetector.SoldierAttackList != null)
        {
            detectedMonsters = attackRangeDetector.SoldierAttackList;
        }
        // 如果血量低于等于 0，设置为死亡状态
        if (health.GetHealth() <= 0)
        {
            Debug.Log("士兵死亡：" + gameObject.name);
            //HealthBarManager.Instance.RemoveHealthBar(gameObject); // 移除血条
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
    private void JingJieIdleState()
    {

        if (detectedMonsters.Count > 0)
        {
            SwitchTarget();
        }
    }
    private void XunLuoIdleState()
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

    private void MovingState()
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
        Debug.Log(gameObject.name + "修改路径");
        targetGridPosition = WorldToGrid(target);
        path = LPAStar.FindPath(gridMap, currentGridPosition, targetGridPosition);
        currentPathIndex = 1;

        // 设置路径可视化
        SetLine(path);

        //if (path.Count == 0)
        //{
        //    currentBaseState = SoldierBaseState.Idle;
        //}
    }

    private void AttackingState()
    {

        if (currentMonsterTarget != null && detectedMonsters.First() != null)
        {
            currentMonsterTarget = detectedMonsters.First();
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                animator.SetTrigger("isAttack01");
                StartCoroutine(DelayAttack(0.5f)); // 1秒后执行伤害
                //AttackTarget(currentMonsterTarget);
            }
        }
        else
        {

            SwitchTarget();
        }
    }


    // 创建协程来延迟执行伤害
    private IEnumerator DelayAttack(float delayTime)
    {
        // 等待指定的时间
        yield return new WaitForSeconds(delayTime);

        // 执行伤害逻辑
        AttackTarget(currentMonsterTarget);
    }

    private void AttackTarget(GameObject monster)
    {
        if (monster != null)
        {
            monster.GetComponent<Health>()?.TakeDamage(attackDamage);
        }
    }

    private void SwitchTarget()
    {
        if(detectedMonsters.First() == null)
        {
            detectedMonsters.RemoveAt(0);
        }
        if (detectedMonsters.Count > 0)
        {
            currentMonsterTarget = detectedMonsters.First();
            currentBaseState = SoldierBaseState.Attacking;
        }
        else
        {
            currentBaseState = SoldierBaseState.Idle;
        }
    }

    private void DeadState()
    {
        SoldierManager.Instance.UnregisterSoldier(this);
        HealthBarManager.Instance.RemoveHealthBar(this.gameObject);
        Destroy(gameObject);
    }

    public void HandleMonsterDetected(GameObject monster)
    {
        if (!detectedMonsters.Contains(monster))
        {
            Debug.Log("Soldier Get "+ monster);
            detectedMonsters.Add(monster);
            SwitchTarget();
        }
    }

    public void HandleMonsterExit(GameObject monster)
    {
        detectedMonsters.Remove(monster);

        if (monster == currentMonsterTarget)
        {
            currentMonsterTarget = null;
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(targetPosition.x,1.5f,targetPosition.z));

    }

    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }

    public void SetTarget(Vector3 newTarget)
    {
        target = newTarget;
        currentBaseState = (newTarget != null) ? SoldierBaseState.Moving : SoldierBaseState.Idle;
    }

    /// <summary>
    /// 可视化寻路路劲
    /// </summary>
    public void SetLine(List<LPAStarNode> path)
    {
        // 获取或添加 LineRenderer 组件
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // 设置 LineRenderer 的基本属性
        lineRenderer.positionCount = path.Count;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // 设置路径点
        List<Vector3> pathPoints = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 point = new Vector3(path[i].Position.x, 1, path[i].Position.y);
            pathPoints.Add(point);
        }

        lineRenderer.SetPositions(pathPoints.ToArray());

        // 设置颜色渐变
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;

        // 设置材质
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

}
