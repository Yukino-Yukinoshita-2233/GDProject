using UnityEngine;

// Dragon 类继承自 Monster，包含特定属性和行为
public class RockDragon : Monster
{
    void Awake()
    {
        moveSpeed = 1f;
        damage = 20;
        maxHealth = 200f;
        currentHealth = maxHealth;
        attackCooldown = 1.0f;

        // 找到子物体上的AttackRangeDetector脚本
        AttackRangeDetector attackRangeDetector = GetComponentInChildren<AttackRangeDetector>();
        Health health = gameObject.GetComponent<Health>();

        // 监听子物体触发的事件
        if (attackRangeDetector != null)
        {
            attackRangeDetector.OnBuildingDetected += HandleBuildingDetected;
            attackRangeDetector.OnSoldierDetected += HandleSoldierDetected;
        }

        // 监听物体触发的事件
        if (health != null)
        {
            health.healthChange += TakeDamage;
        }

    }

    protected override void Attack()
    {
        //Debug.Log("RockDragon is using a heavy attack!");
    }
}