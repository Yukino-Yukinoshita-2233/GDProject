using UnityEngine;

// Goblin 类继承自 Monster，包含特定属性和行为
public class Goblin : Monster
{
    private void Awake()
    {
        health = 20; // 设置哥布林的生命值
        damage = 2;  // 设置哥布林的伤害值
        moveSpeed = 3f; // 设置哥布林的移动速度
    }

    // 覆写攻击方法，输出特定的攻击行为
    protected override void Attack()
    {
        base.Attack();
        //Debug.Log("Goblin deals " + damage + " damage.");
    }

    // 覆写死亡方法，执行特定死亡效果
    protected override void OnDeath()
    {
        base.OnDeath();
        //Debug.Log("Goblin death animation triggered.");
    }
}
