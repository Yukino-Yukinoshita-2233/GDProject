using System.Linq;
using UnityEngine;

// Dragon 类继承自 Monster，包含特定属性和行为
public class RockDragon : Monster
{
    //private float lastAttackTime;  // 上一次攻击的时间
    //private float attackCooldownTimer;  // 攻击冷却计时器
    //AttackRangeDetector attackRangeDetector;
    // 石龙的初始化
    void Awake()
    {
        moveSpeed = 0.5f;  // 设置石龙的移动速度
        damage = 20;     // 设置石龙的攻击力
        maxHealth = 200; // 设置石龙的最大血量
        attackCooldown = 2.0f;  // 设置攻击冷却时间
        currentHealth = maxHealth;

        // 获取攻击范围检测器并注册事件
        //attackRangeDetector = GetComponentInChildren<AttackRangeDetector>();
        //if (attackRangeDetector != null)
        //{
        //    attackRangeDetector.OnBuildingDetected += HandleBuildingDetected;
        //    attackRangeDetector.OnSoldierDetected += HandleSoldierDetected;
        //    attackRangeDetector.OnBuildingExited += HandleBuildingExit;
        //    attackRangeDetector.OnSoldierExited += HandleSoldierExit;
        //}

        // 获取血量组件并监听血量变化
        Health health = gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.SetHealth(maxHealth);
            //health.healthChange += TakeDamage;
        }

        // 获取攻击范围检测器
        attackRangeDetector = GetComponentInChildren<AttackRangeDetector>();
        lastAttackTime = Time.time;  // 初始化攻击时间

        //lastAttackTime = Time.time;  // 初始化攻击时间
        //attackCooldownTimer = attackCooldown;  // 初始化攻击冷却计时器
    }

    // 处理建筑物的检测逻辑
    protected override void HandleBuildingDetected(GameObject building)
    {
        if (!currentTargets.Contains(building))
        {
            currentTargets.Add(building);  // 如果目标是新的建筑物，添加到攻击目标列表
            //building.GetComponent<Health>().TakeDamage(damage * 2);  // 石龙对建筑物造成双倍伤害
        }
    }

    // 处理士兵的检测逻辑
    protected override void HandleSoldierDetected(GameObject soldier)
    {
        if (!currentTargets.Contains(soldier))
        {
            currentTargets.Add(soldier);  // 如果目标是新的士兵，添加到攻击目标列表
            //soldier.GetComponent<Health>().TakeDamage(damage);  // 哥布林对士兵造成的伤害
        }
    }

    // 攻击逻辑（针对当前目标进行攻击）
    //protected override void Attack()
    //{
        // 检查是否符合攻击冷却条件
        //if (Time.time - lastAttackTime >= attackCooldown && currentTargets.Count != 0)
        //{
        //    GameObject target = currentTargets.First();
        //    if (target != null)
        //    {
        //        Health targetHealth = target.GetComponent<Health>();
        //        if (targetHealth != null)
        //        {
        //            Debug.Log("RockDragon,Damage:" + damage);
        //            targetHealth.TakeDamage(damage);  // 对每个目标造成伤害
        //        }
        //    }

        //    lastAttackTime = Time.time;  // 更新最后攻击的时间
        //}
   //}

    // 处理建筑物离开攻击范围的逻辑
    protected override void HandleBuildingExit(GameObject building)
    {
        if (currentTargets.Contains(building))
        {
            currentTargets.Remove(building);  // 如果目标是建筑物，移除它
            if (currentTargets.Count == 0)
                currentState = State.Moving;  // 如果没有目标，恢复移动状态
        }
    }

    // 处理士兵离开攻击范围的逻辑
    protected override void HandleSoldierExit(GameObject soldier)
    {
        if (currentTargets.Contains(soldier))
        {
            currentTargets.Remove(soldier);  // 如果目标是士兵，移除它
            if (currentTargets.Count == 0)
                currentState = State.Moving;  // 如果没有目标，恢复移动状态
        }
    }
}