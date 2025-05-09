using System.Collections;
using System.Linq;
using UnityEngine;

public class Mage : Soldier
{
    public LineRenderer laserBeam; // Ԥ�Ƶļ������
    public Transform firePoint;

    //protected override void Start()
    //{
    //base.Start();
    //animator = GetComponent<Animator>();
    //attackCooldown = 5.0f; // ÿ 2 �빥��һ��
    //attackDamage = 10;
    //}

    protected override void MovingState()
    {
        currentBaseState = detectedMonsters.Count > 0 ? SoldierBaseState.Attacking : SoldierBaseState.Moving;

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





    protected override void AttackingState()
    {
        if (currentMonsterTarget != null && detectedMonsters.FirstOrDefault() != null)
        {
            currentMonsterTarget = detectedMonsters.FirstOrDefault();
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                animator?.SetTrigger("isAttack01");
                // ������ת����Ŀ��
                RotateTowardsTarget(currentMonsterTarget);

                StartCoroutine(LaserAttack(0.5f)); // ���� 1 ��ļ���
            }
        }
        else
        {
            SwitchTarget();
        }
    }

    private IEnumerator LaserAttack(float duration)
    {
        if (currentMonsterTarget)
        {
            yield return new WaitForSeconds(duration);
            laserBeam.gameObject.SetActive(true); // ���ü���
            laserBeam.SetPosition(0, firePoint.position); // �������Ϊ Mage ��ʩ����
            laserBeam.SetPosition(1, currentMonsterTarget.transform.position); // �����յ�Ϊ����

            if (currentMonsterTarget) // ȷ��Ŀ����Ȼ����
            {
                AttackTarget(currentMonsterTarget);
            }
            // ִ���˺��߼�



            laserBeam.gameObject.SetActive(false); // 1 ������ؼ���
        }
    }

    protected override void HandleMonsterDetected(GameObject monster)
    {
        if (!detectedMonsters.Contains(monster))
        {
            Debug.Log("Soldier Get " + monster);
            detectedMonsters.Add(monster);
            SwitchTarget();
        }
    }
    protected override void HandleMonsterExit(GameObject monster)
    {
        detectedMonsters.Remove(monster);

        if (monster == currentMonsterTarget)
        {
            currentMonsterTarget = null;
        }
    }

}
