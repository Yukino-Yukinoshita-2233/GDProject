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

    protected override void AttackingState()
    {
        if (currentMonsterTarget != null && detectedMonsters.FirstOrDefault() != null)
        {
            currentMonsterTarget = detectedMonsters.FirstOrDefault();
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                animator?.SetTrigger("isAttack01");
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
