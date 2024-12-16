using UnityEngine;

// Dragon 类继承自 Monster，包含特定属性和行为
public class Goblin : Monster
{
    void Awake()
    {
        moveSpeed = 2f;
        damage = 10;
        maxHealth = 50f;
        attackCooldown = 2.0f;
        currentHealth = maxHealth;
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
        //Debug.Log("Goblin is attacking!");
    }



}