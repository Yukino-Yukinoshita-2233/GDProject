using System.Collections;
using System.Linq;
using UnityEngine;

public class Swordsman : Soldier
{
    //protected override void Start()
    //{
        //base.Start();
        //animator = GetComponent<Animator>();
        //attackCooldown = 1.0f;
        //attackDamage = 20;
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
                // 立即旋转朝向目标
                RotateTowardsTarget(currentMonsterTarget);

                StartCoroutine(DelayAttack(0.5f));
            }
        }
        else
        {
            SwitchTarget();
        }
    }

    private IEnumerator DelayAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 执行伤害逻辑
        AttackTarget(currentMonsterTarget);
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
