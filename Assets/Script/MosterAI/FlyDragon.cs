using UnityEngine;

// Dragon 类继承自 Monster，包含特定属性和行为
public class FlyDragon : Monster
{
    private void Awake()
    {
        health = 10; // 设置飞龙的生命值
        damage = 6;  // 设置飞龙的伤害值
        moveSpeed = 4f; // 设置飞龙的移动速度
    }

    // 覆写攻击方法，输出特定的攻击行为
    protected override void Attack()
    {
        base.Attack();
        Debug.Log("FlyDragon deals " + damage + " damage.");
    }

    // 覆写死亡方法，执行特定死亡效果
    protected override void OnDeath()
    {
        base.OnDeath();
        Debug.Log("FlyDragon death animation triggered.");
    }
}
