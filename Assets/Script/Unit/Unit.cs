using UnityEngine;

/// <summary>
/// 抽象基类 Unit，表示所有单位。
/// </summary>
public abstract class Unit
{
    // 单位的名称
    public string Name { get; private set; }

    // 单位的血量
    public int Health { get; protected set; }

    // 单位的攻击力
    public int Attack { get; protected set; }

    // 单位的位置
    public Vector2 Position { get; set; }

    /// <summary>
    /// Unit 类的构造函数。
    /// </summary>
    /// <param name="name">单位名称。</param>
    /// <param name="health">单位血量。</param>
    /// <param name="attack">单位攻击力。</param>
    public Unit(string name, int health, int attack)
    {
        Name = name;
        Health = health;
        Attack = attack;
    }

    /// <summary>
    /// 抽象方法 OnMove，用于移动单位。
    /// </summary>
    /// <param name="targetPosition">目标位置。</param>
    public abstract void OnMove(Vector2 targetPosition);

    /// <summary>
    /// 抽象方法 OnAttack，用于攻击目标单位。
    /// </summary>
    /// <param name="target">目标单位。</param>
    public abstract void OnAttack(Unit target);
}

/// <summary>
/// FriendlyUnit 类，继承自 Unit，表示友方单位。
/// </summary>
public class FriendlyUnit : Unit
{
    public FriendlyUnit(string name, int health, int attack) : base(name, health, attack)
    {
    }

    public override void OnMove(Vector2 targetPosition)
    {
        // 实现友方单位的移动逻辑
        Debug.Log($"{Name} 移动到 {targetPosition}");
    }

    public override void OnAttack(Unit target)
    {
        // 实现友方单位的攻击逻辑
        Debug.Log($"{Name} 攻击 {target.Name}");
    }
}

/// <summary>
/// EnemyUnit 类，继承自 Unit，表示敌方单位。
/// </summary>
public class EnemyUnit : Unit
{
    public EnemyUnit(string name, int health, int attack) : base(name, health, attack)
    {
    }

    public override void OnMove(Vector2 targetPosition)
    {
        // 实现敌方单位的移动逻辑
        Debug.Log($"{Name} 移动到 {targetPosition}");
    }

    public override void OnAttack(Unit target)
    {
        // 实现敌方单位的攻击逻辑
        Debug.Log($"{Name} 攻击 {target.Name}");
    }
}
