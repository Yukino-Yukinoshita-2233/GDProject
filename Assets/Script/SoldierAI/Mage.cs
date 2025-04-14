using System.Collections;
using System.Linq;
using UnityEngine;

public class Mage : Soldier
{
    public LineRenderer laserBeam; // 预制的激光对象
    public Transform firePoint;

    //protected override void Start()
    //{
    //base.Start();
    //animator = GetComponent<Animator>();
    //attackCooldown = 5.0f; // 每 2 秒攻击一次
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

        // 如果目标格子发生变化，则标记需要更新路径
        if (WorldToGrid(target) != targetGridPosition)
        {
            isPathUpdate = true;
        }

        // 如果需要更新路径，则以当前单位位置作为新起点重新计算路径
        if (isPathUpdate)
        {
            lastUpdatePathTime = Time.time;
            UpdatePath();
            isPathUpdate = false;
        }

        // 当有有效路径且路径点索引未超出路径列表时，进行移动
        if (path.Count > 0 && currentPathIndex < path.Count)
        {
            // 取当前路径点的世界坐标
            Vector3 targetPosition = GridToWorld(path[currentPathIndex].Position);
            targetPosition.y = 1.45f; // 固定Y值，保证角色高度

            // 调用移动函数，将单位向目标点移动
            MoveTowards(targetPosition);

            // 在 MovingState 中判断是否到达当前路径点
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
                currentGridPosition = WorldToGrid(transform.position);

                // 如果已经走完所有路径点，则状态切换为攻击或闲置，并清除路径可视化
                if (currentPathIndex >= path.Count)
                {
                    currentBaseState = detectedMonsters.Count > 0 ? SoldierBaseState.Attacking : SoldierBaseState.Idle;
                    ClearPathVisualization();
                }
            }
        }
        else
        {
            // 没有路径时切换为空闲状态
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
                // 立即旋转朝向目标
                RotateTowardsTarget(currentMonsterTarget);

                StartCoroutine(LaserAttack(0.5f)); // 持续 1 秒的激光
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
            laserBeam.gameObject.SetActive(true); // 启用激光
            laserBeam.SetPosition(0, firePoint.position); // 设置起点为 Mage 的施法点
            laserBeam.SetPosition(1, currentMonsterTarget.transform.position); // 设置终点为怪物

            if (currentMonsterTarget) // 确保目标仍然存在
            {
                AttackTarget(currentMonsterTarget);
            }
            // 执行伤害逻辑



            laserBeam.gameObject.SetActive(false); // 1 秒后隐藏激光
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
